using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipelines;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using TinyCsvParser.Mapping;

namespace TinyCsvParser
{
    // https://blogs.msdn.microsoft.com/dotnet/2018/07/09/system-io-pipelines-high-performance-io-in-net/
    // https://blog.marcgravell.com/2018/07/pipe-dreams-part-1.html

    // TODO: Experiment with StreamConnection from https://github.com/mgravell/Pipelines.Sockets.Unofficial
    // Useful examples there, including PipeTextReader and MemoryMappedPipeReader

    internal class Piper
    {
        public static IObservable<T> ObserveFile<T>(string path, Encoding encoding, CsvParser<T> parser) where T : new()
        {
            return ObserveStream(File.OpenRead(path), encoding, parser);
        }

        public static IObservable<T> ObserveFile<T>(FileInfo file, Encoding encoding, CsvParser<T> parser) where T : new()
        {
            return ObserveStream(file.OpenRead(), encoding, parser);
        }

        public static IObservable<T> ObserveStream<T>(Stream fileStream, Encoding encoding, CsvParser<T> parser) where T : new()
        {
            if (fileStream is null) throw new ArgumentNullException(nameof(fileStream));
            if (parser is null) throw new ArgumentException(nameof(parser));

            return new LineObservable<T>(fileStream, observable =>
            {
                var pipe = new Pipe();
                Task writing = FillPipeAsync(fileStream, pipe.Writer);
                Task reading = ReadPipeAsync(pipe.Reader, encoding, parser, observable);

                Task.WhenAll(reading, writing)
                    .ContinueWith(_ => observable.OnCompleted(),
                        TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.ExecuteSynchronously);
            });

        }

        private static async Task FillPipeAsync(Stream fileStream, PipeWriter writer)
        {
            const int minimumBufferSize = 512;

            while (true)
            {
                try
                {
                    // Request a minimum of 512 bytes from the PipeWriter
                    Memory<byte> memory = writer.GetMemory(minimumBufferSize);
                    int bytesRead = await fileStream.ReadAsync(memory);
                    if (bytesRead == 0)
                    {
                        break;
                    }

                    // Tell the PipeWriter how much was read
                    writer.Advance(bytesRead);
                }
                catch
                {
                    break;
                }

                // Make the data available to the PipeReader
                FlushResult result = await writer.FlushAsync();

                if (result.IsCompleted)
                {
                    break;
                }
            }

            // Signal to the reader that we're done writing
            writer.Complete();
        }

        private static async Task ReadPipeAsync<T>(PipeReader reader, Encoding encoding, CsvParser<T> parser, IObserver<T> observer) where T : new()
        {
            while (true)
            {
                try
                {
                    ReadResult result = await reader.ReadAsync();

                    ReadOnlySequence<byte> buffer = result.Buffer;

                    if (buffer.IsEmpty && (result.IsCompleted || result.IsCanceled))
                        break;

                    SequencePosition? position = null;
                    int lineNum = 0;

                    do
                    {
                        // Find the EOL
                        position = buffer.PositionOf((byte)'\n');

                        if (position != null)
                        {
                            var line = buffer.Slice(0, position.Value);

                            if (lineNum > 0 || !parser.Options.SkipHeader)
                            {
                                var parsed = ProcessLine(line, encoding, parser, lineNum);

                                if (parsed.HasValue)
                                {
                                    var val = parsed.Value;
                                    if (val.IsValid)
                                    {
                                        observer.OnNext(val.Result);
                                    }
                                    else
                                    {
                                        observer.OnError(val.Error);
                                        goto done;
                                    }
                                }
                            }
                            lineNum++;

                            // This is equivalent to position + 1
                            var next = buffer.GetPosition(1, position.Value);

                            // Skip what we've already processed including \n
                            buffer = buffer.Slice(next);
                        }
                    }
                    while (position != null);

                    // We sliced the buffer until no more data could be processed
                    // Tell the PipeReader how much we consumed and how much we have left to process
                    reader.AdvanceTo(buffer.Start, buffer.End);

                    if (result.IsCompleted || result.IsCanceled)
                    {
                        break;
                    }
                }
                catch (Exception exn)
                {
                    observer.OnError(exn);
                    break;
                }
            }

        done:
            reader.Complete();
            observer.OnCompleted();
        }

        private static CsvMappingResult<T>? ProcessLine<T>(in ReadOnlySequence<byte> buffer, Encoding encoding, CsvParser<T> parser, int lineNum) where T : new()
        {
            var pool = MemoryPool<char>.Shared;
            int maxChars;

            if (buffer.IsSingleSegment)
            {
                var bytes = buffer.First.Span;
                maxChars = encoding.GetMaxCharCount(bytes.Length);
                using (var chrMem = pool.Rent(maxChars))
                {
                    var chars = chrMem.Memory.Span;
                    var charCount = encoding.GetChars(bytes, chars);
                    var line = chars.Slice(0, charCount);
                    return ParseLine(parser, lineNum, line);
                }
            }

            int len = 0;
            maxChars = encoding.GetMaxCharCount((int)buffer.Length);
            using (var chrMem = pool.Rent(maxChars))
            {
                var chars = chrMem.Memory.Span;
                foreach (var segment in buffer)
                {
                    var bytes = segment.Span;
                    var charCount = encoding.GetChars(bytes, chars.Slice(len));
                    len += charCount;
                }

                var line = chars.Slice(0, len);
                return ParseLine(parser, lineNum, line);
            }
        }

        private static CsvMappingResult<T>? ParseLine<T>(CsvParser<T> parser, int lineNum, in ReadOnlySpan<char> line) where T : new()
        {
            if (line.IsEmpty)
                return null;

            var commentChar = parser.Options.CommentCharacter;
            if (!string.IsNullOrEmpty(commentChar) && line.StartsWith(commentChar))
                return null;

            // since we're splitting on \n, a \r\n file
            // will have \r at the end of each line, so slice that off
            if (line[line.Length - 1] == '\r')
                return parser.ParseLine(line.Slice(0, line.Length - 1), lineNum);

            return parser.ParseLine(line, lineNum);
        }
    }

    internal class LineObservable<T> : SubjectBase<T> where T : new()
    {
        private readonly List<IObserver<T>> _observers;
        private IDisposable _resource;
        private Action<IObserver<T>> _startAction;
        private bool _disposed = false;
        private bool _started = false;

        public LineObservable(IDisposable resource, Action<IObserver<T>> startAction)
        {
            _resource = resource;
            _startAction = startAction;
            _observers = new List<IObserver<T>>();
        }

        public override bool HasObservers => _observers.Count > 0;

        public override bool IsDisposed => _disposed;

        public override void OnCompleted()
        {
            //Debug.WriteLine("OnCompleted");
            foreach (var observer in _observers.ToArray())
            {
                observer.OnCompleted();
            }
            Dispose();
        }

        public override void OnError(Exception error)
        {
            //Debug.WriteLine("OnError: {0}", error);
            foreach (var observer in _observers.ToArray())
            {
                observer.OnError(error);
            }
            Dispose();
        }

        public override void OnNext(T value)
        {
            //Debug.WriteLine("OnNext: {0}", value);
            foreach (var observer in _observers)
            {
                observer.OnNext(value);
            }
        }

        public override IDisposable Subscribe(IObserver<T> observer)
        {
            if (observer is null) throw new ArgumentNullException(nameof(observer));

            if (_disposed)
                throw new ObjectDisposedException("LineObservable");

            if (_observers.Contains(observer))
            {
                return Disposable.Empty;
            }

            _observers.Add(observer);

            if (!_started)
            {
                Debug.WriteLine("Starting Action");
                _started = true;
                _startAction?.Invoke(this);
            }

            return Disposable.Create(() =>
            {
                _observers.Remove(observer);
            });
        }

        public override void Dispose()
        {
            if (_disposed) return;

            _observers.Clear();
            _resource?.Dispose();
            _startAction = null;
            _resource = null;
            _disposed = true;
        }
    }
}


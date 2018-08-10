using CoreCsvParser.Mapping;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipelines;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace CoreCsvParser
{
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
                    .ContinueWith(x => observable.OnCompleted(), 
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
                    SequencePosition? position = null;
                    int lineNum = 0;

                    do
                    {
                        // Find the EOL
                        position = buffer.PositionOf((byte)'\r');
                        bool foundCR = !(position is null);
                        if (!foundCR)
                            position = buffer.PositionOf((byte)'\n');

                        if (position != null)
                        {
                            var line = buffer.Slice(0, position.Value);
                            if (foundCR && !line.First.IsEmpty && line.First.Span[0] == (byte)'\n')
                            {
                                line = line.Slice(1);
                            }

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

                    if (result.IsCompleted)
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
        }

        private static CsvMappingResult<T>? ProcessLine<T>(in ReadOnlySequence<byte> buffer, Encoding encoding, CsvParser<T> parser, int lineNum) where T : new()
        {
            var pool = MemoryPool<char>.Shared;
            var sb = new StringBuilder();

            foreach (var segment in buffer)
            {
                var bytes = segment.Span;
                var maxChars = encoding.GetMaxCharCount(bytes.Length);
                using (var chrMem = pool.Rent(maxChars))
                {
                    var chars = chrMem.Memory.Span;
                    var charCount = encoding.GetChars(bytes, chars);
                    sb.Append(chars.Slice(0, charCount));
                }
            }

            CsvMappingResult<T>? result = null;
            using (var chrMem = pool.Rent(sb.Length))
            {
                var chars = chrMem.Memory.Span;
                sb.CopyTo(0, chars, sb.Length);
                var line = chars.Slice(0, sb.Length);

                if (!line.IsEmpty
                    && (string.IsNullOrEmpty(parser.Options.CommentCharacter)
                        || !line.StartsWith(parser.Options.CommentCharacter)))
                {
                    result = parser.ParseLine(line, lineNum);
                }

            }

            return result;
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
            foreach (var observer in _observers)
            {
                observer.OnCompleted();
            }
            Dispose();
        }

        public override void OnError(Exception error)
        {
            foreach (var observer in _observers)
            {
                observer.OnError(error);
            }
            Dispose();
        }

        public override void OnNext(T value)
        {
            foreach (var observer in _observers)
            {
                observer.OnNext(value);
            }
        }

        public override IDisposable Subscribe(IObserver<T> observer)
        {
            if (observer is null) throw new ArgumentNullException(nameof(observer));

            if (_disposed || _observers.Contains(observer))
            {
                return Disposable.Empty;
            }
            
            _observers.Add(observer);

            if (!_started)
            {
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


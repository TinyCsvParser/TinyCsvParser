using System;
using System.Buffers;
using System.IO;
using System.IO.Pipelines;
using System.Text;
using System.Threading.Tasks;

namespace TinyCsvParser
{
    public class Piper
    {
        public static async Task ReadFileAsync<T>(string path, Encoding encoding, CsvParser<T> parser) where T : new()
        {
            using (var fs = File.OpenRead(path))
            {
                await ReadStreamAsync(fs, encoding, parser);
            }
        }

        public static async Task ReadFileAsync<T>(FileInfo file, Encoding encoding, CsvParser<T> parser) where T : new()
        {
            using (var fs = file.OpenRead())
            {
                await ReadStreamAsync(fs, encoding, parser);
            }
        }

        public static async Task ReadStreamAsync<T>(Stream fileStream, Encoding encoding, CsvParser<T> parser) where T : new()
        {
            var pipe = new Pipe();
            Task writing = FillPipeAsync(fileStream, pipe.Writer);
            Task reading = ReadPipeAsync(pipe.Reader, encoding, parser);

            await Task.WhenAll(reading, writing);
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

        private static async Task ReadPipeAsync<T>(PipeReader reader, Encoding encoding, CsvParser<T> parser) where T : new()
        {
            while (true)
            {
                ReadResult result = await reader.ReadAsync();

                ReadOnlySequence<byte> buffer = result.Buffer;
                SequencePosition? position = null;
                int lineNum = 0;

                do
                {
                    // Find the EOL
                    position = buffer.PositionOf((byte)'\r') ?? buffer.PositionOf((byte)'\n');

                    if (position != null)
                    {
                        var line = buffer.Slice(0, position.Value);
                        if (!line.First.IsEmpty && line.First.Span[0] == (byte)'\n')
                        {
                            line = line.Slice(1);
                        }
                        ProcessLine(line, encoding, parser, lineNum++);

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

            reader.Complete();
        }

        private static void ProcessLine<T>(in ReadOnlySequence<byte> buffer, Encoding encoding, CsvParser<T> parser, int lineNum) where T : new()
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

            Console.WriteLine(sb.ToString());

            using (var chrMem = pool.Rent(sb.Length))
            {
                var chars = chrMem.Memory.Span;
                sb.CopyTo(0, chars, sb.Length);
                var result = parser.ParseLine(chars.Slice(0, sb.Length), lineNum);
                if (result.IsValid)
                    Console.WriteLine(result.Result.ToString());
                else
                    Console.Write(result.Error.ToString());
            }
        }
    }
}


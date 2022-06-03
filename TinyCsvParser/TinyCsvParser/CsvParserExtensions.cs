// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TinyCsvParser.Mapping;
using TinyCsvParser.Model;

namespace TinyCsvParser
{
    public static class CsvParserExtensions
    {
        public static ParallelQuery<CsvMappingResult<TEntity>> ReadFromFile<TEntity>(this ICsvParser<TEntity> csvParser, string fileName, Encoding encoding)
        {
            if (fileName == null)
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            var lines = File
                .ReadLines(fileName, encoding)
                .Select((line, index) => new Row(index, line));

            return csvParser.Parse(lines);
        }

        public static ParallelQuery<CsvMappingResult<TEntity>> ReadFromString<TEntity>(this ICsvParser<TEntity> csvParser, CsvReaderOptions csvReaderOptions, string csvData)
        {
            var lines = csvData
                .Split(csvReaderOptions.NewLine, StringSplitOptions.None)
                .Select((line, index) => new Row(index, line));

            return csvParser.Parse(lines);
        }

        private static IEnumerable<string> ReadLinesFromStream(Stream stream, Encoding encoding, bool detectEncodingFromByteOrderMarks = false, int bufferSize = 1024, bool leaveOpen = false)
        {
            using (var reader = new StreamReader(stream, encoding, detectEncodingFromByteOrderMarks, bufferSize, leaveOpen))
            {
                while (!reader.EndOfStream)
                {
                    yield return reader.ReadLine();
                }
            }
        }

        public static ParallelQuery<CsvMappingResult<TEntity>> ReadFromStream<TEntity>(this ICsvParser<TEntity> csvParser, Stream stream, Encoding encoding, bool detectEncodingFromByteOrderMarks = false, int bufferSize = 1024, bool leaveOpen = false)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            var lines = ReadLinesFromStream(stream, encoding, detectEncodingFromByteOrderMarks, bufferSize, leaveOpen)
                .Select((line, index) => new Row(index, line));

            return csvParser.Parse(lines);
        }
    }
}

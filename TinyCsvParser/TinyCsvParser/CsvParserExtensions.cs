// Copyright (c) Philipp Wagner. All rights reserved.
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
        public static ParallelQuery<CsvMappingResult<TEntity>> ReadFromFile<TEntity>(this CsvParser<TEntity> csvParser, string fileName, Encoding encoding)
            where TEntity : class, new()
        {
            if (fileName == null)
            {
                throw new ArgumentNullException("fileName");
            }

            var lines = File
                .ReadLines(fileName, encoding)
                .Select((line, index) => new Row(index, line));

            return csvParser.Parse(lines);
        }

        public static ParallelQuery<CsvMappingResult<TEntity>> ReadFromString<TEntity>(this CsvParser<TEntity> csvParser, CsvReaderOptions csvReaderOptions, string csvData)
            where TEntity : class, new()
        {
            var lines = csvData
                .Split(csvReaderOptions.NewLine, StringSplitOptions.None)
                .Select((line, index) => new Row(index, line));

            return csvParser.Parse(lines);
        }

        private static IEnumerable<string> ReadLinesFromStream(Stream stream, Encoding encoding)
        {
            using (var reader = new StreamReader(stream, encoding, false, 4096, true)) // This stream is volontary kept open allow other uses of it
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    yield return line;
                }
            }
        }

        public static ParallelQuery<CsvMappingResult<TEntity>> ReadFromStream<TEntity>(this CsvParser<TEntity> csvParser, Stream stream, Encoding encoding)
            where TEntity : class, new()
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            var lines = ReadLinesFromStream(stream, encoding)
                .Select((line, index) => new Row(index, line));

            return csvParser.Parse(lines);
        }
    }
}

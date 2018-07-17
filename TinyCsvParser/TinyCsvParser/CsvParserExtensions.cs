// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TinyCsvParser.Extensions;
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
                .Select((line, index) => new Row(index, line.AsMemory()));

            return csvParser.Parse(lines);
        }

        public static ParallelQuery<CsvMappingResult<TEntity>> ReadFromString<TEntity>(this CsvParser<TEntity> csvParser, CsvReaderOptions csvReaderOptions, string csvData)
            where TEntity : class, new()
        {
            var lines = csvData
                .Split(csvReaderOptions.NewLine, StringSplitOptions.None)
                .Select((line, index) => new Row(index, line.AsMemory()));

            return csvParser.Parse(lines);
        }

        public static ParallelQuery<CsvMappingResult<TEntity>> ReadFromSpan<TEntity>(this CsvParser<TEntity> csvParser, CsvReaderOptions csvReaderOptions, ReadOnlySpan<char> csvData)
            where TEntity : class, new()
        {
            var lines = new List<Row>();
            var parts = csvData.Split(csvReaderOptions.NewLine, StringSplitOptions.RemoveEmptyEntries);
            var i = 0;

            foreach (var part in parts)
            {
                lines.Add(new Row(i++, part.ToArray().AsMemory()));
            }

            // TODO: can we make an overload of csvParser.Parse that takes a SpanSplitCharEnumerable directly?
            return csvParser.Parse(lines);
        }
    }
}

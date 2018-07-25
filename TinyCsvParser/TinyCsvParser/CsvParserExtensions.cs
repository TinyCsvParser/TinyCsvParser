// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TinyCsvParser.Mapping;

namespace TinyCsvParser
{
    public static class CsvParserExtensions
    {
        public static IEnumerable<CsvMappingResult<TEntity>> ReadFromFile<TEntity>(this CsvParser<TEntity> csvParser, string fileName, Encoding encoding)
            where TEntity : new()
        {
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentException(nameof(fileName));

            var lines = File.ReadLines(fileName, encoding);

            return csvParser.Parse(lines);
        }

        public static CsvMappingEnumerable<TEntity> ReadFromString<TEntity>(this CsvParser<TEntity> csvParser, CsvReaderOptions csvReaderOptions, string csvData)
            where TEntity : new()
        {
            return ReadFromSpan(csvParser, csvReaderOptions, csvData.AsSpan());
        }

        public static CsvMappingEnumerable<TEntity> ReadFromSpan<TEntity>(this CsvParser<TEntity> csvParser, CsvReaderOptions csvReaderOptions, ReadOnlySpan<char> csvData)
            where TEntity : new()
        {
            var parts = csvData.Split(csvReaderOptions.NewLine);
            return csvParser.Parse(parts);
        }
    }
}

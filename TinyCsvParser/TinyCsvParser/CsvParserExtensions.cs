// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TinyCsvParser.Mapping;

namespace TinyCsvParser
{
    public static class CsvParserExtensions
    {
        public static ParallelQuery<CsvMappingResult<TEntity>> ReadFromFile<TEntity>(this CsvParser<TEntity> csvParser, string fileName, System.Text.Encoding encoding)
            where TEntity : class, new()
        {
            var lines = File.ReadLines(fileName, encoding);

            return csvParser.Parse(lines);
        }

        public static ParallelQuery<CsvMappingResult<TEntity>> ReadFromString<TEntity>(this CsvParser<TEntity> csvParser, CsvReaderOptions csvReaderOptions, string csvData)
            where TEntity : class, new()
        {
            var lines = csvData.Split(csvReaderOptions.NewLine, StringSplitOptions.None);

            return csvParser.Parse(lines);
        }
    }
}

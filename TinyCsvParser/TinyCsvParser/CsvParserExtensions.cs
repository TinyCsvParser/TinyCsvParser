// Copyright (c) Philipp Wagner and Joel Mueller. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using CoreCsvParser.Mapping;

namespace CoreCsvParser
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
            var parts = csvData.Split(csvReaderOptions.NewLine, StringSplitOptions.RemoveEmptyEntries);
            return csvParser.Parse(parts);
        }

        public static IObservable<TEntity> ObserveFromFile<TEntity>(this CsvParser<TEntity> csvParser, string fileName, Encoding encoding)
            where TEntity : new()
        {
            return Piper.ObserveFile(fileName, encoding, csvParser);
        }

        public static IObservable<TEntity> ObserveFromFile<TEntity>(this CsvParser<TEntity> csvParser, FileInfo file, Encoding encoding)
            where TEntity : new()
        {
            return Piper.ObserveFile(file, encoding, csvParser);
        }

        public static IObservable<TEntity> ObserveFromStream<TEntity>(this CsvParser<TEntity> csvParser, Stream stream, Encoding encoding)
            where TEntity : new()
        {
            return Piper.ObserveStream(stream, encoding, csvParser);
        }
    }
}

// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TinyCsvParser.Mapping;

namespace TinyCsvParser
{
    public class CsvParser<TEntity> : ICsvParser<TEntity> where TEntity : new()
    {
        private readonly CsvParserOptions _options;
        private readonly CsvMapping<TEntity> _mapping;

        public CsvParser(CsvParserOptions options, CsvMapping<TEntity> mapping)
        {
            _options = options;
            _mapping = mapping;
        }

        // TODO: Could we use System.IO.Pipelines to improve the performance of this overload?
        // https://blogs.msdn.microsoft.com/dotnet/2018/07/09/system-io-pipelines-high-performance-io-in-net/
        public IEnumerable<CsvMappingResult<TEntity>> Parse(Stream csvData)
        {
            if (csvData is null)
                throw new ArgumentNullException(nameof(csvData));

            IEnumerable<string> read()
            {
                using (var reader = new StreamReader(csvData))
                {
                    while (!reader.EndOfStream)
                    {
                        yield return reader.ReadLine();
                    }
                }
            }

            return Parse(read());
        }

        public IEnumerable<CsvMappingResult<TEntity>> Parse(IEnumerable<string> csvData)
        {
            if (csvData is null)
                throw new ArgumentNullException(nameof(csvData));

            var query = csvData
                .Select((line, index) => (line, index))
                .Skip(_options.SkipHeader ? 1 : 0);

            if (_options.DegreeOfParallelism > 1)
            {
                var parallelQuery = query.AsParallel();

                // If you want to get the same order as in the CSV file, this option needs to be set:
                if (_options.KeepOrder)
                {
                    parallelQuery = parallelQuery.AsOrdered();
                }

                query = parallelQuery.WithDegreeOfParallelism(_options.DegreeOfParallelism);
            }

            query = query.Where(x => !string.IsNullOrWhiteSpace(x.line));

            // Ignore lines that start with comment character(s):
            if (!string.IsNullOrWhiteSpace(_options.CommentCharacter))
            {
                query = query.Where(x => !x.line.StartsWith(_options.CommentCharacter));
            }

            var tokenizer = _options.Tokenizer;

            return query.Select(x =>
            {
                var tokens = tokenizer.Tokenize(x.line);
                return _mapping.Map(tokens, x.index);
            });
        }

        public CsvMappingEnumerable<TEntity> Parse(SpanSplitEnumerable csvData)
        {
            return new CsvMappingEnumerable<TEntity>(_options, _mapping, csvData);
        }

        public override string ToString()
        {
            return string.Format("CsvParser (Options = {0}, Mapping = {1})", _options, _mapping);
        }
    }
}

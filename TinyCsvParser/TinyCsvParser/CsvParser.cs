// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using TinyCsvParser.Mapping;

namespace TinyCsvParser
{
    public class CsvParser<TEntity> : ICsvParser<TEntity>
        where TEntity : class, new()
    {
        private readonly CsvParserOptions options;
        private readonly CsvMapping<TEntity> mapping;

        public CsvParser(CsvParserOptions options, CsvMapping<TEntity> mapping)
        {
            this.options = options;
            this.mapping = mapping;
        }

        public ParallelQuery<CsvMappingResult<TEntity>> Parse(IEnumerable<string> csvData)
        {
            if (csvData == null)
            {
                throw new ArgumentNullException("csvData");
            }

            var query = csvData
                .Skip(options.SkipHeader ? 1 : 0)
                .AsParallel();

            if (options.KeepOrder)
            {
                query = query.AsOrdered();
            }

            return query
                .WithDegreeOfParallelism(options.DegreeOfParallelism)
                .Where(line => !string.IsNullOrWhiteSpace(line))
                .Select(line => line.Trim().Split(options.FieldsSeparator))
                .Select(fields => mapping.Map(fields));
        }



        public override string ToString()
        {
            return string.Format("CsvParser (Options = {0}, Mapping = {1})", options, mapping);
        }
    }
}

// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using TinyCsvParser.Mapping;
using TinyCsvParser.Model;

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

        public ParallelQuery<CsvMappingResult<TEntity>> Parse(IEnumerable<Row> csvData)
        {
            if (csvData == null)
            {
                throw new ArgumentNullException("csvData");
            }

            var query = csvData
                .Skip(options.SkipHeader ? 1 : 0)
                .AsParallel();

            // If you want to get the same order as in the CSV file, this option needs to be set:
            if (options.KeepOrder)
            {
                query = query.AsOrdered();
            }

            query = query
                .WithDegreeOfParallelism(options.DegreeOfParallelism)
                .Where(row => !row.Data.Span.IsWhiteSpace());

            // Ignore Lines, that start with a comment character:
            if (!string.IsNullOrWhiteSpace(options.CommentCharacter)) 
            {
                query = query.Where(line => !line.Data.Span.StartsWith(options.CommentCharacter));
            }
                
            return query
                .Select(line => new TokenizedRow(line.Index, options.Tokenizer.Tokenize(line.Data.Span)))
                .Select(fields => mapping.Map(fields));
        }

        public override string ToString()
        {
            return string.Format("CsvParser (Options = {0}, Mapping = {1})", options, mapping);
        }
    }
}

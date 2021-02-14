// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using TinyCsvParser.Mapping;
using TinyCsvParser.Model;

namespace TinyCsvParser
{
    public class CsvParser<TEntity> : ICsvParser<TEntity>
    {
        private readonly CsvParserOptions options;
        private readonly ICsvMapping<TEntity> mapping;

        public CsvParser(CsvParserOptions options, ICsvMapping<TEntity> mapping)
        {
            this.options = options;
            this.mapping = mapping;
        }

        public ParallelQuery<CsvMappingResult<TEntity>> Parse(IEnumerable<string> csvData)
        {
            if (csvData == null)
            {
                throw new ArgumentNullException(nameof(csvData));
            }

            var input = csvData
                .Skip(options.SkipHeader ? 1 : 0);

            var rows = options.Tokenizer.Tokenize(input);

            var query = rows.AsParallel();

            // If you want to get the same order as in the CSV file, this option needs to be set:
            if (options.KeepOrder)
            {
                query = query.AsOrdered();
            }

            return query
                .Select(fields => mapping.Map(new TokenizedRow(0, fields)));
        }

        public override string ToString()
        {
            return $"CsvParser (Options = {options}, Mapping = {mapping})";
        }
    }
}

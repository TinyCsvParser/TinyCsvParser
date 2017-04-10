// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
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
        throw new ArgumentNullException(nameof(csvData));
      }

      if (options.StoreRowNumbers)
      {
        var numberedQuery = csvData.Skip(options.SkipHeader ? 1 : 0)
                          .Where(line => !string.IsNullOrWhiteSpace(line))
                          .Select((line, i) => new KeyValuePair<int,string>(i, line))
                          .AsParallel();

        // If you want to get the same order as in the CSV file, this option needs to be set:
        if (options.KeepOrder)
        {
          numberedQuery = numberedQuery.AsOrdered();
        }

        //// Remove empty lines
        //numberedQuery = numberedQuery
        //  .WithDegreeOfParallelism(options.DegreeOfParallelism)
        //  .Where(line => !string.IsNullOrWhiteSpace(line));

        // Ignore Lines, that start with a comment character:
        if (!string.IsNullOrWhiteSpace(options.CommentCharacter))
        {
          numberedQuery = numberedQuery.Where(rowKv => !rowKv.Value.StartsWith(options.CommentCharacter));
        }

        return numberedQuery
          .Select(rowKv => options.Tokenizer.Tokenize(rowKv))
          .Select((token) => mapping.Map(token));
      }

      var query = csvData
          .Skip(options.SkipHeader ? 1 : 0)
          .AsParallel();

      // If you want to get the same order as in the CSV file, this option needs to be set:
      if (options.KeepOrder)
      {
        query = query.AsOrdered();
      }

      // Remove empty lines
      query = query
          .WithDegreeOfParallelism(options.DegreeOfParallelism)
          .Where(line => !string.IsNullOrWhiteSpace(line));

      // Ignore Lines, that start with a comment character:
      if (!string.IsNullOrWhiteSpace(options.CommentCharacter))
      {
        query = query.Where(line => !line.StartsWith(options.CommentCharacter));
      }

      return query
          .Select(line => options.Tokenizer.Tokenize(line))
          .Select(fields => mapping.Map(fields, null));
    }


    public override string ToString()
    {
      return string.Format("CsvParser (Options = {0}, Mapping = {1})", options, mapping);
    }
  }


}

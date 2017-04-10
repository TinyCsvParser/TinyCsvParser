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
    private readonly CsvParserOptions _options;
    private readonly CsvMapping<TEntity> _mapping;

    public CsvParser(CsvParserOptions options, CsvMapping<TEntity> mapping)
    {
      this._options = options;
      this._mapping = mapping;
    }

    public ParallelQuery<CsvMappingResult<TEntity>> Parse(IEnumerable<string> csvData)
    {
      if (csvData == null)
      {
        throw new ArgumentNullException(nameof(csvData));
      }

      if (_options.StoreRowNumbers)
      {
        var numberedQuery = csvData.Skip(_options.SkipHeader ? 1 : 0)
                          .Select((line, i) => new KeyValuePair<int, string>(i, line))
                          .AsParallel();

        // If you want to get the same order as in the CSV file, this option needs to be set:
        if (_options.KeepOrder)
        {
          numberedQuery = numberedQuery.AsOrdered();
        }

        //// Remove empty lines
        numberedQuery = numberedQuery
          .WithDegreeOfParallelism(_options.DegreeOfParallelism)
          .Where(rowKv => !string.IsNullOrWhiteSpace(rowKv.Value));

        // Ignore Lines, that start with a comment character:
        if (!string.IsNullOrWhiteSpace(_options.CommentCharacter))
        {
          numberedQuery = numberedQuery.Where(rowKv => !rowKv.Value.StartsWith(_options.CommentCharacter));
        }

        return numberedQuery
          .Select(rowKv => _options.Tokenizer.Tokenize(rowKv))
          .Select((token) => _mapping.Map(token));
      }

      var query = csvData
          .Skip(_options.SkipHeader ? 1 : 0)
          .AsParallel();

      // If you want to get the same order as in the CSV file, this option needs to be set:
      if (_options.KeepOrder)
      {
        query = query.AsOrdered();
      }

      // Remove empty lines
      query = query
          .WithDegreeOfParallelism(_options.DegreeOfParallelism)
          .Where(line => !string.IsNullOrWhiteSpace(line));

      // Ignore Lines, that start with a comment character:
      if (!string.IsNullOrWhiteSpace(_options.CommentCharacter))
      {
        query = query.Where(line => !line.StartsWith(_options.CommentCharacter));
      }

      return query
          .Select(line => _options.Tokenizer.Tokenize(line))
          .Select(fields => _mapping.Map(fields));
    }


    public override string ToString()
    {
      return string.Format("CsvParser (Options = {0}, Mapping = {1})", _options, _mapping);
    }
  }


}

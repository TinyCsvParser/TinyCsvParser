// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TinyCsvParser.Mapping;
using TinyCsvParser.Model;

namespace TinyCsvParser
{
    public interface ICsvParser<TEntity>
        where TEntity : class, new()
    {
        IAsyncEnumerable<CsvMappingResult<TEntity>> ParseAsync(IAsyncEnumerable<Row> csvData);
    }
}

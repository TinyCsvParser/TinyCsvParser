// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using TinyCsvParser.Mapping;

namespace TinyCsvParser
{
    public interface ICsvParser<TEntity>
        where TEntity : class, new()
    {
        ParallelQuery<CsvMappingResult<TEntity>> Parse(IEnumerable<string> csvData);
    }
}

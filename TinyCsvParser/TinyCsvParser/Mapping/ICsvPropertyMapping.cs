// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace TinyCsvParser.Mapping
{
    public interface ICsvPropertyMapping<TEntity> where TEntity : class, new()
    {
        bool TryMapValue(TEntity entity, ReadOnlyMemory<char> value);
    }
}

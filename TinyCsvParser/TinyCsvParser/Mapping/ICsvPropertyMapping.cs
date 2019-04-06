// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace TinyCsvParser.Mapping
{
    public interface ICsvPropertyMapping<in TEntity>
        where TEntity : class, new()
    {
        bool TryMapValue(TEntity entity, string value);
    }
}

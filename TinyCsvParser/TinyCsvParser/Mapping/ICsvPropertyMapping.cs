// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace TinyCsvParser.Mapping
{
    public interface ICsvPropertyMapping<in TEntity, in TProperty>
    {
        bool TryMapValue(TEntity entity, TProperty value);
    }
}

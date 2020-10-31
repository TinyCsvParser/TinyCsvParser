// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using TinyCsvParser.Model;

namespace TinyCsvParser.Mapping
{
    public class CsvRowMapping<TEntity> : ICsvPropertyMapping<TEntity, TokenizedRow>
    {
        private readonly Func<TEntity, TokenizedRow, bool> action;

        public CsvRowMapping(Func<TEntity, TokenizedRow, bool> action)
        {
            this.action = action;
        }

        public bool TryMapValue(TEntity entity, TokenizedRow value)
        {
            return action(entity, value);
        }
    }
}

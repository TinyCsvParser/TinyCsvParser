using System;
using TinyCsvParser.Model;

namespace TinyCsvParser.Mapping
{
    public class CsvRowMapping<TEntity> : ICsvPropertyMapping<TEntity, TokenizedRow>
    {
        private readonly Action<TEntity, TokenizedRow> action;

        public CsvRowMapping(Action<TEntity, TokenizedRow> action)
        {
            this.action = action;
        }

        public bool TryMapValue(TEntity entity, TokenizedRow value)
        {
            // TODO Better Error Handling is a must!
            try
            {
                action(entity, value);

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}

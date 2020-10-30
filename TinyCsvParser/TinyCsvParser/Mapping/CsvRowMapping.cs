using System;
using TinyCsvParser.Model;
using TinyCsvParser.Tokenizer.RFC4180;

namespace TinyCsvParser.Mapping
{
    public class CsvRowMapping<TEntity> : ICsvPropertyMapping<TEntity, TokenizedRow>
    {
        private readonly Func<TEntity, TokenizedRow, bool> action;

        // This signature was never included in a release, maybe we just blow it away now?
        [Obsolete("Use the constructor that accepts a Func and return true/false to indicate mapping success.", true)]
        public CsvRowMapping(Action<TEntity, TokenizedRow> action)
        {
            this.action = (entity, value) =>
            {
                try
                {
                    action(entity, value);
                    return true;
                }
                catch
                {
                    return false;
                }
            };
        }

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

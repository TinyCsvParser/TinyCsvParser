namespace TinyCsvParser.Mapping
{
	using System;
	using TinyCsvParser.Model;

	public class CsvRowConstructor<TEntity>
	{
		private readonly Func<TokenizedRow, TEntity> action;

		public CsvRowConstructor(Func<TokenizedRow, TEntity> action)
		{
			this.action = action;
		}

		public bool TryMapValue(TokenizedRow value, out TEntity entity)
		{
			entity = action(value);
			return entity != null;
		}
	}
}

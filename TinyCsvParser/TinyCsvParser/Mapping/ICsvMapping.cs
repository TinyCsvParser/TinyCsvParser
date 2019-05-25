using TinyCsvParser.Model;

namespace TinyCsvParser.Mapping
{
    public interface ICsvMapping<TEntity>
    {
        CsvMappingResult<TEntity> Map(TokenizedRow values);
    }
}

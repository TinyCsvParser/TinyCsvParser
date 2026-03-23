using System;
using System.Collections.Generic;
using TinyCsvParser.TypeConverters;

namespace TinyCsvParser.Models;

public delegate bool DynamicConvert(ReadOnlySpan<char> span, out object? result);

public class CsvSchema
{
    private readonly Dictionary<string, DynamicConvert> _columns = new(StringComparer.OrdinalIgnoreCase);

    private readonly ITypeConverterProvider _typeConverterProvider;

    public CsvSchema() : this(new TypeConverterProvider())
    {
    }

    public CsvSchema(ITypeConverterProvider typeConverterProvider)
    {
        _typeConverterProvider = typeConverterProvider;
    }

    public void Add<T>(string columnName)
    {
        var converter = _typeConverterProvider.Resolve<T>();
        Add(columnName, converter);
    }

    public void Add<T>(string columnName, ITypeConverter<T> converter)
    {
        _columns[columnName] = (ReadOnlySpan<char> span, out object? result) =>
        {
            if (converter.TryConvert(span, out T? typedResult))
            {
                result = typedResult;
                return true;
            }
            result = null;
            return false;
        };
    }

    public bool TryGetConverter(string columnName, out DynamicConvert? converter)
    {
        return _columns.TryGetValue(columnName, out converter);
    }
}

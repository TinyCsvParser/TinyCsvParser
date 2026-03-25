using System;
using System.Collections.Generic;
using System.Dynamic;
using TinyCsvParser.Mappings;
using TinyCsvParser.Models;
using TinyCsvParser.TypeConverters;

namespace TinyCsvParser;

public static class CsvParser
{
    public static CsvParser<ExpandoObject> CreateExpandoParser(CsvOptions options, CsvSchema schema)
    {
        return new CsvParser<ExpandoObject>(options, new ExpandoMapping(schema));
    }

    public static CsvParser<ExpandoObject> CreateExpandoParser(CsvOptions options, Action<CsvSchema> configureSchema)
    {
        CsvSchema schema = new();
        configureSchema(schema);
        return new CsvParser<ExpandoObject>(options, new ExpandoMapping(schema));
    }

    public static CsvParser<ExpandoObject> CreateExpandoParser(CsvOptions options, ITypeConverterProvider provider, Action<CsvSchema> configureSchema)
    {
        CsvSchema schema = new(provider);
        configureSchema(schema);
        return new CsvParser<ExpandoObject>(options, new ExpandoMapping(schema));
    }

    public static CsvParser<Dictionary<string, object?>> CreateDictionaryParser(CsvOptions options, CsvSchema schema)
    {
        return new CsvParser<Dictionary<string, object?>>(options, new DictionaryMapping(schema));
    }

    public static CsvParser<Dictionary<string, object?>> CreateDictionaryParser(CsvOptions options, Action<CsvSchema> configureSchema)
    {
        CsvSchema schema = new();
        configureSchema(schema);
        return new CsvParser<Dictionary<string, object?>>(options, new DictionaryMapping(schema));
    }

    public static CsvParser<Dictionary<string, object?>> CreateDictionaryParser(CsvOptions options, ITypeConverterProvider provider, Action<CsvSchema> configureSchema)
    {
        CsvSchema schema = new(provider);
        configureSchema(schema);
        return new CsvParser<Dictionary<string, object?>>(options, new DictionaryMapping(schema));
    }
}
using System;
using System.Collections.Generic;
using TinyCsvParser.Core;
using TinyCsvParser.Models;

namespace TinyCsvParser.Mappings;

public class DictionaryMapping : CsvMapping<Dictionary<string, object?>>, IHeaderBinder
{
    private readonly CsvSchema _schema;
    private string[]? _columnNames;

    public DictionaryMapping(CsvSchema schema)
    {
        _schema = schema;

        MapUsing((Dictionary<string, object?> dict, ref CsvRow row) =>
        {
            if (_columnNames == null) return false;

            for (int i = 0; i < row.Count; i++)
            {
                if (i >= _columnNames.Length) break;

                string colName = _columnNames[i];
                ReadOnlySpan<char> rawValue = row.GetSpan(i);

                if (_schema.TryGetConverter(colName, out DynamicConvert? converter))
                {
                    if (converter!.Invoke(rawValue, out object? result))
                    {
                        dict[colName] = result;
                    }
                    else return false;
                }
                else
                {
                    dict[colName] = row.GetString(i);
                }
            }
            return true;
        });
    }

    public override void BindHeaders(ref CsvRow headerRow)
    {
        _columnNames = new string[headerRow.Count];
        for (int i = 0; i < headerRow.Count; i++)
        {
            _columnNames[i] = headerRow.GetString(i);
        }
    }

    public override bool NeedsHeaderResolution => _columnNames == null;
}
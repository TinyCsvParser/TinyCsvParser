using System;
using System.Collections.Generic;
using System.Dynamic;
using TinyCsvParser.Core;
using TinyCsvParser.Models;

namespace TinyCsvParser.Mappings;

public class ExpandoMapping : CsvMapping<ExpandoObject>, IHeaderBinder
{
    private readonly CsvSchema _schema;

    private string[]? _columnNames;

    public ExpandoMapping(CsvSchema schema)
    {
        _schema = schema;

        MapUsing((ExpandoObject entity, ref CsvRow row) =>
        {
            if (_columnNames == null) return false;

            IDictionary<string, object?> dictionary = entity;

            for (int i = 0; i < row.Count; i++)
            {
                if (i >= _columnNames.Length) break;

                string columnName = _columnNames[i];
                ReadOnlySpan<char> rawValue = row.GetSpan(i);

                if (_schema.TryGetConverter(columnName, out DynamicConvert? converter))
                {
                    if (converter!.Invoke(rawValue, out object? result))
                    {
                        dictionary[columnName] = result;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    // Fallback: Kein Typ im Schema -> String
                    dictionary[columnName] = row.GetString(i);
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

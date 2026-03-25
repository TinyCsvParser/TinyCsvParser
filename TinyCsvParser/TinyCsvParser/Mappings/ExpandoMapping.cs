using System;
using System.Collections.Generic;
using System.Dynamic;
using TinyCsvParser.Models;

namespace TinyCsvParser.Mappings;

/// <summary>
/// Provides an implementation of CsvMapping that maps CSV rows to ExpandoObject instances. Each column in the CSV is 
/// added as a property to the ExpandoObject, with optional type conversion based on the provided CsvSchema.
/// </summary>
public class ExpandoMapping : CsvMapping<ExpandoObject>
{
    private readonly CsvSchema _schema;
    private string[]? _columnNames;

    public ExpandoMapping(CsvSchema schema)
    {
        _schema = schema;

        MapUsing((ExpandoObject entity, ref CsvRow row) =>
        {
            if (_columnNames == null)
            {
                return MapUsingResult.Failure("Headers have not been resolved.");
            }

            IDictionary<string, object?> dictionary = entity;

            for (int i = 0; i < row.Count; i++)
            {
                if (i >= _columnNames.Length)
                {
                    break;
                }

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
                        return MapUsingResult.Failure($"Conversion failed for column '{columnName}' with value '{row.GetString(i)}'.");
                    }
                }
                else
                {
                    dictionary[columnName] = row.GetString(i);
                }
            }

            return MapUsingResult.Success();
        });
    }

    public override CsvHeaderResolution BindHeaders(ref CsvRow headerRow)
    {
        _columnNames = new string[headerRow.Count];
     
        for (int i = 0; i < headerRow.Count; i++)
        {
            _columnNames[i] = headerRow.GetString(i);
        }

        return new CsvHeaderResolution(Array.Empty<int>());
    }

    public override bool NeedsHeaderResolution => _columnNames == null;
}
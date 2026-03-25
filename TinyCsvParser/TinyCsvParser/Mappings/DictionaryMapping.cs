using System;
using System.Collections.Generic;
using TinyCsvParser.Core;
using TinyCsvParser.Models;

namespace TinyCsvParser.Mappings;

/// <summary>
/// Creates a mapping that maps CSV rows to Dictionary<string, object?> instances. Each column in 
/// the CSV is added as a key-value pair to the dictionary, with optional type conversion based on 
/// the provided CsvSchema.
/// </summary>
public class DictionaryMapping : CsvMapping<Dictionary<string, object?>>
{
    private readonly CsvSchema _schema;
    private string[]? _columnNames;

    public DictionaryMapping(CsvSchema schema)
    {
        _schema = schema;

        MapUsing((Dictionary<string, object?> dict, ref CsvRow row) =>
        {
            if (_columnNames == null)
            {
                return MapUsingResult.Failure("Headers have not been resolved.");
            }

            for (int i = 0; i < row.Count; i++)
            {
                if (i >= _columnNames.Length)
                {
                    break;
                }

                string colName = _columnNames[i];

                ReadOnlySpan<char> rawValue = row.GetSpan(i);

                if (_schema.TryGetConverter(colName, out DynamicConvert? converter))
                {
                    if (converter!.Invoke(rawValue, out object? result))
                    {
                        dict[colName] = result;
                    }
                    else
                    {
                        return MapUsingResult.Failure($"Conversion failed for column '{colName}' with value '{row.GetString(i)}'.");
                    }
                }
                else
                {
                    dict[colName] = row.GetString(i);
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
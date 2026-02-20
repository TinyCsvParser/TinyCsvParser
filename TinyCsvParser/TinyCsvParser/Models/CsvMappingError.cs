// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace TinyCsvParser.Models;

/// <summary>
/// Represents an error that occurred during mapping.
/// </summary>
public class CsvMappingError
{
    public int ColumnIndex { get; set; }
    public string Value { get; set; } = string.Empty;
    public override string ToString() => $"Error at Column {ColumnIndex}: {Value}";
}



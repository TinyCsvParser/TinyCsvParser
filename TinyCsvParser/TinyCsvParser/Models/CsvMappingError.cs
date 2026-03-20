// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace TinyCsvParser.Models;

/// <summary>
/// Represents an error that occurred during mapping.
/// </summary>
public readonly record struct CsvMappingError(int RecordIndex, int LineNumber, int ColumnIndex, string Value)
{
    public override string ToString() => $"Error at Column {ColumnIndex}: {Value}";
}
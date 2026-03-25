namespace TinyCsvParser.Models;

/// <summary>
/// A struct representing the position and characteristics of a CSV field within a row. It includes the starting index, 
/// length, whether the field is quoted, and whether it needs unescaping.
/// </summary>
public readonly record struct CsvFieldRange(int Start, int Length, bool IsQuoted, bool NeedsUnescape);
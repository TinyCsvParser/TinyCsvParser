// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace TinyCsvParser.Models;

/// <summary>
/// The Resolved Column Indices after header resolution. This is used internally to optimize mapping when headers are present.
/// </summary>
public readonly record struct CsvHeaderResolution(int[] ResolvedIndices);

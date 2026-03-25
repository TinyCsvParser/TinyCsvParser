// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using TinyCsvParser.Models;

namespace TinyCsvParser.Core;

/// <summary>
/// Extended interface for mappings that require header resolution.
/// </summary>
public interface IHeaderBinder
{
    /// <summary>
    /// A flag indicating whether this mapping requires header resolution. If true, the parser will call 
    /// BindHeaders before mapping rows.
    /// </summary>
    bool NeedsHeaderResolution { get; }

    /// <summary>
    /// Resolves column indices against the actual header row and returns the resolution map.
    /// </summary>
    CsvHeaderResolution BindHeaders(ref CsvRow headerRow);
}
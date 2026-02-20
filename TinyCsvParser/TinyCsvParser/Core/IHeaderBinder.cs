// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using TinyCsvParser.Models;

namespace TinyCsvParser.Core;

/// <summary>
/// Extended interface for mappings that require header resolution.
/// </summary>
public interface IHeaderBinder
{
    bool NeedsHeaderResolution { get; }

    void BindHeaders(ref CsvRow headerRow);
}



// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using TinyCsvParser.Models;

namespace TinyCsvParser.Core;

/// <summary>
/// Used for Custom Mappings that are defined via MapUsing. The delegate takes the entity being mapped 
/// and the current CSV row, and returns a MapUsingResult indicating success or failure 
/// of the mapping.
/// </summary>
public delegate MapUsingResult MapUsingFunc<in TEntity>(TEntity entity, ref CsvRow row);

/// <summary>
/// The core interface for the parser to execute mapping logic.
/// </summary>
public interface ICsvMapping<TEntity> where TEntity : class, new()
{
    /// <summary>
    /// Maps a CSV row to the entity. Optionally takes pre-resolved header indices.
    /// </summary>
    CsvMappingResult<TEntity> Map(ref CsvRow row, CsvHeaderResolution? headerResolution = null);
}

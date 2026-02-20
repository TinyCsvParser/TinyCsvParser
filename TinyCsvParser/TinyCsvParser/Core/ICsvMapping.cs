// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using TinyCsvParser.Models;

namespace TinyCsvParser.Core;

/// <summary>
/// The core interface for the parser to execute mapping logic.
/// </summary>
public interface ICsvMapping<TEntity>
{
    CsvMappingResult<TEntity> Map(ref CsvRow row);
}
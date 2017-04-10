// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace TinyCsvParser.Mapping
{
  public class CsvMappingResult<TEntity>
      where TEntity : class, new()
  {
    public CsvMappingError Error { get; set; }

    public TEntity Result { get; set; }

    public bool IsValid => Error == null;

    public override string ToString()
    {
      return $"CsvMappingResult (Error = {Error}, Result = {Result})";
    }
  }
}

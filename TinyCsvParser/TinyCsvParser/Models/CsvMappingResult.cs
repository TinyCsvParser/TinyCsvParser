// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace TinyCsvParser.Models;

/// <summary>
/// Represents the result of a mapping operation.
/// </summary>
public readonly struct CsvMappingResult<TEntity>
{
    private readonly object? _value;
    private readonly int _index;

    private CsvMappingResult(object? value, int index)
    {
        _value = value;
        _index = index;
    }

    public bool IsSuccess => _index == 0;

    public TEntity Result => _index == 0
        ? (TEntity)_value!
        : throw new InvalidOperationException($"Cannot access 'Result' on a failed mapping. Error: {_value}");

    public CsvMappingError Error => _index == 1
        ? (CsvMappingError)_value!
        : throw new InvalidOperationException("Cannot access 'Error' on a successful mapping.");

    public static implicit operator CsvMappingResult<TEntity>(TEntity success) => new(success, 0);
    public static implicit operator CsvMappingResult<TEntity>(CsvMappingError error) => new(error, 1);

    public TResult Match<TResult>(Func<TEntity, TResult> onSuccess, Func<CsvMappingError, TResult> onFailure)
    {
        return _index == 0 ? onSuccess((TEntity)_value!) : onFailure((CsvMappingError)_value!);
    }
}



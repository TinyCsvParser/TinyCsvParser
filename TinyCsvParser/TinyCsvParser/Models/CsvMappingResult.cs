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
    private readonly int _recordIndex;
    private readonly int _lineNumber;

    public CsvMappingResult(TEntity success, int recordIndex, int lineNumber)
    {
        _value = success;
        _index = 0;
        _recordIndex = recordIndex;
        _lineNumber = lineNumber;
    }

    public CsvMappingResult(CsvMappingError error, int recordIndex, int lineNumber)
    {
        _value = error;
        _index = 1;
        _recordIndex = recordIndex;
        _lineNumber = lineNumber;
    }

    public int RecordIndex => _recordIndex;

    public int LineNumber => _lineNumber;

    public bool IsSuccess => _index == 0;

    public TEntity Result => _index == 0
        ? (TEntity)_value!
        : throw new InvalidOperationException($"Cannot access 'Result' on a failed mapping. Error: {_value}");

    public CsvMappingError Error => _index == 1
        ? (CsvMappingError)_value!
        : throw new InvalidOperationException("Cannot access 'Error' on a successful mapping.");

    public TResult Match<TResult>(Func<TEntity, TResult> onSuccess, Func<CsvMappingError, TResult> onFailure)
    {
        return _index == 0 ? onSuccess((TEntity)_value!) : onFailure((CsvMappingError)_value!);
    }
}



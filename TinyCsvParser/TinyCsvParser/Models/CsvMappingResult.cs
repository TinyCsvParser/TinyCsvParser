// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace TinyCsvParser.Models;

/// <summary>
/// Represents the result of a mapping operation.
/// </summary>
public readonly struct CsvMappingResult<TEntity>
{
    private readonly object? _value;
    private readonly int _state;
    private readonly int _recordIndex;
    private readonly int _lineNumber;

    public CsvMappingResult(TEntity success, int recordIndex, int lineNumber)
    {
        _value = success;
        _state = 0;
        _recordIndex = recordIndex;
        _lineNumber = lineNumber;
    }

    public CsvMappingResult(CsvMappingError error, int recordIndex, int lineNumber)
    {
        _value = error;
        _state = 1;
        _recordIndex = recordIndex;
        _lineNumber = lineNumber;
    }

    public CsvMappingResult(string comment, int recordIndex, int lineNumber)
    {
        _value = comment;
        _state = 2;
        _recordIndex = recordIndex;
        _lineNumber = lineNumber;
    }

    public int RecordIndex => _recordIndex;

    public int LineNumber => _lineNumber;

    public bool IsSuccess => _state == 0;
    public bool IsError => _state == 1;
    public bool IsComment => _state == 2;


    public TEntity Result => _state == 0
        ? (TEntity)_value!
        : throw new InvalidOperationException($"Cannot access 'Result' on a failed mapping. Error: {_value}");

    public CsvMappingError Error => _state == 1
        ? (CsvMappingError)_value!
        : throw new InvalidOperationException("Cannot access 'Error' on a successful mapping.");

    public string Comment => _state == 2
        ? (string)_value!
        : throw new InvalidOperationException("Cannot access 'Comment' on a non-comment mapping.");


    public TResult Match<TResult>(Func<TEntity, TResult> onSuccess, Func<CsvMappingError, TResult> onFailure, Func<string, TResult> onComment)
    {
        return _state switch
        {
            0 => onSuccess((TEntity)_value!),
            1 => onFailure((CsvMappingError)_value!),
            2 => onComment((string)_value!),
            _ => throw new InvalidOperationException("Unknown state")
        };
    }


    public void Switch(Action<TEntity> onSuccess, Action<CsvMappingError> onFailure, Action<string> onComment)
    {
        switch (_state)
        {
            case 0:
                onSuccess((TEntity)_value!);
                break;
            case 1:
                onFailure((CsvMappingError)_value!);
                break;
            case 2:
                onComment((string)_value!);
                break;
        }
    }

    public bool TryGetResult(out TEntity? result)
    {
        if (_state == 0)
        {
            result = (TEntity)_value!;
            return true;
        }
        result = default;
        return false;
    }

    public bool TryGetError(out CsvMappingError? error)
    {
        if (_state == 1)
        {
            error = (CsvMappingError)_value!;
            return true;
        }

        error = default;
        return false;
    }

    public bool TryGetComment(out string? comment)
    {
        if (_state == 2)
        {
            comment = (string)_value!;
            return true;
        }
        
        comment = default;
        return false;
    }
}



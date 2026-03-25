using System;

namespace TinyCsvParser.Models;

public readonly struct MapUsingResult
{
    private readonly string? _errorMessage;
    private readonly int _state;

    private MapUsingResult(int state, string? errorMessage)
    {
        _state = state;
        _errorMessage = errorMessage;
    }

    public static MapUsingResult Success() => new(0, null);
    
    public static MapUsingResult Failure(string errorMessage) => new(1, errorMessage);

    public bool IsSuccess => _state == 0;
    public bool IsError => _state == 1;

    public string ErrorMessage => _state == 1
        ? _errorMessage!
        : throw new InvalidOperationException("Cannot access 'ErrorMessage' on a successful result.");

    public TResult Match<TResult>(Func<TResult> onSuccess, Func<string, TResult> onFailure)
    {
        return _state switch
        {
            0 => onSuccess(),
            1 => onFailure(_errorMessage!),
            _ => throw new InvalidOperationException("Unknown state")
        };
    }

    public void Switch(Action onSuccess, Action<string> onFailure)
    {
        switch (_state)
        {
            case 0:
                onSuccess();
                break;
            case 1:
                onFailure(_errorMessage!);
                break;
        }
    }
}

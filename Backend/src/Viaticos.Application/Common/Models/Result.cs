using Viaticos.Domain.Common;

namespace Viaticos.Application.Common.Models;

public class Result
{
    public bool IsSuccess { get; }
    public string? Error { get; }
    public string? ErrorCode { get; }

    protected Result(bool isSuccess, string? error, string? errorCode)
    {
        IsSuccess = isSuccess;
        Error = error;
        ErrorCode = errorCode;
    }

    public static Result Success() => new(true, null, null);
    public static Result Failure(string code, string message) => new(false, message, code);

    public static Result Failure(DomainException ex) => new(false, ex.Message, ex.Code);
}

public class Result<T> : Result
{
    public T? Value { get; }

    private Result(T value) : base(true, null, null) => Value = value;
    private Result(string code, string message) : base(false, message, code) { }

    public static Result<T> Success(T value) => new(value);
    public static new Result<T> Failure(string code, string message) => new(code, message);
}

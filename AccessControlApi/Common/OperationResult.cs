using FluentValidation.Results;

namespace AccessControlApi.Common;

public class OperationResult<T>
{
    public T Data { get; }
    public bool Success { get; }
    public string? Message { get; }
    public ValidationResult? ValidationResult { get; }
    public int Code { get; }

    public OperationResult(T data, bool success, string? message, ValidationResult? validationResult, int code)
    {
        Data = data;
        Success = success;
        Message = message;
        ValidationResult = validationResult;
        Code = code;
    }
}
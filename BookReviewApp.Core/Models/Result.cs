using BookReviewApp.Core.Enums;

namespace BookReviewApp.Core.Models;

public class Result(string message, ResultType resultType)
{
    public string Message { get; init; } = message;
    public ResultType ResultType { get; init; } = resultType;

    public bool Success => ResultType is ResultType.Success;
    public bool Failed => ResultType is ResultType.Failed;

    public static Result CreateSuccessful(string message) => new(message, ResultType.Success);

    public static Result CreateFailed(string message) => new(message, ResultType.Failed);
}

public class Result<T>(string message, ResultType resultType, T? data) where T : class
{
    public string Message { get; init; } = message;
    public ResultType ResultType { get; init; } = resultType;
    public T? Data { get; init; } = data;

    public bool Success => ResultType is ResultType.Success;
    public bool Failed => ResultType is ResultType.Failed;

    public static Result<T> CreateSuccessful(T? data, string message) => new(message, ResultType.Success, data);

    public static Result<T> CreateFailed(T? data, string message) => new(message, ResultType.Failed, data);
    public static Result<T> CreateFailed(string message) => new(message, ResultType.Failed, null);
}
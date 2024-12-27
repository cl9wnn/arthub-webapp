namespace WebAPI;

public class Result<T>(bool isSuccess, T data, string? errorMessage, int statusCode)
{
    public bool IsSuccess { get; set; } = isSuccess;
    public string? ErrorMessage { get; set; } = errorMessage;
    public int StatusCode { get; set; } = statusCode;
    public T Data { get; set; } = data;
    
    public static Result<T> Success(T data) => new (true, data, null, 200);
    public static Result<T?> Failure(int statusCode, string error) =>
        new (false, default, error, statusCode);
}
namespace Shared;
public class Result<T>
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }

    public Result()
    {
        
    }
    private Result(bool isSuccess, string message, T? data = default)
    {
        IsSuccess = isSuccess;
        Message = message;
        Data = data;
    }

    public static Result<T> Success(T data, string message = "عملیات با موفقیت انجام شد.")
        => new(true, message, data);
  public static Result<T> Failure(T data,string message)
        => new(false, message,data);
  public static Result<T> Failure(string message)
        => new(false, message);
}


public class Result
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;

    private Result(bool success, string message)
    {
        IsSuccess = success;
        Message = message;
    }

    public static Result Success(string message = "عملیات با موفقیت انجام شد.")
        => new(true, message);

    public static Result Failure(string message)
        => new(false, message);
}


public enum PaymentStatus
{
    Pending = 0,
    Success = 1,
    Failed = 2,
    Expired = 3
}
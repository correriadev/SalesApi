namespace SalesApi.ViewModel.V1.Common;

public class ApiResponse<T>
{
    public T Data { get; set; }
    public string Status { get; set; }
    public string Message { get; set; }

    public static ApiResponse<T> Success(T data, string message = "Operation completed successfully")
    {
        return new ApiResponse<T>
        {
            Data = data,
            Status = "success",
            Message = message
        };
    }

    public static ApiResponse<T> Error(string message, T? data = default)
    {
        return new ApiResponse<T>
        {
            Data = data,
            Status = "error",
            Message = message
        };
    }
} 
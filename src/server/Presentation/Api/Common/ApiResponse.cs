namespace CampaignSaaS.Api.Common;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public object? Error { get; set; }
    public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;

    public static ApiResponse<T> Ok(T data) => new() { Success = true, Data = data, Error = null };
    public static ApiResponse<T> Fail(object error) => new() { Success = false, Data = default, Error = error };
}

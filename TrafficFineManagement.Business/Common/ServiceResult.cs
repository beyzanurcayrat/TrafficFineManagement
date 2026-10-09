namespace TrafficFineManagement.Business.Common;

public class ServiceResult
{
    public bool Success { get; }
    public string? ErrorMessage { get; }

    protected ServiceResult(bool success, string? errorMessage)
    {
        Success = success;
        ErrorMessage = errorMessage;
    }

    public static ServiceResult Ok() => new(true, null);
    public static ServiceResult Fail(string message) => new(false, message);
}
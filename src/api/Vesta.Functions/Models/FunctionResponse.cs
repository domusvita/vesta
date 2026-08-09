namespace Vesta.Functions.Models;

public class FunctionResponse<T> where T : class
{
    public FunctionResponse()
    {
    }

    public FunctionResponse(bool success, T? data = null, string? message = null)
    {
        Success = success;
        Data = data;
        Message = message;
    }

    public bool Success { get; set; }

    public T? Data { get; set; } = null;

    public string? Message { get; set; } = null;
}
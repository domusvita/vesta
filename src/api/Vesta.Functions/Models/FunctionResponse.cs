namespace Vesta.Functions.Models;

public class FunctionResponse<T> where T : class
{
    public FunctionResponse()
    {
    }

    public FunctionResponse(bool success, T? data = null)
    {
        Success = success;
        Data = data;
    }

    public bool Success { get; set; }

    public T? Data { get; set; } = null;
}
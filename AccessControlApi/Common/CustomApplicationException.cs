namespace AccessControlApi.Common;

public sealed class CustomApplicationException : Exception
{
    public int StatusCode { get; }
    public CustomApplicationException(string message, int statusCode) : base(message)
    {
        StatusCode=statusCode;
    }
}
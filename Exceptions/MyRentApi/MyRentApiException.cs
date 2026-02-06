using System.Net;

namespace MyRent.Exceptions.MyRentApi;

public sealed class MyRentApiException(HttpStatusCode statusCode, string message) : Exception(message)
{
    public HttpStatusCode StatusCode { get; } = statusCode;
}
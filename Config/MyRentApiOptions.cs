namespace MyRent.Config;

public sealed class MyRentApiOptions
{
    public const string SectionName = "MyRentApi";

    public string BaseUrl { get; init; } = string.Empty;
    public string Guid { get; init; } = string.Empty;
    public string Token { get; init; } = string.Empty;
}
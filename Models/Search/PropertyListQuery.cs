namespace MyRent.Models.Search;

public sealed class PropertyListQuery
{
    public string? SearchTerm { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 12;
    public string? SearchCity { get; init; }
    public string? SearchCountry { get; init; }
}
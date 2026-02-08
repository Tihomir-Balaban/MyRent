namespace MyRent.Models.ViewModels;

public sealed class PropertyListPageVm
{
    public IReadOnlyList<PropertyListItemVm> Items { get; init; } = Array.Empty<PropertyListItemVm>();

    public string? Search { get; init; }
    public int Page { get; init; }
    public int PageSize { get; init; }

    public int TotalCount { get; init; }
    public int TotalPages { get; init; }
    public string? Message { get; init; } = null;
}
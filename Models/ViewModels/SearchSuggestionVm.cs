namespace MyRent.Models.ViewModels;

public sealed class SearchSuggestionVm
{
    public string Kind { get; init; } = default!;
    public string Value { get; init; } = default!;
    public string Title { get; init; } = default!;
    public string? Subtitle { get; init; }
    public int? Stars { get; init; }     
}
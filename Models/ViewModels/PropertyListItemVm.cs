namespace MyRent.Models.ViewModels;

public sealed class PropertyListItemVm
{
    public string IdHash { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;

    public string? PictureMainUrl { get; init; }
    public string? ObjectType { get; init; }

    public string? CityName { get; init; }
    public string? Country { get; init; }

    public int CanSleepOptimal { get; init; }
    public int CanSleepMax { get; init; }

    public int?ClassificationStar { get; init; }

    public bool InstantBooking { get; init; }
    public bool PayCard { get; init; }
    public bool PayIban { get; init; }
    public bool PayCash { get; init; }
    
    public bool IsPetsAllowed { get; init; }
}
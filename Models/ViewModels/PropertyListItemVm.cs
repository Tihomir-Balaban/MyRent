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

    public int? ClassificationStar { get; init; }

    public string? InstantBooking { get; init; }
    public string? PayCard { get; init; }
    public string? PayIban { get; init; }
    public string? PayCash { get; init; }
}
using MyRent.Models.Dtos;

namespace MyRent.Models.ViewModels;

public sealed class PropertyDetailsPageVm
{
    public IReadOnlyList<PropertyDetails> Details { get; init; } = Array.Empty<PropertyDetails>();
    public IReadOnlyList<string> PictureLinks { get; init; } = Array.Empty<string>();
}
using MyRent.Models.Dtos;

namespace MyRent.Models.ViewModels;

public sealed class PropertyDetailsPageVm
{
    public PropertyItem? Property { get; init; }
    public IReadOnlyList<string> PictureLinks { get; init; } = Array.Empty<string>();
}
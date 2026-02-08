using MyRent.Models.Dtos;

namespace MyRent.Interfaces.Services;

public interface IMyRentClientService
{
    Task<IReadOnlyList<PropertyItem>?> GetPropertiesAsync(CancellationToken cancellationToken);
    Task<PropertyItem> GetPropertyDetailsAsync(string idHash, CancellationToken cancellationToken);
    Task<IReadOnlyList<PropertyPicture>?> GetPicturesAsync(string idHash, CancellationToken cancellationToken);
}
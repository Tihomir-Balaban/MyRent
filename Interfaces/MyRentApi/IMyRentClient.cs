using MyRent.Exceptions.MyRentApi;
using MyRent.Models.Dtos;

namespace MyRent.Interfaces.MyRentApi;

public interface IMyRentClient
{
    Task<IReadOnlyList<PropertyListItem>?> GetPropertiesAsync(CancellationToken cancellationToken);
    Task<IReadOnlyList<PropertyDetails>> GetPropertyDetailsAsync(string idHash, CancellationToken cancellationToken);
    Task<IReadOnlyList<PropertyPicture>?> GetPicturesAsync(string idHash, CancellationToken cancellationToken);
}
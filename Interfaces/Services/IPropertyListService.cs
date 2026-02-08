using MyRent.Models.Search;
using MyRent.Models.ViewModels;

namespace MyRent.Interfaces.Services;

public interface IPropertyListService
{
    Task<PagedResult<PropertyListItemVm>> GetAsync(PropertyListQuery query, CancellationToken cancellationToken);
}
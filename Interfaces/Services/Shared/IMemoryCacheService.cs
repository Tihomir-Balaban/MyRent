using MyRent.Models.ViewModels;

namespace MyRent.Interfaces.Services.Shared;

public interface IMemoryCacheService
{
    Task<IReadOnlyList<PropertyListItemVm>> GetCachedListAsync(string cacheKey, CancellationToken cancellationToken);
}
using Microsoft.Extensions.Caching.Memory;
using MyRent.Interfaces.Services;
using MyRent.Interfaces.Services.Shared;
using MyRent.Models.ViewModels;

namespace MyRent.Services.Shared;

public sealed class MemoryCacheService(IMyRentClientService clientService, IMemoryCache cache) : IMemoryCacheService
{
    private static readonly TimeSpan CacheTtl = TimeSpan.FromMinutes(3);

    /// <summary>
    /// Retrieves a cached list of property items from memory cache. If data is not found in the cache,
    /// fetches it from the client service, maps it to the required format, and caches the result.
    /// </summary>
    /// <param name="cacheKey">The unique key used to retrieve or store the cached data.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A read-only list of property items. Returns an empty list if no properties are available.</returns>
    public async Task<IReadOnlyList<PropertyListItemVm>> GetCachedListAsync(string cacheKey, CancellationToken cancellationToken)
    {
        if (cache.TryGetValue(cacheKey, out IReadOnlyList<PropertyListItemVm>? cached) && cached is not null)
            return cached;

        var data = await clientService.GetPropertiesAsync(cancellationToken);

        var mapped = data?
            .Select(x => new PropertyListItemVm
            {
                IdHash = x.IdHash,
                Name = x.Name,
                PictureMainUrl = x.PictureMainUrl,
                ObjectType = x.ObjectType,
                CityName = x.CityName,
                Country = x.Country,
                CanSleepOptimal = x.CanSleepOptimal,
                CanSleepMax = x.CanSleepMax,
                ClassificationStar = x.ClassificationStar,
                InstantBooking = x.IsInstantBooking,
                PayCard = x.CanPayCard,
                PayIban = x.CanPayIban,
                PayCash = x.CanPayCash,
                IsPetsAllowed = x.IsPetsAllowed
            })
            .ToList() ?? new List<PropertyListItemVm>();
            
        if (mapped.Count == 0) 
            return Array.Empty<PropertyListItemVm>();
            
        cache.Set(cacheKey, mapped, CacheTtl);

        return mapped;
    }
}
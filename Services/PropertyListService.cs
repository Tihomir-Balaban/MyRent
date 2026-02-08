using Microsoft.Extensions.Caching.Memory;
using MyRent.Interfaces.Services;
using MyRent.Interfaces.Services.Shared;
using MyRent.Models.Search;
using MyRent.Models.ViewModels;

namespace MyRent.Services;

public sealed class PropertyListService(IMyRentClientService clientService, IMemoryCacheService cache) : IPropertyListService
{
        private const string CacheKey = "myrent:properties:list";

        /// Retrieves a paginated list of property listings based on the provided query parameters.
        /// <param name="query">
        /// An instance of <see cref="PropertyListQuery"/> containing filtering and pagination options.
        /// </param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> to observe while waiting for the task to complete.
        /// </param>
        /// <return>
        /// A task that represents the asynchronous operation. The task result contains a
        /// <see cref="PagedResult{PropertyListItemVm}"/> object, which includes the filtered and paginated
        /// property listings, total count, current page, page size, and total pages.
        /// </return>
        public async Task<PagedResult<PropertyListItemVm>> GetAsync(PropertyListQuery query, CancellationToken cancellationToken)
        {
            var page = query.Page < 1 ? 1 : query.Page;
            var pageSize = query.PageSize is < 6 or > 24 ? 3 : query.PageSize;

            var all = await cache.GetCachedListAsync(CacheKey, cancellationToken);

            if (all.Count == 0)
            {
                return new PagedResult<PropertyListItemVm>
                {
                    Items = Array.Empty<PropertyListItemVm>(),
                    TotalCount = 0,
                    Page = 1,
                    PageSize = 3,
                    TotalPages = 1,
                    Message = "No properties found."
                };
            }
            
            IEnumerable<PropertyListItemVm> filtered = all;

            if (!string.IsNullOrWhiteSpace(query.SearchTerm))
            {
                var s = query.SearchTerm.Trim();

                filtered = filtered.Where(x =>
                    query.SearchCity != null
                        ? x.CityName.Equals(query.SearchCity, StringComparison.OrdinalIgnoreCase)
                        : query.SearchCountry != null
                            ? x.Country.Equals(query.SearchCountry, StringComparison.OrdinalIgnoreCase)
                            : query.SearchTerm != null
                                ? (
                                    x.Name.Contains(query.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                                    x.CityName.Contains(query.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                                    x.Country.Contains(query.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                                    x.ObjectType.Contains(query.SearchTerm, StringComparison.OrdinalIgnoreCase)
                                )
                                : true
                );
            }

            var totalCount = filtered.Count();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            if (totalPages == 0) totalPages = 1;
            if (page > totalPages) page = totalPages;

            var items = filtered
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PagedResult<PropertyListItemVm>
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = totalPages
            };
        }
    }
using Microsoft.AspNetCore.Mvc;
using MyRent.Interfaces.Services.Shared;
using MyRent.Models.ViewModels;

namespace MyRent.Controllers;

[Route("/search")]
public sealed class SearchController(IMemoryCacheService memoryCacheService) : Controller
{
    private const string CacheKey = "myrent:search:query:list";
    
    // GET /search?q=...
    [HttpGet("")]
    public async Task<IActionResult> SearchSuggestions(string q, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(q) || q.Length < 2)
            return Json(Array.Empty<SearchSuggestionVm>());

        var cacheKey = string.Concat(CacheKey, ":", q);
        var list = await memoryCacheService.GetCachedListAsync(cacheKey ,cancellationToken);

        var filtered = list
            .Where(x =>
                x.Name.Contains(q, StringComparison.OrdinalIgnoreCase) ||
                x.CityName.Contains(q, StringComparison.OrdinalIgnoreCase) ||
                x.Country.Contains(q, StringComparison.OrdinalIgnoreCase) ||
                x.ObjectType.Contains(q, StringComparison.OrdinalIgnoreCase))
            .ToList();
        
        var citySuggestions = filtered
            .Select(x => x.CityName)
            .Where(s => !string.IsNullOrWhiteSpace(s) && s.Contains(q, StringComparison.OrdinalIgnoreCase))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Take(3)
            .Select(city => new SearchSuggestionVm
            {
                Kind = "city",
                Value =  $"city:{city}",
                Title = city!,
                Subtitle = "City"
            })
            .OrderBy(x => x.Title);

        var countrySuggestions = filtered
            .Select(x => x.Country)
            .Where(s => !string.IsNullOrWhiteSpace(s) && s.Contains(q, StringComparison.OrdinalIgnoreCase))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Take(3)
            .Select(country => new SearchSuggestionVm
            {
                Kind = "country",
                Value =  $"country:{country}",
                Title = country!,
                Subtitle = "Country"
            })
            .OrderBy(x => x.Title);

        var propertySuggestions = filtered
            .Select(x => new SearchSuggestionVm
            {
                Kind = "property",
                Value = x.Name,
                Title = x.Name,
                Subtitle = $"{x.CityName}, {x.Country}",
                Stars = x.ClassificationStar ?? 0
            })
            .DistinctBy(x => x.Title)
            .Take(8)
            .OrderBy(x => x.Title);

        var combined = citySuggestions
            .Concat(countrySuggestions)
            .Concat(propertySuggestions)
            .Take(10)
            .ToList();

        return Json(combined);
    }
}
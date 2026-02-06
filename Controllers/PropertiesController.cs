using Microsoft.AspNetCore.Mvc;
using MyRent.Interfaces.MyRentApi;
using MyRent.Models.ViewModels;

namespace MyRent.Controllers;

public class PropertiesController(IMyRentClient client) : Controller
{
    // GET /properties?search=&page=&pageSize=
    [HttpGet("")]
    public async Task<IActionResult> Index(
        [FromQuery] string? search,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 12,
        CancellationToken cancellationToken = default)
    {
        // Step 8 will replace this with PropertyListService (server-side-ready paging/search).
        // For now: fetch all and do simple in-memory filtering + paging.
        var all = await client.GetPropertiesAsync(cancellationToken);

        var filtered = all.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim();
            filtered = filtered.Where(x =>
                (x.Name?.Contains(s, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (x.CityName?.Contains(s, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (x.Country?.Contains(s, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (x.ObjectType?.Contains(s, StringComparison.OrdinalIgnoreCase) ?? false));
        }

        var totalCount = filtered.Count();
        page = page < 1 ? 1 : page;
        pageSize = pageSize is < 6 or > 48 ? 12 : pageSize;

        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        if (totalPages == 0) totalPages = 1;
        if (page > totalPages) page = totalPages;

        var items = filtered
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
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
                InstantBooking = x.InstantBooking,
                PayCard = x.PayCard,
                PayIban = x.PayIban,
                PayCash = x.PayCash
            })
            .ToList();

        var vm = new PropertyListPageVm
        {
            Items = items,
            Search = search,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = totalPages
        };

        return View(vm);
    }

    // GET /properties/{idHash}
    [HttpGet("{idHash}")]
    public async Task<IActionResult> Details([FromRoute] string idHash, CancellationToken cancellationToken)
    {
        // Parallel fetch
        var detailsTask = client.GetPropertyDetailsAsync(idHash, cancellationToken);
        var picturesTask = client.GetPicturesAsync(idHash, cancellationToken);

        try
        {
            await Task.WhenAll(detailsTask, picturesTask);
        }
        catch
        {
            // let MVC error page show for now; later we can render a friendly error view
            throw;
        }

        var vm = new PropertyDetailsPageVm
        {
            Details = detailsTask.Result,
            PictureLinks = picturesTask.Result?.Select(p => p.PictureLink).ToList()
        };

        return View(vm);
    }
}
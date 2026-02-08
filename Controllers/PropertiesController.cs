using Microsoft.AspNetCore.Mvc;
using MyRent.Exceptions.MyRentApi;
using MyRent.Interfaces.Services;
using MyRent.Models.Dtos;
using MyRent.Models.Search;
using MyRent.Models.ViewModels;
using MyRent.Models.ViewModels.Shared;

namespace MyRent.Controllers;

[Route("/properties")]
public sealed class PropertiesController(IMyRentClientService clientService, IPropertyListService listService) : Controller
{
    // GET /properties or GET /properties?search=...
    [HttpGet("")]
    public async Task<IActionResult> Index(
        [FromQuery] string? search,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 3,
        CancellationToken cancellationToken = default)
    {
        try
        {
            string? searchCity = null;
            string? searchCountry = null;
            string? searchText = null;

            if (!string.IsNullOrWhiteSpace(search))
            {
                if (search.StartsWith("city:", StringComparison.OrdinalIgnoreCase))
                {
                    searchCity = search["city:".Length..].Trim();
                }
                else if (search.StartsWith("country:", StringComparison.OrdinalIgnoreCase))
                {
                    searchCountry = search["country:".Length..].Trim();
                }
                else
                {
                    searchText = search.Trim();
                }
            }
            
            var result = await listService.GetAsync(new PropertyListQuery
            {
                SearchTerm = searchText,
                Page = page,
                PageSize = pageSize,
                SearchCity = searchCity,
                SearchCountry = searchCountry,
            }, cancellationToken);

            var vm = new PropertyListPageVm
            {
                Items = result.Items,
                Search = search,
                Page = result.Page,
                PageSize = result.PageSize,
                TotalCount = result.TotalCount,
                TotalPages = result.TotalPages,
                Message = result.Message
            };

            return View(vm);
        }
        catch (MyRentApiException ex)
        {
            return View("~/Views/Shared/ErrorFriendly.cshtml", new ErrorVm
            {
                Title = "MyRent API error",
                Message = "Failed to load properties from the external API.",
                StatusCode = (int)ex.StatusCode
            });
        }
    }

    // GET /properties/{idHash}
    [HttpGet("{idHash}")]
    public async Task<IActionResult> Details([FromRoute] string idHash, CancellationToken cancellationToken)
    {
        try
        {
            var detailsTask = clientService.GetPropertyDetailsAsync(idHash, cancellationToken);
            var picturesTask = clientService.GetPicturesAsync(idHash, cancellationToken);

            await Task.WhenAll(detailsTask, picturesTask);

            var vm = new PropertyDetailsPageVm
            {
                Property = detailsTask.Result,
                PictureLinks = picturesTask.Result?.Select(p => p.PictureLink).ToList() ?? new List<string>()
            };
            
            // NOTE:
            // Due to the fact that the api doesn't have a property with more than 12 pictures, if
            // you want to mock this comment the PropertyDetailsPageVm reference type instantiation and
            // uncomment the following commented lines.

            // List<string> longerPictureList = new List<string>();
            //
            // for (int i = 0; i < 3; i++)
            //     longerPictureList.AddRange(picturesTask.Result?.Select(p => p.PictureLink).ToList());
            //
            // var vm = new PropertyDetailsPageVm
            // {
            //     Property = detailsTask.Result,
            //     PictureLinks = longerPictureList
            // };

            return View(vm);
        }
        catch (MyRentApiException ex)
        {
            return View("~/Views/Shared/ErrorFriendly.cshtml", new ErrorVm
            {
                Title = "MyRent API error",
                Message = "Failed to load property details from the external API.",
                StatusCode = (int)ex.StatusCode
            });
        }
    }
}
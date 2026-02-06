using Microsoft.AspNetCore.Mvc;
using MyRent.Interfaces.MyRentApi;

namespace MyRent.Controllers;

[Route("/myrent")]
public sealed class MyRentController(IMyRentClient client) : Controller
{
        // private readonly IMyRentClient _client = client;
        
        // GET /debug/myrent/list
        [HttpGet("list")]
        public async Task<IActionResult> List(CancellationToken cancellationToken)
        {
            var data = await client.GetPropertiesAsync(cancellationToken);
            return Json(data);
        }

        // GET /debug/myrent/details?idHash=...
        [HttpGet("details")]
        public async Task<IActionResult> Details([FromQuery] string idHash, CancellationToken cancellationToken)
        {
            var data = await client.GetPropertyDetailsAsync(idHash, cancellationToken);
            return Json(data);
        }

        // GET /debug/myrent/pictures?idHash=...
        [HttpGet("pictures")]
        public async Task<IActionResult> Pictures([FromQuery] string idHash, CancellationToken cancellationToken)
        {
            var data = await client.GetPicturesAsync(idHash, cancellationToken);
            return Json(data);
        }
}
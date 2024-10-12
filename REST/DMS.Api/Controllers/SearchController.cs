using Microsoft.AspNetCore.Mvc;
using DMS.REST.Api.Controllers;
using MediatR;

[ApiController]
[Route("/api/[controller]")]
public class SearchController(IMediator mediator) : BaseController(mediator)
{
    private ILogger<SearchController> _logger;

    // [HttpGet]
    // public async Task<ActionResult<IResponse>> Search([FromBody] SearchQuery searchQuery)
    // {
    //     mediator.Send(searchQuery);
    //     return await ResponseAsync(searchQuery);
    // }
}

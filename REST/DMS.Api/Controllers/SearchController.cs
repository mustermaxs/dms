using Microsoft.AspNetCore.Mvc;
using DMS.REST.Api.Controllers;
using MediatR;

[ApiController]
[Route("/api/[controller]")]
public class SearchController(IMediator mediator, ILogger<SearchController> logger) : BaseController(mediator, logger)
{
    // [HttpGet]
    // public async Task<ActionResult<IResponse>> Search([FromBody] SearchQuery searchQuery)
    // {
    //     mediator.Send(searchQuery);
    //     return await ResponseAsync(searchQuery);
    // }
}

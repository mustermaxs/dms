using DMS.Application.Commands;
using Microsoft.AspNetCore.Mvc;
using DMS.REST.Api.Controllers;
using MediatR;
using Microsoft.Extensions.Logging;

[ApiController]
[Route("api/[controller]")]
public class SearchController : BaseController
{
    private readonly IMediator _mediator;
    private readonly ILogger<SearchController> _logger;

    public SearchController(IMediator mediator, ILogger<SearchController> logger) : base(mediator)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet("tags")]
    public async Task<ActionResult> SearchByTag([FromQuery] string prefix)
    {
        var res = await _mediator.Send(new GetTagsQuery()); 
        return Ok(res);
    }

    [HttpGet("documents")] 
    public async Task<ActionResult> SearchByDocumentTitle([FromQuery] string title) 
    {
        var res = await _mediator.Send(new GetDocumentsQuery()); 
        return Ok(res);
    }
    
    [HttpGet("documents/content")] 
    public async Task<ActionResult> SearchByDocumentContent([FromQuery] string content)
    {
        var res = await _mediator.Send(new GetDocumentsQuery()); 
        return Ok(res);
    }
}
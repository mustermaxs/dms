using DMS.Application.DTOs;
using DMS.Domain.Entities;

namespace DMS.REST.Api.Controllers;

using Microsoft.AspNetCore.Mvc;
using DMS.REST.Api.Controllers;
using MediatR;
using DMS.Application.Commands;

[ApiController]
[Route("/api/[controller]")]
public class TagsController : BaseController
{
    public TagsController(IMediator mediator) : base(mediator)
    {
    }

    [HttpGet]
    public async Task<ActionResult<List<DocumentTag>>> GetTags()
    {
        var res = await _mediator.Send(new GetTagsQuery());
        return Ok(res);
    }

    [HttpGet("/search")]
    public async Task<ActionResult<DocumentTag>> FindTagByPrefix([FromQuery] string tagPrefix)
    {
        var res = await _mediator.Send(new FindTagsByPrefixQuery(tagPrefix));
        return Ok(res);
    }

    [HttpPost]
    public async Task<ActionResult<DocumentTag>> CreateTag([FromBody] CreateTagDto createTagDto)
    {
        var res = await _mediator.Send(new CreateTagCommand(createTagDto.Label, createTagDto.Value));
        return Ok(res);
    }
}
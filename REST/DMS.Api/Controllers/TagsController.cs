using DMS.Api.Configuration;
using DMS.Application.DTOs;
using DMS.Domain.Entities;
using DMS.Domain.Entities.Tag;

namespace DMS.REST.Api.Controllers;

using Microsoft.AspNetCore.Mvc;
using DMS.REST.Api.Controllers;
using MediatR;
using DMS.Application.Commands;

[ApiController]
[Route("/api/[controller]")]
public class TagsController : BaseController
{
    public TagsController(IMediator mediator, ILogger<TagsController> logger) : base(mediator, logger)
    {
    }

    [HttpGet]
    public async Task<ActionResult<List<TagDto>>> GetTags()
    {
        return await ApiResponse<GetTagsQuery, List<TagDto>>(
            new GetTagsQuery(),
            onSuccess: data => Ok(
                new Response<List<TagDto>>
                {
                    Success = true,
                    Data = data,
                    Message = "Successfully retrieved tags"
                }),
            onFailure: () => BadRequest(
                new Response
                {
                    Message = "Failed to retrieve tags",
                    Success = false
                })
        );
    }

    [HttpGet("/search")]
    public async Task<ActionResult<TagDto>> FindTagByPrefix([FromQuery] string tagPrefix)
    {
        var res = await _mediator.Send(new FindTagsByPrefixQuery(tagPrefix));
        return Ok(res);
    }

    [HttpPost]
    public async Task<ActionResult<TagDto>> CreateTag([FromBody] CreateTagDto createTagDto)
    {
        var res = await _mediator.Send(new CreateTagCommand(createTagDto.Label, createTagDto.Value));
        return Ok(res);
    }
}
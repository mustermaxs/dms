using DMS.Api.Configuration;
using DMS.Application.DTOs;
using DMS.Application.Queries;
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
                    Content = data,
                    Message = data.Count > 0 ? 
                        "Successfully retrieved tags"
                        : "No tags found"
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

    [HttpDelete]
    public async Task<ActionResult> DeleteAllTags()
    {
        return await ApiResponse<DeleteAllTagsCommand>(
            new DeleteAllTagsCommand(),
            onSuccess: () => Ok(
                new Response
                {
                    Message = "Successfully deleted all tags",
                    Success = true
                }),
            onFailure: () => BadRequest(
                new Response
                {
                    Message = "Failed to delete all tags",
                    Success = false
                }));
    }
}
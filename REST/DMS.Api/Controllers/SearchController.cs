using Microsoft.AspNetCore.Mvc;
using MediatR;
using DMS.Application.DTOs;
using DMS.Application.Commands;
using DMS.Api.Configuration;

namespace DMS.REST.Api.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class SearchController : BaseController
{
    public SearchController(IMediator mediator, ILogger<SearchController> logger) : base(mediator, logger)
    {
    }

    [HttpGet]
    public async Task<ActionResult<List<DocumentSearchResultDto>>> Search([FromQuery] string query)
    {
        return await ApiResponse<SearchDocumentsQuery, List<DocumentSearchResultDto>>(
            new SearchDocumentsQuery(query),
            onSuccess: data => Ok(
                new Response<List<DocumentSearchResultDto>>
                {
                    Success = true,
                    Content = data,
                    Message = "Successfully searched documents"
                }),
            onFailure: () => BadRequest(
                new Response
                {
                    Message = "Failed to search documents",
                    Success = false
                })
        );
    }

}

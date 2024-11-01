using DMS.Api.Configuration;
using DMS.Application.DTOs;
using Microsoft.AspNetCore.Mvc;
using DMS.REST.Api.Controllers;
using MediatR;
using DMS.Application.Commands;
namespace DMS.REST.Api.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class DocumentsController : BaseController
{
    public DocumentsController(IMediator mediator, ILogger<DocumentsController> logger) : base(mediator, logger)
    {
    }

    [HttpPost]
    public async Task<ActionResult<DmsDocumentDto>> UploadDocument([FromBody] UploadDocumentDto documentDto)
    {
        return await ApiResponse<UploadDocumentCommand, DmsDocumentDto>(
            new UploadDocumentCommand(documentDto.Title, documentDto.Content, documentDto.Tags),
            onSuccess: data => Ok(
                new Response<DmsDocumentDto>
                {
                    Success = true,
                    Data = data,
                    Message = "Successfully uploaded document"
                }),
            onFailure: () => BadRequest(
                new Response
                {
                    Message = "Failed to upload document",
                    Success = false
                })
        );
        // try
        // {
        //     var res = await _mediator.Send(new UploadDocumentCommand(documentDto.Title, documentDto.Content, documentDto.Tags));
        //     return Ok(new Response<string>{ Success = true, Message = "Successfully uploaded document" });
        // }
        // catch (Exception e)
        // {
        //     return BadRequest(new Response<string>{Message = "Failed to upload document", Success = false});
        //     throw;
        // }
    }

    [HttpGet]
    public async Task<ActionResult<List<DmsDocumentDto>>> GetDocuments()
    {
        return await ApiResponse<GetDocumentsQuery, List<DmsDocumentDto>>(
            new GetDocumentsQuery(),
            onSuccess: data => Ok(
                new Response<List<DmsDocumentDto>>
            {
                Success = true,
                Data = data,
                Message = "Successfully retrieved documents"
            }),
            onFailure: () => BadRequest( 
                new Response
            {
                Message = "Failed to retrieve documents",
                Success = false
            })
            );
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<DmsDocumentDto>> GetDocument(Guid id)
    {
        return await ApiResponse<GetDocumentQuery, DmsDocumentDto>(
            new GetDocumentQuery(id),
            onSuccess: data => Ok(
                new Response<DmsDocumentDto>
                {
                    Success = true,
                    Data = data,
                    Message = "Successfully retrieved document"
                }),
            onFailure: () => BadRequest(
                new Response
                {
                    Message = "Failed to retrieve document",
                    Success = false
                })
        );
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<DmsDocumentDto>> UpdateDocument([FromBody] UpdateDocumentDto documentDto)
    {
        return await ApiResponse<UpdateDocumentCommand, DmsDocumentDto>(
            command: new UpdateDocumentCommand(documentDto),
            onSuccess: data => Ok(
                new Response<DmsDocumentDto>
                {
                    Success = true,
                    Data = data,
                    Message = "Successfully updated document"
                }),
            onFailure: () => BadRequest(
                new Response
                {
                    Message = "Failed to update document",
                    Success = false
                })
        );
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteDocument(Guid id)
    {
        return await ApiResponse<DeleteDocumentCommand>(
            command: new DeleteDocumentCommand(id),
            onSuccess: () => Ok(
                new Response
                {
                    Success = true,
                    Message = "Successfully deleted document"
                }),
            onFailure: () => BadRequest(
                new Response
                {
                    Message = "Failed to delete document",
                    Success = false
                })
        );
    }

    [HttpDelete]
    public async Task<ActionResult> DeleteAllDocuments()
    {
        return await ApiResponse<DeleteAllDocumentsCommand>(
            command: new DeleteAllDocumentsCommand(),
            onSuccess: () => Ok(
                new Response
                {
                    Success = true,
                    Message = "Successfully deleted all documents"
                }),
            onFailure: () => BadRequest(
                new Response
                {
                    Message = "Failed to delete all documents",
                    Success = false
                })
        );
    }
}
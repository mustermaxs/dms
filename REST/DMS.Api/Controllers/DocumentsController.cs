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
                    Content = data,
                    Message = "Successfully uploaded document"
                }),
            onFailure: () => BadRequest(
                new Response
                {
                    Message = "Failed to upload document",
                    Success = false
                })
        );
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
                Content = data,
                Message = data.Count > 0 ? "Successfully retrieved documents" : "No documents found",
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
                    Content = data,
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
    
    [HttpGet("{id}/content")]
    public async Task<ActionResult<DocumentContentDto>> GetDocumentContent(Guid id)
    {
        return await ApiResponse<GetDocumentContentQuery, DocumentContentDto>(
            new GetDocumentContentQuery(id),
            onSuccess: data => Ok(
                new Response<DocumentContentDto>
                {
                    Success = true,
                    Content = data,
                    Message = string.IsNullOrEmpty(data.Content) ?
                        "Content isn't available" :
                        "Successfully retrieved documents content"
                }),
            onFailure: () => BadRequest(
                new Response
                {
                    Message = "Failed to retrieve document content",
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
                    Content = data,
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
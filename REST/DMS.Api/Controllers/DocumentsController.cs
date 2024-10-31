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
    public DocumentsController(IMediator mediator) : base(mediator)
    {
    }

    [HttpPost]
    public async Task<ActionResult> UploadDocument([FromBody] UploadDocumentDto documentDto)
    {
        try
        {
            return await ResponseAsync(new UploadDocumentCommand(documentDto.Title, documentDto.Content, documentDto.Tags));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    [HttpGet]
    public async Task<ActionResult> GetDocuments()
    {
        return await ResponseAsync(new GetDocumentsQuery());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetDocument(Guid id)
    {
        return await ResponseAsync(new GetDocumentQuery(id));
    }

    [HttpPut]
    public async Task<ActionResult> UpdateDocument([FromBody] UpdateDocumentDto documentDto)
    {
        Console.WriteLine(documentDto);

        return await ResponseAsync(new UpdateDocumentCommand( documentDto));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteDocument(Guid id)
    {
        var res = await _mediator.Send(new DeleteDocumentCommand(id));
        return Ok(res);
    }
}
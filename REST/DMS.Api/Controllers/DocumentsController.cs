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
            // var res = await _mediator.Send();
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
        var res = await _mediator.Send(new GetDocumentsQuery());
        return Ok(res);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetDocument(Guid id)
    {
        var res = await _mediator.Send(new GetDocumentQuery(id));
        return Ok(res);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateDocument(Guid id)
    {
        var res = await _mediator.Send(new UpdateDocumentCommand(id));
        return Ok(res);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteDocument(Guid id)
    {
        var res = await _mediator.Send(new DeleteDocumentCommand(id));
        return Ok(res);
    }
}
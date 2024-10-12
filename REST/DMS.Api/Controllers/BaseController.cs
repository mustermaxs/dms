using System.Text.Json;
using System.Text.Json.Nodes;
using System.Windows.Input;
using Microsoft.AspNetCore.Mvc;
using MediatR;

namespace DMS.REST.Api.Controllers;
public abstract class BaseController : ControllerBase
{
    private readonly IMediator Mediator;

    public BaseController(IMediator mediator)
    {
        Mediator = mediator;
    }

    protected async Task<ActionResult<IResponse>> ResponseAsync(IRequest command)   // TODO refactor
    {
        try
        {
            Console.WriteLine(command.ToString());
            dynamic responseObj = await Mediator.Send(command);
            if (responseObj is null)
            {
                // Logger.Error($"Response for request {command.GetType().FullName} is null. {command.ToString()}");
                return NotFound();
            }

            if (responseObj is JsonObject)
            {
                var jsonRes = JsonSerializer.Serialize(responseObj);
                // Logger.Info($"Response for request {command.GetType().FullName} is {jsonRes}");
                return Content(jsonRes, "application/json");
            }

            // if (command is ExportTourAsJsonCommand) // TODO eigene FileResponse Klasse um file name und file type angeben zu können?
            // {
            //     return File(responseObj as byte[], "application/json", "tourexport.json");
            // }
            
            if (responseObj is byte[] pdfBytes) // TODO eigene FileResponse Klasse um file name und file type angeben zu können?
            {
                return File(pdfBytes, "application/pdf");
            }

            if (responseObj is FileContentResult)
            {
                return responseObj;
            }

            return Ok(responseObj);
        }
        catch (ResourceNotFoundException rex)
        {
            Console.WriteLine(rex);
            // Logger.Error($"Request {command.GetType().FullName} failed: {rex.Message}. {command.ToString()}");
            return BadRequest(rex.Message);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            // Logger.Error($"Request {command.GetType().FullName} failed.  {command.ToString()}. {e.Message}");
            return Problem($"Something went wrong :(");
        }
    }
}
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Mvc;
using MediatR;

namespace DMS.REST.Api.Controllers
{
    public abstract class BaseController : ControllerBase
    {
        protected readonly IMediator _mediator;

        public BaseController(IMediator mediator)
        {
            _mediator = mediator;
        }
        protected async Task<ActionResult<dynamic>> ResponseAsync<TResponse>(IRequest<TResponse> command)
        {
            try
            {
                Console.WriteLine(command.ToString());
                dynamic responseObj = await _mediator.Send(command); 

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

                if (responseObj is byte[] pdfBytes)
                {
                    return File(pdfBytes, "application/pdf");
                }

                if (responseObj is FileContentResult)
                {
                    return responseObj;
                }

                return Ok(responseObj);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                // Logger.Error($"Request {command.GetType().FullName} failed.  {command.ToString()}. {e.Message}");
                return Problem("Something went wrong :(");
            }
        }
    }
}

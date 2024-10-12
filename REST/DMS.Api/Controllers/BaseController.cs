using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Mvc;
using MediatR;

namespace DMS.REST.Api.Controllers
{
    public abstract class BaseController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BaseController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // Refactored method to return ActionResult<object> to handle various response types
        protected async Task<ActionResult<dynamic>> ResponseAsync<TResponse>(IRequest<TResponse> command)   // Refactor: Specify TResponse type
        {
            try
            {
                Console.WriteLine(command.ToString());
                dynamic responseObj = await _mediator.Send(command);  // Send the command to MediatR

                if (responseObj is null)
                {
                    // Logger.Error($"Response for request {command.GetType().FullName} is null. {command.ToString()}");
                    return NotFound();
                }

                // If the response is a JsonObject, return it as a JSON response
                if (responseObj is JsonObject)
                {
                    var jsonRes = JsonSerializer.Serialize(responseObj);
                    // Logger.Info($"Response for request {command.GetType().FullName} is {jsonRes}");
                    return Content(jsonRes, "application/json");
                }

                // Handle file responses: PDF byte array
                if (responseObj is byte[] pdfBytes)
                {
                    return File(pdfBytes, "application/pdf");
                }

                // Handle other file responses
                if (responseObj is FileContentResult)
                {
                    return responseObj;
                }

                // For any other types of response, return it as a standard Ok response
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

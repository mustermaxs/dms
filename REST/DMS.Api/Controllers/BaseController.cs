using System.Text.Json;
using System.Text.Json.Nodes;
using DMS.Api.Configuration;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Microsoft.AspNetCore.Components.Forms.Mapping;
using Microsoft.AspNetCore.Http.HttpResults;

namespace DMS.REST.Api.Controllers
{
    public abstract class BaseController: ControllerBase
    {
        protected readonly IMediator _mediator;
        protected readonly ILogger _logger;

        public BaseController(IMediator mediator, ILogger logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        protected async Task<ActionResult<TResponseData>> ApiResponse<TRequest, TResponseData>(
            TRequest command,
            Func<TResponseData, ActionResult<TResponseData>> onSuccess,
            Func<ActionResult<TResponseData>> onFailure)
            where TResponseData : class
        {
            TResponseData? responseObj = null;
            try
            {
                _logger.LogInformation($"Sending command: {command.GetType().Name}");
                responseObj = await _mediator.Send(command) as TResponseData;

                if (responseObj is null)
                    throw new Exception("Response object is null");

                return onSuccess(responseObj);
            }
            catch (Exception e)
            {
                _logger.LogError($"Error: {e.Message}");
                return onFailure();
            }
        }
        
        protected async Task<ActionResult> ApiResponse<TRequest>(
            TRequest command,
            Func<ActionResult> onSuccess,
            Func<ActionResult> onFailure) 
        {
            try
            {
                _logger.LogInformation($"Sending command: {command.GetType().Name}");
                var responseObj = await _mediator.Send(command);
                
                return onSuccess();
            }
            catch (Exception e)
            {
                _logger.LogError($"Error: {e.Message}");
                return onFailure();
            }
        }
        protected async Task<ActionResult> ResponseAsync<TResponse>(IRequest<TResponse> command)
        {
            try
            {
                _logger.LogInformation(command.ToString());
                dynamic responseObj = await _mediator.Send(command); 

                if (responseObj is null)
                {
                    _logger.LogInformation($"Response for request {command.GetType().FullName} is null. {command.ToString()}");
                    return NotFound();
                }

                if (responseObj is JsonObject)
                {
                    var jsonRes = JsonSerializer.Serialize(responseObj);
                    _logger.LogInformation($"Response for request {command.GetType().FullName} is {jsonRes}");
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
                _logger.LogError(e.Message);
                return BadRequest(e.Message);
            }
        }
    }
}

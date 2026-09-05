namespace CampaignSaaS.Api.Common;

using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Mvc;

[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    private ISender? _mediator;
    protected ISender Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<ISender>();

    protected IActionResult HandleResult<T>(ErrorOr<T> result, int successStatusCode = StatusCodes.Status200OK)
    {
        if (!result.IsError)
        {
            return StatusCode(successStatusCode, ApiResponse<T>.Ok(result.Value));
        }

        if (result.Errors.All(e => e.Type == ErrorType.Validation))
        {
            var errorsDict = result.Errors
                .GroupBy(e => e.Code)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.Description).ToArray());

            var problemDetails = new ValidationProblemDetails(errorsDict)
            {
                Type = "https://api.campaignsaas.com/errors/validation-error",
                Title = "One or more validation errors occurred.",
                Status = StatusCodes.Status400BadRequest,
                Detail = "The payload contains invalid field values.",
                Instance = HttpContext.Request.Path
            };

            return BadRequest(problemDetails);
        }

        var firstError = result.FirstError;
        var (statusCode, title, type) = firstError.Type switch
        {
            ErrorType.NotFound => (StatusCodes.Status404NotFound, "Resource Not Found", "https://api.campaignsaas.com/errors/not-found"),
            ErrorType.Conflict => (StatusCodes.Status409Conflict, "Conflict Error", "https://api.campaignsaas.com/errors/conflict"),
            ErrorType.Unauthorized => (StatusCodes.Status401Unauthorized, "Unauthorized", "https://api.campaignsaas.com/errors/unauthorized"),
            ErrorType.Forbidden => (StatusCodes.Status403Forbidden, "Forbidden", "https://api.campaignsaas.com/errors/forbidden"),
            _ => (StatusCodes.Status500InternalServerError, "Internal Server Error", "https://api.campaignsaas.com/errors/server-error")
        };

        var errorProblem = new ProblemDetails
        {
            Type = type,
            Title = title,
            Status = statusCode,
            Detail = firstError.Description,
            Instance = HttpContext.Request.Path,
            Extensions = { ["errorCode"] = firstError.Code }
        };

        return StatusCode(statusCode, errorProblem);
    }
}

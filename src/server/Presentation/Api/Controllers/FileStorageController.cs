namespace CampaignSaaS.Api.Controllers;

using CampaignSaaS.Modules.Deliverable.Application.Commands.GeneratePresignedUploadUrl;
using CampaignSaaS.Modules.Deliverable.Contracts.DTOs;
using CampaignSaaS.Modules.Deliverable.Contracts.Requests;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Authorize]
public class FileStorageController : ControllerBase
{
    private readonly ISender _sender;

    public FileStorageController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost("api/v1/files/presigned-upload")]
    public async Task<IActionResult> GetPresignedUploadUrl(
        [FromBody] GetPresignedUploadUrlRequest request,
        CancellationToken cancellationToken)
    {
        var command = new GeneratePresignedUploadUrlCommand(
            request.FileName,
            request.ContentType,
            request.FileSize,
            request.CampaignId,
            request.DeliverableId);

        var result = await _sender.Send(command, cancellationToken);

        return result.Match(
            urlDto => Ok(new { success = true, data = urlDto, timestamp = DateTime.UtcNow }),
            errors => Problem(errors));
    }

    private IActionResult Problem(List<ErrorOr.Error> errors)
    {
        var firstError = errors.First();
        var statusCode = firstError.Type switch
        {
            ErrorOr.ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorOr.ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorOr.ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorOr.ErrorType.Forbidden => StatusCodes.Status403Forbidden,
            ErrorOr.ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            _ => StatusCodes.Status500InternalServerError
        };

        return StatusCode(statusCode, new
        {
            success = false,
            error = new
            {
                code = firstError.Code,
                message = firstError.Description,
                details = errors.Select(e => new { code = e.Code, message = e.Description })
            },
            timestamp = DateTime.UtcNow
        });
    }
}


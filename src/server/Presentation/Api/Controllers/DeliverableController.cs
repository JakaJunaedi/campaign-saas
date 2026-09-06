namespace CampaignSaaS.Api.Controllers;

using CampaignSaaS.Modules.Deliverable.Application.Commands.CreateContentSubmission;
using CampaignSaaS.Modules.Deliverable.Application.Commands.CreateDeliverable;
using CampaignSaaS.Modules.Deliverable.Application.Commands.SubmitPublishProof;
using CampaignSaaS.Modules.Deliverable.Application.Commands.UpdateDeliverable;
using CampaignSaaS.Modules.Deliverable.Application.Queries.GetContentSubmissions;
using CampaignSaaS.Modules.Deliverable.Application.Queries.GetDeliverableById;
using CampaignSaaS.Modules.Deliverable.Application.Queries.GetDeliverablesByCampaign;
using CampaignSaaS.Modules.Deliverable.Contracts.DTOs;
using CampaignSaaS.Modules.Deliverable.Contracts.Requests;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Authorize]
public class DeliverableController : ControllerBase
{
    private readonly ISender _sender;

    public DeliverableController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet("api/v1/campaigns/{campaignId:guid}/deliverables")]
    public async Task<IActionResult> GetDeliverablesByCampaign(Guid campaignId, CancellationToken cancellationToken)
    {
        var query = new GetDeliverablesByCampaignQuery(campaignId);
        var result = await _sender.Send(query, cancellationToken);

        return result.Match(
            deliverables => Ok(new { success = true, data = deliverables, timestamp = DateTime.UtcNow }),
            errors => Problem(errors));
    }

    [HttpPost("api/v1/campaigns/{campaignId:guid}/deliverables")]
    public async Task<IActionResult> CreateDeliverable(
        Guid campaignId,
        [FromBody] CreateDeliverableRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateDeliverableCommand(
            campaignId,
            request.CampaignCreatorId,
            request.Title,
            request.Platform,
            request.ContentType,
            request.BriefNotes,
            request.DueDate);

        var result = await _sender.Send(command, cancellationToken);

        return result.Match(
            deliverable => CreatedAtAction(nameof(GetDeliverableById), new { id = deliverable.Id }, new { success = true, data = deliverable, timestamp = DateTime.UtcNow }),
            errors => Problem(errors));
    }

    [HttpGet("api/v1/deliverables/{id:guid}")]
    public async Task<IActionResult> GetDeliverableById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetDeliverableByIdQuery(id);
        var result = await _sender.Send(query, cancellationToken);

        return result.Match(
            deliverable => Ok(new { success = true, data = deliverable, timestamp = DateTime.UtcNow }),
            errors => Problem(errors));
    }

    [HttpPut("api/v1/deliverables/{id:guid}")]
    public async Task<IActionResult> UpdateDeliverable(
        Guid id,
        [FromBody] UpdateDeliverableRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateDeliverableCommand(
            id,
            request.Title,
            request.Platform,
            request.ContentType,
            request.BriefNotes,
            request.DueDate);

        var result = await _sender.Send(command, cancellationToken);

        return result.Match(
            deliverable => Ok(new { success = true, data = deliverable, timestamp = DateTime.UtcNow }),
            errors => Problem(errors));
    }

    [HttpPost("api/v1/deliverables/{id:guid}/submissions")]
    public async Task<IActionResult> CreateContentSubmission(
        Guid id,
        [FromBody] CreateContentSubmissionRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateContentSubmissionCommand(
            id,
            request.MediaObjectKey,
            request.MediaFileName,
            request.MediaFileSize,
            request.Caption);

        var result = await _sender.Send(command, cancellationToken);

        return result.Match(
            submission => Ok(new { success = true, data = submission, timestamp = DateTime.UtcNow }),
            errors => Problem(errors));
    }

    [HttpGet("api/v1/deliverables/{id:guid}/submissions")]
    public async Task<IActionResult> GetContentSubmissions(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetContentSubmissionsQuery(id);
        var result = await _sender.Send(query, cancellationToken);

        return result.Match(
            submissions => Ok(new { success = true, data = submissions, timestamp = DateTime.UtcNow }),
            errors => Problem(errors));
    }

    [HttpPost("api/v1/deliverables/{id:guid}/publish-proof")]
    public async Task<IActionResult> SubmitPublishProof(
        Guid id,
        [FromBody] SubmitPublishProofRequest request,
        CancellationToken cancellationToken)
    {
        var command = new SubmitPublishProofCommand(
            id,
            request.LiveUrl,
            request.ProofMediaKey,
            request.PostingDate);

        var result = await _sender.Send(command, cancellationToken);

        return result.Match(
            deliverable => Ok(new { success = true, data = deliverable, timestamp = DateTime.UtcNow }),
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


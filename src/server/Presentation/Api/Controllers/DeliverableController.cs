namespace CampaignSaaS.Api.Controllers;

using CampaignSaaS.Api.Common;
using CampaignSaaS.Modules.Deliverable.Application.Commands.CreateContentSubmission;
using CampaignSaaS.Modules.Deliverable.Application.Commands.CreateDeliverable;
using CampaignSaaS.Modules.Deliverable.Application.Commands.SubmitPublishProof;
using CampaignSaaS.Modules.Deliverable.Application.Commands.UpdateDeliverable;
using CampaignSaaS.Modules.Deliverable.Application.Queries.GetContentSubmissions;
using CampaignSaaS.Modules.Deliverable.Application.Queries.GetDeliverableById;
using CampaignSaaS.Modules.Deliverable.Application.Queries.GetDeliverablesByCampaign;
using CampaignSaaS.Modules.Deliverable.Contracts.DTOs;
using CampaignSaaS.Modules.Deliverable.Contracts.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
public class DeliverableController : ApiControllerBase
{
    [HttpGet("api/v1/campaigns/{campaignId:guid}/deliverables")]
    [Authorize(Roles = "SuperAdmin,AgencyOwner,CampaignManager,ContentReviewer,ClientViewer,Creator")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<DeliverableDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetDeliverablesByCampaign(Guid campaignId, CancellationToken cancellationToken)
    {
        var query = new GetDeliverablesByCampaignQuery(campaignId);
        var result = await Mediator.Send(query, cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("api/v1/campaigns/{campaignId:guid}/deliverables")]
    [Authorize(Roles = "SuperAdmin,AgencyOwner,CampaignManager")]
    [ProducesResponseType(typeof(ApiResponse<DeliverableDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
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

        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result, StatusCodes.Status201Created);
    }

    [HttpGet("api/v1/deliverables/{id:guid}")]
    [Authorize(Roles = "SuperAdmin,AgencyOwner,CampaignManager,ContentReviewer,ClientViewer,Creator")]
    [ProducesResponseType(typeof(ApiResponse<DeliverableDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDeliverableById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetDeliverableByIdQuery(id);
        var result = await Mediator.Send(query, cancellationToken);
        return HandleResult(result);
    }

    [HttpPut("api/v1/deliverables/{id:guid}")]
    [Authorize(Roles = "SuperAdmin,AgencyOwner,CampaignManager")]
    [ProducesResponseType(typeof(ApiResponse<DeliverableDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
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

        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("api/v1/deliverables/{id:guid}/submissions")]
    [Authorize(Roles = "SuperAdmin,AgencyOwner,CampaignManager,Creator")]
    [ProducesResponseType(typeof(ApiResponse<ContentSubmissionDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
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

        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result, StatusCodes.Status201Created);
    }

    [HttpGet("api/v1/deliverables/{id:guid}/submissions")]
    [Authorize(Roles = "SuperAdmin,AgencyOwner,CampaignManager,ContentReviewer,ClientViewer,Creator")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<ContentSubmissionDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetContentSubmissions(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetContentSubmissionsQuery(id);
        var result = await Mediator.Send(query, cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("api/v1/deliverables/{id:guid}/publish-proof")]
    [Authorize(Roles = "SuperAdmin,AgencyOwner,CampaignManager,Creator")]
    [ProducesResponseType(typeof(ApiResponse<DeliverableDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
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

        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }
}

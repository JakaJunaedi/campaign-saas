namespace CampaignSaaS.Api.Controllers;

using System.Security.Claims;
using CampaignSaaS.Api.Common;
using CampaignSaaS.Modules.Approval.Application.Commands.SubmitReview;
using CampaignSaaS.Modules.Approval.Application.Queries.GetReviewById;
using CampaignSaaS.Modules.Approval.Application.Queries.GetReviewsBySubmissionId;
using CampaignSaaS.Modules.Approval.Contracts.DTOs;
using CampaignSaaS.Modules.Approval.Contracts.Requests;
using CampaignSaaS.Modules.Approval.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize(Roles = "SuperAdmin,AgencyOwner,CampaignManager,ContentReviewer")]
[ApiController]
public class ApprovalController : ApiControllerBase
{
    [HttpPost("api/v1/submissions/{submissionId:guid}/reviews")]
    [ProducesResponseType(typeof(ApiResponse<ApprovalReviewDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SubmitReview(
        Guid submissionId,
        [FromBody] SubmitReviewRequest request,
        CancellationToken cancellationToken)
    {
        var reviewerId = request.ReviewerId;
        if (!reviewerId.HasValue || reviewerId.Value == Guid.Empty)
        {
            var claimValue = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? User.FindFirst("sub")?.Value;

            if (Guid.TryParse(claimValue, out var parsedId))
            {
                reviewerId = parsedId;
            }
            else
            {
                reviewerId = Guid.Empty;
            }
        }

        if (!Enum.TryParse<ReviewDecision>(request.Decision, true, out var decision))
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Invalid Review Decision",
                Detail = $"Decision '{request.Decision}' is not valid. Valid values: Approved, RevisionRequested, Rejected.",
                Status = StatusCodes.Status400BadRequest
            });
        }

        var command = new SubmitReviewCommand(
            submissionId,
            reviewerId.Value,
            decision,
            request.FeedbackNotes);

        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result, StatusCodes.Status201Created);
    }

    [HttpGet("api/v1/submissions/{submissionId:guid}/reviews")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<ApprovalReviewDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetReviewsBySubmissionId(
        Guid submissionId,
        CancellationToken cancellationToken)
    {
        var query = new GetReviewsBySubmissionIdQuery(submissionId);
        var result = await Mediator.Send(query, cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("api/v1/reviews/{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<ApprovalReviewDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetReviewById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetReviewByIdQuery(id);
        var result = await Mediator.Send(query, cancellationToken);
        return HandleResult(result);
    }
}

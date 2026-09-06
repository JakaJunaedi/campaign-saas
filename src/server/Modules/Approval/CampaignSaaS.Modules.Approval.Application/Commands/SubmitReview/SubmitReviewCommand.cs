namespace CampaignSaaS.Modules.Approval.Application.Commands.SubmitReview;

using CampaignSaaS.Modules.Approval.Contracts.DTOs;
using CampaignSaaS.Modules.Approval.Domain.Enums;
using ErrorOr;
using MediatR;

public record SubmitReviewCommand(
    Guid ContentSubmissionId,
    Guid ReviewerId,
    ReviewDecision Decision,
    string? FeedbackNotes) : IRequest<ErrorOr<ApprovalReviewDto>>;

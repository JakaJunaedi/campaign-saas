namespace CampaignSaaS.Modules.Approval.Application.Queries.GetReviewsBySubmissionId;

using CampaignSaaS.Modules.Approval.Contracts.DTOs;
using ErrorOr;
using MediatR;

public record GetReviewsBySubmissionIdQuery(Guid ContentSubmissionId) : IRequest<ErrorOr<IReadOnlyList<ApprovalReviewDto>>>;

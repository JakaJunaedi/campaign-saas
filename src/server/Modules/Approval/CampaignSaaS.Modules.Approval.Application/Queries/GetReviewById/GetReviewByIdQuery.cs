namespace CampaignSaaS.Modules.Approval.Application.Queries.GetReviewById;

using CampaignSaaS.Modules.Approval.Contracts.DTOs;
using ErrorOr;
using MediatR;

public record GetReviewByIdQuery(Guid Id) : IRequest<ErrorOr<ApprovalReviewDto>>;

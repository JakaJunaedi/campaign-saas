namespace CampaignSaaS.Modules.Approval.Application.Queries.GetReviewById;

using CampaignSaaS.Modules.Approval.Application.Abstractions;
using CampaignSaaS.Modules.Approval.Contracts.DTOs;
using CampaignSaaS.SharedKernel.MultiTenancy;
using ErrorOr;
using MediatR;

public class GetReviewByIdQueryHandler : IRequestHandler<GetReviewByIdQuery, ErrorOr<ApprovalReviewDto>>
{
    private readonly IApprovalReviewRepository _reviewRepository;
    private readonly ICurrentTenantContext _tenantContext;

    public GetReviewByIdQueryHandler(
        IApprovalReviewRepository reviewRepository,
        ICurrentTenantContext tenantContext)
    {
        _reviewRepository = reviewRepository;
        _tenantContext = tenantContext;
    }

    public async Task<ErrorOr<ApprovalReviewDto>> Handle(GetReviewByIdQuery request, CancellationToken cancellationToken)
    {
        if (!_tenantContext.OrganizationId.HasValue)
        {
            return Error.Forbidden("Tenant.Required", "Active organization context is required.");
        }

        var review = await _reviewRepository.GetByIdAsync(request.Id, cancellationToken);
        if (review == null)
        {
            return Error.NotFound("ApprovalReview.NotFound", "Review not found.");
        }

        return new ApprovalReviewDto(
            review.Id,
            review.OrganizationId,
            review.ContentSubmissionId,
            review.ReviewerId,
            review.Decision.ToString(),
            review.FeedbackNotes,
            review.ReviewedAt);
    }
}

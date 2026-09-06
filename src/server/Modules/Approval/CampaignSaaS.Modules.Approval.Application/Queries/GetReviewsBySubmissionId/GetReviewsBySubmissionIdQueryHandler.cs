namespace CampaignSaaS.Modules.Approval.Application.Queries.GetReviewsBySubmissionId;

using CampaignSaaS.Modules.Approval.Application.Abstractions;
using CampaignSaaS.Modules.Approval.Contracts.DTOs;
using CampaignSaaS.SharedKernel.MultiTenancy;
using ErrorOr;
using MediatR;

public class GetReviewsBySubmissionIdQueryHandler : IRequestHandler<GetReviewsBySubmissionIdQuery, ErrorOr<IReadOnlyList<ApprovalReviewDto>>>
{
    private readonly IApprovalReviewRepository _reviewRepository;
    private readonly ICurrentTenantContext _tenantContext;

    public GetReviewsBySubmissionIdQueryHandler(
        IApprovalReviewRepository reviewRepository,
        ICurrentTenantContext tenantContext)
    {
        _reviewRepository = reviewRepository;
        _tenantContext = tenantContext;
    }

    public async Task<ErrorOr<IReadOnlyList<ApprovalReviewDto>>> Handle(GetReviewsBySubmissionIdQuery request, CancellationToken cancellationToken)
    {
        if (!_tenantContext.OrganizationId.HasValue)
        {
            return Error.Forbidden("Tenant.Required", "Active organization context is required.");
        }

        var reviews = await _reviewRepository.GetBySubmissionIdAsync(request.ContentSubmissionId, cancellationToken);

        var dtos = reviews.Select(r => new ApprovalReviewDto(
            r.Id,
            r.OrganizationId,
            r.ContentSubmissionId,
            r.ReviewerId,
            r.Decision.ToString(),
            r.FeedbackNotes,
            r.ReviewedAt)).ToList();

        return dtos;
    }
}

namespace CampaignSaaS.Modules.Approval.Application.Commands.SubmitReview;

using CampaignSaaS.Modules.Approval.Application.Abstractions;
using CampaignSaaS.Modules.Approval.Contracts.DTOs;
using CampaignSaaS.Modules.Approval.Domain.Entities;
using CampaignSaaS.SharedKernel.MultiTenancy;
using ErrorOr;
using MediatR;

public class SubmitReviewCommandHandler : IRequestHandler<SubmitReviewCommand, ErrorOr<ApprovalReviewDto>>
{
    private readonly IApprovalReviewRepository _reviewRepository;
    private readonly IApprovalUnitOfWork _unitOfWork;
    private readonly ICurrentTenantContext _tenantContext;

    public SubmitReviewCommandHandler(
        IApprovalReviewRepository reviewRepository,
        IApprovalUnitOfWork unitOfWork,
        ICurrentTenantContext tenantContext)
    {
        _reviewRepository = reviewRepository;
        _unitOfWork = unitOfWork;
        _tenantContext = tenantContext;
    }

    public async Task<ErrorOr<ApprovalReviewDto>> Handle(SubmitReviewCommand request, CancellationToken cancellationToken)
    {
        if (!_tenantContext.OrganizationId.HasValue)
        {
            return Error.Forbidden("Tenant.Required", "Active organization context is required.");
        }

        var orgId = _tenantContext.OrganizationId.Value;

        var review = ApprovalReview.Create(
            orgId,
            request.ContentSubmissionId,
            request.ReviewerId,
            request.Decision,
            request.FeedbackNotes);

        await _reviewRepository.AddAsync(review, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

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

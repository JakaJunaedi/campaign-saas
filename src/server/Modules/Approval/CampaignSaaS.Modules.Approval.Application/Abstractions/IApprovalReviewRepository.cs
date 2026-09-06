namespace CampaignSaaS.Modules.Approval.Application.Abstractions;

using CampaignSaaS.Modules.Approval.Domain.Entities;

public interface IApprovalReviewRepository
{
    Task<ApprovalReview?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ApprovalReview>> GetBySubmissionIdAsync(Guid contentSubmissionId, CancellationToken cancellationToken = default);
    Task AddAsync(ApprovalReview review, CancellationToken cancellationToken = default);
}

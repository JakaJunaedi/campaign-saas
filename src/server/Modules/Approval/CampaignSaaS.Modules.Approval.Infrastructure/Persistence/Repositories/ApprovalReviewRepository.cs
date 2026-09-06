namespace CampaignSaaS.Modules.Approval.Infrastructure.Persistence.Repositories;

using CampaignSaaS.Modules.Approval.Application.Abstractions;
using CampaignSaaS.Modules.Approval.Domain.Entities;
using CampaignSaaS.Modules.Approval.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

public class ApprovalReviewRepository : IApprovalReviewRepository
{
    private readonly ApprovalDbContext _dbContext;

    public ApprovalReviewRepository(ApprovalDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ApprovalReview?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.ApprovalReviews
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<ApprovalReview>> GetBySubmissionIdAsync(Guid contentSubmissionId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.ApprovalReviews
            .Where(r => r.ContentSubmissionId == contentSubmissionId)
            .OrderByDescending(r => r.ReviewedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(ApprovalReview review, CancellationToken cancellationToken = default)
    {
        await _dbContext.ApprovalReviews.AddAsync(review, cancellationToken);
    }
}

namespace CampaignSaaS.Modules.Approval.Infrastructure.Persistence;

using CampaignSaaS.Modules.Approval.Domain.Entities;
using CampaignSaaS.SharedKernel.MultiTenancy;
using Microsoft.EntityFrameworkCore;

public class ApprovalDbContext : DbContext
{
    private readonly ICurrentTenantContext _tenantContext;

    public ApprovalDbContext(
        DbContextOptions<ApprovalDbContext> options,
        ICurrentTenantContext tenantContext) : base(options)
    {
        _tenantContext = tenantContext;
    }

    public DbSet<ApprovalReview> ApprovalReviews => Set<ApprovalReview>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApprovalDbContext).Assembly);

        // Global Multi-Tenancy Query Filter
        modelBuilder.Entity<ApprovalReview>()
            .HasQueryFilter(r => _tenantContext.OrganizationId == null || r.OrganizationId == _tenantContext.OrganizationId);
    }
}

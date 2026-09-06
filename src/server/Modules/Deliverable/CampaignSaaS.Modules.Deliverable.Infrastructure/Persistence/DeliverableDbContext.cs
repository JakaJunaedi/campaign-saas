namespace CampaignSaaS.Modules.Deliverable.Infrastructure.Persistence;

using CampaignSaaS.Modules.Deliverable.Domain.Entities;
using CampaignSaaS.SharedKernel.MultiTenancy;
using Microsoft.EntityFrameworkCore;

public class DeliverableDbContext : DbContext
{
    private readonly ICurrentTenantContext _tenantContext;

    public DeliverableDbContext(
        DbContextOptions<DeliverableDbContext> options,
        ICurrentTenantContext tenantContext) : base(options)
    {
        _tenantContext = tenantContext;
    }

    public DbSet<Deliverable> Deliverables => Set<Deliverable>();
    public DbSet<ContentSubmission> ContentSubmissions => Set<ContentSubmission>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DeliverableDbContext).Assembly);

        // Global Multi-Tenancy Query Filters
        modelBuilder.Entity<Deliverable>()
            .HasQueryFilter(d => _tenantContext.OrganizationId == null || d.OrganizationId == _tenantContext.OrganizationId);

        modelBuilder.Entity<ContentSubmission>()
            .HasQueryFilter(s => _tenantContext.OrganizationId == null || s.OrganizationId == _tenantContext.OrganizationId);
    }
}


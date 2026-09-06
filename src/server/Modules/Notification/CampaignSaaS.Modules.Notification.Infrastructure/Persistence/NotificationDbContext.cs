namespace CampaignSaaS.Modules.Notification.Infrastructure.Persistence;

using CampaignSaaS.Modules.Notification.Domain.Entities;
using CampaignSaaS.SharedKernel.MultiTenancy;
using Microsoft.EntityFrameworkCore;

public class NotificationDbContext : DbContext
{
    private readonly ICurrentTenantContext _tenantContext;

    public NotificationDbContext(
        DbContextOptions<NotificationDbContext> options,
        ICurrentTenantContext tenantContext) : base(options)
    {
        _tenantContext = tenantContext;
    }

    public DbSet<InAppNotification> Notifications => Set<InAppNotification>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(NotificationDbContext).Assembly);

        // Global Multi-Tenancy Query Filter
        modelBuilder.Entity<InAppNotification>()
            .HasQueryFilter(n => _tenantContext.OrganizationId == null || n.OrganizationId == _tenantContext.OrganizationId);
    }
}

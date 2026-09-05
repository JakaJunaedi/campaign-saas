namespace CampaignSaaS.Modules.Client.Infrastructure.Persistence;

using CampaignSaaS.Modules.Client.Domain.Entities;
using CampaignSaaS.SharedKernel.MultiTenancy;
using Microsoft.EntityFrameworkCore;

public class ClientDbContext : DbContext
{
    private readonly ICurrentTenantContext? _tenantContext;

    public ClientDbContext(
        DbContextOptions<ClientDbContext> options,
        ICurrentTenantContext? tenantContext = null)
        : base(options)
    {
        _tenantContext = tenantContext;
    }

    public DbSet<Client> Clients => Set<Client>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ClientDbContext).Assembly);

        // Global multi-tenant query filter
        modelBuilder.Entity<Client>().HasQueryFilter(c =>
            !c.IsDeleted &&
            (_tenantContext == null ||
             _tenantContext.IsSuperAdmin ||
             (_tenantContext.OrganizationId != null && c.OrganizationId == _tenantContext.OrganizationId.Value)));
    }
}

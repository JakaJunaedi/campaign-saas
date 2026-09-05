namespace CampaignSaaS.SharedKernel.MultiTenancy;

public interface ICurrentTenantContext
{
    Guid? OrganizationId { get; }
    bool IsSuperAdmin { get; }
    Guid? CurrentUserId { get; }
    void SetTenant(Guid organizationId, Guid? userId = null, bool isSuperAdmin = false);
}

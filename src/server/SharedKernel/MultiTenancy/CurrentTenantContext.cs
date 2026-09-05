namespace CampaignSaaS.SharedKernel.MultiTenancy;

public class CurrentTenantContext : ICurrentTenantContext
{
    public Guid? OrganizationId { get; private set; }
    public bool IsSuperAdmin { get; private set; }
    public Guid? CurrentUserId { get; private set; }

    public void SetTenant(Guid organizationId, Guid? userId = null, bool isSuperAdmin = false)
    {
        OrganizationId = organizationId;
        CurrentUserId = userId;
        IsSuperAdmin = isSuperAdmin;
    }
}

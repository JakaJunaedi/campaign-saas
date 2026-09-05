namespace CampaignSaaS.SharedKernel.MultiTenancy;

public interface ITenantEntity
{
    Guid OrganizationId { get; }
}

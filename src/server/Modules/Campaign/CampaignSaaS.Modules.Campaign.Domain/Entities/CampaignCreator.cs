namespace CampaignSaaS.Modules.Campaign.Domain.Entities;

using CampaignSaaS.Modules.Campaign.Domain.Enums;
using CampaignSaaS.Modules.Campaign.Domain.Events;
using CampaignSaaS.SharedKernel.Domain;
using CampaignSaaS.SharedKernel.MultiTenancy;

public class CampaignCreator : AggregateRoot<Guid>, ITenantEntity
{
    public Guid OrganizationId { get; private set; }
    public Guid CampaignId { get; private set; }
    public Guid CreatorId { get; private set; }
    public CampaignCreatorStatus Status { get; private set; } = CampaignCreatorStatus.Shortlisted;
    public decimal AgreedRate { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? UpdatedAt { get; private set; }

    private CampaignCreator() { }

    private CampaignCreator(
        Guid id,
        Guid organizationId,
        Guid campaignId,
        Guid creatorId,
        CampaignCreatorStatus status,
        decimal agreedRate) : base(id)
    {
        OrganizationId = organizationId;
        CampaignId = campaignId;
        CreatorId = creatorId;
        Status = status;
        AgreedRate = Math.Max(0, agreedRate);
        CreatedAt = DateTimeOffset.UtcNow;
    }

    public static CampaignCreator Create(
        Guid organizationId,
        Guid campaignId,
        Guid creatorId,
        CampaignCreatorStatus status = CampaignCreatorStatus.Shortlisted,
        decimal agreedRate = 0)
    {
        var rosterItem = new CampaignCreator(Guid.NewGuid(), organizationId, campaignId, creatorId, status, agreedRate);
        rosterItem.AddDomainEvent(new CreatorAssignedToCampaignDomainEvent(rosterItem.Id, rosterItem.OrganizationId, rosterItem.CampaignId, rosterItem.CreatorId, rosterItem.CreatedAt));
        return rosterItem;
    }

    public void UpdateStatus(CampaignCreatorStatus status)
    {
        Status = status;
        UpdatedAt = DateTimeOffset.UtcNow;
        AddDomainEvent(new CampaignCreatorStatusChangedDomainEvent(Id, OrganizationId, CampaignId, CreatorId, Status, UpdatedAt.Value));
    }

    public void UpdateRate(decimal agreedRate)
    {
        AgreedRate = Math.Max(0, agreedRate);
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}

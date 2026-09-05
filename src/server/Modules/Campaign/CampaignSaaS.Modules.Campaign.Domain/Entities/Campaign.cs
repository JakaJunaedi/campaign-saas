namespace CampaignSaaS.Modules.Campaign.Domain.Entities;

using CampaignSaaS.Modules.Campaign.Domain.Enums;
using CampaignSaaS.Modules.Campaign.Domain.Events;
using CampaignSaaS.SharedKernel.Domain;
using CampaignSaaS.SharedKernel.MultiTenancy;

public class Campaign : AggregateRoot<Guid>, ITenantEntity
{
    public Guid OrganizationId { get; private set; }
    public Guid ClientId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public decimal Budget { get; private set; }
    public DateOnly StartDate { get; private set; }
    public DateOnly EndDate { get; private set; }
    public CampaignStatus Status { get; private set; } = CampaignStatus.Draft;
    public DateTimeOffset CreatedAt { get; private set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? UpdatedAt { get; private set; }
    public bool IsDeleted { get; private set; }

    private Campaign() { }

    private Campaign(
        Guid id,
        Guid organizationId,
        Guid clientId,
        string title,
        string? description,
        decimal budget,
        DateOnly startDate,
        DateOnly endDate,
        CampaignStatus status) : base(id)
    {
        OrganizationId = organizationId;
        ClientId = clientId;
        Title = title.Trim();
        Description = description?.Trim();
        Budget = Math.Max(0, budget);
        StartDate = startDate;
        EndDate = endDate;
        Status = status;
        CreatedAt = DateTimeOffset.UtcNow;
        IsDeleted = false;
    }

    public static Campaign Create(
        Guid organizationId,
        Guid clientId,
        string title,
        string? description,
        decimal budget,
        DateOnly startDate,
        DateOnly endDate,
        CampaignStatus status = CampaignStatus.Draft)
    {
        var campaign = new Campaign(Guid.NewGuid(), organizationId, clientId, title, description, budget, startDate, endDate, status);
        campaign.AddDomainEvent(new CampaignCreatedDomainEvent(campaign.Id, campaign.OrganizationId, campaign.ClientId, campaign.Title, campaign.CreatedAt));
        return campaign;
    }

    public void UpdateDetails(string title, string? description, decimal budget, DateOnly startDate, DateOnly endDate)
    {
        Title = title.Trim();
        Description = description?.Trim();
        Budget = Math.Max(0, budget);
        StartDate = startDate;
        EndDate = endDate;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void SetStatus(CampaignStatus status)
    {
        Status = status;
        UpdatedAt = DateTimeOffset.UtcNow;
        AddDomainEvent(new CampaignStatusChangedDomainEvent(Id, OrganizationId, Status, UpdatedAt.Value));
    }

    public void Delete()
    {
        IsDeleted = true;
        UpdatedAt = DateTimeOffset.UtcNow;
        AddDomainEvent(new CampaignDeletedDomainEvent(Id, OrganizationId, UpdatedAt.Value));
    }
}

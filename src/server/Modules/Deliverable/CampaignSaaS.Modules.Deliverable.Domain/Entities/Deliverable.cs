namespace CampaignSaaS.Modules.Deliverable.Domain.Entities;

using CampaignSaaS.Modules.Deliverable.Domain.Enums;
using CampaignSaaS.Modules.Deliverable.Domain.Events;
using CampaignSaaS.SharedKernel.Domain;
using CampaignSaaS.SharedKernel.MultiTenancy;

public class Deliverable : AggregateRoot<Guid>, ITenantEntity
{
    public Guid OrganizationId { get; private set; }
    public Guid CampaignId { get; private set; }
    public Guid CampaignCreatorId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public PlatformType Platform { get; private set; }
    public ContentType ContentType { get; private set; }
    public string? BriefNotes { get; private set; }
    public DateOnly DueDate { get; private set; }
    public DateOnly? PostingDate { get; private set; }
    public DeliverableStatus Status { get; private set; } = DeliverableStatus.Pending;
    public string? LiveUrl { get; private set; }
    public string? ProofMediaKey { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? UpdatedAt { get; private set; }

    private Deliverable() { }

    private Deliverable(
        Guid id,
        Guid organizationId,
        Guid campaignId,
        Guid campaignCreatorId,
        string title,
        PlatformType platform,
        ContentType contentType,
        string? briefNotes,
        DateOnly dueDate) : base(id)
    {
        OrganizationId = organizationId;
        CampaignId = campaignId;
        CampaignCreatorId = campaignCreatorId;
        Title = title.Trim();
        Platform = platform;
        ContentType = contentType;
        BriefNotes = briefNotes?.Trim();
        DueDate = dueDate;
        Status = DeliverableStatus.Pending;
        CreatedAt = DateTimeOffset.UtcNow;
    }

    public static Deliverable Create(
        Guid organizationId,
        Guid campaignId,
        Guid campaignCreatorId,
        string title,
        PlatformType platform,
        ContentType contentType,
        string? briefNotes,
        DateOnly dueDate)
    {
        var deliverable = new Deliverable(
            Guid.NewGuid(),
            organizationId,
            campaignId,
            campaignCreatorId,
            title,
            platform,
            contentType,
            briefNotes,
            dueDate);

        deliverable.AddDomainEvent(new DeliverableCreatedDomainEvent(
            deliverable.Id,
            deliverable.OrganizationId,
            deliverable.CampaignId,
            deliverable.CampaignCreatorId,
            deliverable.Title,
            deliverable.CreatedAt));

        return deliverable;
    }

    public void UpdateDetails(
        string title,
        PlatformType platform,
        ContentType contentType,
        string? briefNotes,
        DateOnly dueDate)
    {
        Title = title.Trim();
        Platform = platform;
        ContentType = contentType;
        BriefNotes = briefNotes?.Trim();
        DueDate = dueDate;
        UpdatedAt = DateTimeOffset.UtcNow;

        AddDomainEvent(new DeliverableUpdatedDomainEvent(Id, OrganizationId, UpdatedAt.Value));
    }

    public void SetStatus(DeliverableStatus status)
    {
        Status = status;
        UpdatedAt = DateTimeOffset.UtcNow;
        AddDomainEvent(new DeliverableStatusChangedDomainEvent(Id, OrganizationId, Status, UpdatedAt.Value));
    }

    public void MarkAsSubmitted()
    {
        SetStatus(DeliverableStatus.Submitted);
    }

    public void MarkAsRevision()
    {
        SetStatus(DeliverableStatus.Revision);
    }

    public void MarkAsApproved()
    {
        SetStatus(DeliverableStatus.Approved);
    }

    public void MarkAsRejected()
    {
        SetStatus(DeliverableStatus.Rejected);
    }

    public void MarkAsPublished(string liveUrl, string? proofMediaKey, DateOnly? postingDate = null)
    {
        LiveUrl = liveUrl.Trim();
        ProofMediaKey = proofMediaKey?.Trim();
        PostingDate = postingDate ?? DateOnly.FromDateTime(DateTime.UtcNow);
        Status = DeliverableStatus.Published;
        UpdatedAt = DateTimeOffset.UtcNow;

        AddDomainEvent(new DeliverablePublishedDomainEvent(
            Id,
            OrganizationId,
            LiveUrl,
            ProofMediaKey,
            PostingDate.Value,
            UpdatedAt.Value));
    }
}


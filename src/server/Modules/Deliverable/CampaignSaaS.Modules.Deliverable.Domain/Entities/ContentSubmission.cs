namespace CampaignSaaS.Modules.Deliverable.Domain.Entities;

using CampaignSaaS.Modules.Deliverable.Domain.Events;
using CampaignSaaS.SharedKernel.Domain;
using CampaignSaaS.SharedKernel.MultiTenancy;

public class ContentSubmission : AggregateRoot<Guid>, ITenantEntity
{
    public Guid OrganizationId { get; private set; }
    public Guid DeliverableId { get; private set; }
    public int VersionNumber { get; private set; } = 1;
    public string MediaObjectKey { get; private set; } = string.Empty;
    public string MediaFileName { get; private set; } = string.Empty;
    public long MediaFileSize { get; private set; }
    public string? Caption { get; private set; }
    public DateTimeOffset SubmittedAt { get; private set; } = DateTimeOffset.UtcNow;

    private ContentSubmission() { }

    private ContentSubmission(
        Guid id,
        Guid organizationId,
        Guid deliverableId,
        int versionNumber,
        string mediaObjectKey,
        string mediaFileName,
        long mediaFileSize,
        string? caption) : base(id)
    {
        OrganizationId = organizationId;
        DeliverableId = deliverableId;
        VersionNumber = versionNumber;
        MediaObjectKey = mediaObjectKey.Trim();
        MediaFileName = mediaFileName.Trim();
        MediaFileSize = Math.Max(0, mediaFileSize);
        Caption = caption?.Trim();
        SubmittedAt = DateTimeOffset.UtcNow;
    }

    public static ContentSubmission Create(
        Guid organizationId,
        Guid deliverableId,
        int versionNumber,
        string mediaObjectKey,
        string mediaFileName,
        long mediaFileSize,
        string? caption)
    {
        var submission = new ContentSubmission(
            Guid.NewGuid(),
            organizationId,
            deliverableId,
            versionNumber,
            mediaObjectKey,
            mediaFileName,
            mediaFileSize,
            caption);

        submission.AddDomainEvent(new ContentSubmittedDomainEvent(
            submission.DeliverableId,
            submission.OrganizationId,
            submission.Id,
            submission.VersionNumber,
            submission.MediaObjectKey,
            submission.SubmittedAt));

        return submission;
    }
}


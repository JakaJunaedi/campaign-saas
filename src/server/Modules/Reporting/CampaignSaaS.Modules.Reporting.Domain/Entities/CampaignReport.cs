namespace CampaignSaaS.Modules.Reporting.Domain.Entities;

using CampaignSaaS.Modules.Reporting.Domain.Enums;
using CampaignSaaS.Modules.Reporting.Domain.Events;
using CampaignSaaS.SharedKernel.Domain;
using CampaignSaaS.SharedKernel.MultiTenancy;

public class CampaignReport : AggregateRoot<Guid>, ITenantEntity
{
    public Guid OrganizationId { get; private set; }
    public Guid CampaignId { get; private set; }
    public ReportStatus Status { get; private set; } = ReportStatus.Pending;
    public string? FileObjectKey { get; private set; }
    public string? ErrorMessage { get; private set; }
    public DateTimeOffset RequestedAt { get; private set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? CompletedAt { get; private set; }

    private CampaignReport() { }

    private CampaignReport(
        Guid id,
        Guid organizationId,
        Guid campaignId) : base(id)
    {
        OrganizationId = organizationId;
        CampaignId = campaignId;
        Status = ReportStatus.Pending;
        RequestedAt = DateTimeOffset.UtcNow;
    }

    public static CampaignReport RequestReport(Guid organizationId, Guid campaignId)
    {
        var report = new CampaignReport(Guid.NewGuid(), organizationId, campaignId);

        report.AddDomainEvent(new ReportRequestedDomainEvent(
            report.Id,
            report.OrganizationId,
            report.CampaignId,
            report.RequestedAt));

        return report;
    }

    public void MarkAsGenerating()
    {
        Status = ReportStatus.Generating;
    }

    public void MarkAsCompleted(string fileObjectKey)
    {
        Status = ReportStatus.Completed;
        FileObjectKey = fileObjectKey.Trim();
        CompletedAt = DateTimeOffset.UtcNow;
        ErrorMessage = null;

        AddDomainEvent(new ReportGeneratedDomainEvent(
            Id,
            OrganizationId,
            CampaignId,
            FileObjectKey,
            CompletedAt.Value));
    }

    public void MarkAsFailed(string errorMessage)
    {
        Status = ReportStatus.Failed;
        ErrorMessage = errorMessage.Trim();
        CompletedAt = DateTimeOffset.UtcNow;

        AddDomainEvent(new ReportGenerationFailedDomainEvent(
            Id,
            OrganizationId,
            CampaignId,
            ErrorMessage,
            CompletedAt.Value));
    }
}

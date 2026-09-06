namespace CampaignSaaS.SharedKernel.IntegrationEvents;

public record GenerateCampaignReportJob(
    Guid ReportId,
    Guid OrganizationId,
    Guid CampaignId,
    DateTime RequestedAt
);

public record CampaignReportGeneratedIntegrationEvent(
    Guid EventId,
    DateTime OccurredOnUtc,
    Guid OrganizationId,
    Guid CampaignId,
    Guid ReportId,
    string FileObjectKey,
    string DownloadUrl
) : IIntegrationEvent
{
    public static CampaignReportGeneratedIntegrationEvent Create(
        Guid organizationId,
        Guid campaignId,
        Guid reportId,
        string fileObjectKey,
        string downloadUrl) =>
        new(Guid.NewGuid(), DateTime.UtcNow, organizationId, campaignId, reportId, fileObjectKey, downloadUrl);
}

public record CampaignReportFailedIntegrationEvent(
    Guid EventId,
    DateTime OccurredOnUtc,
    Guid OrganizationId,
    Guid CampaignId,
    Guid ReportId,
    string ErrorMessage
) : IIntegrationEvent
{
    public static CampaignReportFailedIntegrationEvent Create(
        Guid organizationId,
        Guid campaignId,
        Guid reportId,
        string errorMessage) =>
        new(Guid.NewGuid(), DateTime.UtcNow, organizationId, campaignId, reportId, errorMessage);
}

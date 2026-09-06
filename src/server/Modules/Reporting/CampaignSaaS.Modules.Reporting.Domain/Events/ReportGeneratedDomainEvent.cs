namespace CampaignSaaS.Modules.Reporting.Domain.Events;

using CampaignSaaS.SharedKernel.Domain;

public record ReportGeneratedDomainEvent(
    Guid ReportId,
    Guid OrganizationId,
    Guid CampaignId,
    string FileObjectKey,
    DateTimeOffset CompletedAt) : IDomainEvent;

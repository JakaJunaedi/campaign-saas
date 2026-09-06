namespace CampaignSaaS.Modules.Reporting.Domain.Events;

using CampaignSaaS.SharedKernel.Domain;

public record ReportGenerationFailedDomainEvent(
    Guid ReportId,
    Guid OrganizationId,
    Guid CampaignId,
    string ErrorMessage,
    DateTimeOffset FailedAt) : IDomainEvent;

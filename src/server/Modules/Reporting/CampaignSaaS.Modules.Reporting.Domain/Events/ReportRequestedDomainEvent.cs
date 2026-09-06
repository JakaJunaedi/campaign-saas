namespace CampaignSaaS.Modules.Reporting.Domain.Events;

using CampaignSaaS.SharedKernel.Domain;

public record ReportRequestedDomainEvent(
    Guid ReportId,
    Guid OrganizationId,
    Guid CampaignId,
    DateTimeOffset RequestedAt) : IDomainEvent;

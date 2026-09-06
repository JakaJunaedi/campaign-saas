namespace CampaignSaaS.Modules.Reporting.Domain.Events;

using CampaignSaaS.SharedKernel.Domain;

public record MetricsRecordedDomainEvent(
    Guid MetricId,
    Guid OrganizationId,
    Guid DeliverableId,
    long Reach,
    long Impressions,
    long Views,
    long TotalEngagement,
    DateTimeOffset RecordedAt) : IDomainEvent;

namespace CampaignSaaS.Modules.Deliverable.Domain.Events;

using CampaignSaaS.Modules.Deliverable.Domain.Enums;
using CampaignSaaS.SharedKernel.Domain;

public record DeliverableStatusChangedDomainEvent(
    Guid DeliverableId,
    Guid OrganizationId,
    DeliverableStatus Status,
    DateTimeOffset ChangedAt) : IDomainEvent;


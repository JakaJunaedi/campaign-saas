namespace CampaignSaaS.Modules.Deliverable.Domain.Events;

using CampaignSaaS.SharedKernel.Domain;

public record DeliverableUpdatedDomainEvent(
    Guid DeliverableId,
    Guid OrganizationId,
    DateTimeOffset UpdatedAt) : IDomainEvent;


namespace CampaignSaaS.Modules.Deliverable.Domain.Events;

using CampaignSaaS.SharedKernel.Domain;

public record DeliverableCreatedDomainEvent(
    Guid DeliverableId,
    Guid OrganizationId,
    Guid CampaignId,
    Guid CampaignCreatorId,
    string Title,
    DateTimeOffset CreatedAt) : IDomainEvent;


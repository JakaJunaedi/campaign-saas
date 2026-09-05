namespace CampaignSaaS.Modules.Client.Domain.Events;

using CampaignSaaS.SharedKernel.Domain;

public record ClientUpdatedDomainEvent(
    Guid ClientId,
    Guid OrganizationId,
    string Name,
    DateTimeOffset OccurredOn) : IDomainEvent;

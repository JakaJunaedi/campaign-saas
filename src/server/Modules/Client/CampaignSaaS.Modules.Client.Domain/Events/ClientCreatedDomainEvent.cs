namespace CampaignSaaS.Modules.Client.Domain.Events;

using CampaignSaaS.SharedKernel.Domain;

public record ClientCreatedDomainEvent(
    Guid ClientId,
    Guid OrganizationId,
    string Name,
    DateTimeOffset OccurredOn) : IDomainEvent;

namespace CampaignSaaS.Modules.Client.Domain.Events;

using CampaignSaaS.SharedKernel.Domain;

public record ClientDeletedDomainEvent(
    Guid ClientId,
    Guid OrganizationId,
    DateTimeOffset OccurredOn) : IDomainEvent;

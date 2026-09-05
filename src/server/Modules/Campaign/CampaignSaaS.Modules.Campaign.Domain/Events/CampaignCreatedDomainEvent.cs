namespace CampaignSaaS.Modules.Campaign.Domain.Events;

using CampaignSaaS.SharedKernel.Domain;

public record CampaignCreatedDomainEvent(
    Guid CampaignId,
    Guid OrganizationId,
    Guid ClientId,
    string Title,
    DateTimeOffset OccurredOn) : IDomainEvent;

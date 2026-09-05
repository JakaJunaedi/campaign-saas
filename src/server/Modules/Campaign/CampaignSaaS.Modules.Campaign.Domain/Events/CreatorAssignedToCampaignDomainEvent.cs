namespace CampaignSaaS.Modules.Campaign.Domain.Events;

using CampaignSaaS.SharedKernel.Domain;

public record CreatorAssignedToCampaignDomainEvent(
    Guid CampaignCreatorId,
    Guid OrganizationId,
    Guid CampaignId,
    Guid CreatorId,
    DateTimeOffset OccurredOn) : IDomainEvent;

namespace CampaignSaaS.Modules.Campaign.Domain.Events;

using CampaignSaaS.Modules.Campaign.Domain.Enums;
using CampaignSaaS.SharedKernel.Domain;

public record CampaignCreatorStatusChangedDomainEvent(
    Guid CampaignCreatorId,
    Guid OrganizationId,
    Guid CampaignId,
    Guid CreatorId,
    CampaignCreatorStatus Status,
    DateTimeOffset OccurredOn) : IDomainEvent;

namespace CampaignSaaS.Modules.Campaign.Domain.Events;

using CampaignSaaS.Modules.Campaign.Domain.Enums;
using CampaignSaaS.SharedKernel.Domain;

public record CampaignStatusChangedDomainEvent(
    Guid CampaignId,
    Guid OrganizationId,
    CampaignStatus Status,
    DateTimeOffset OccurredOn) : IDomainEvent;

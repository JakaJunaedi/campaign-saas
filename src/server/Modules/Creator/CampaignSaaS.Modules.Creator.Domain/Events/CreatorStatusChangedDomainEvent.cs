namespace CampaignSaaS.Modules.Creator.Domain.Events;

using CampaignSaaS.Modules.Creator.Domain.Enums;
using CampaignSaaS.SharedKernel.Domain;

public record CreatorStatusChangedDomainEvent(
    Guid CreatorId,
    Guid OrganizationId,
    CreatorStatus Status,
    DateTimeOffset OccurredOn) : IDomainEvent;

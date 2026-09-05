namespace CampaignSaaS.Modules.Creator.Domain.Events;

using CampaignSaaS.SharedKernel.Domain;

public record CreatorCreatedDomainEvent(
    Guid CreatorId,
    Guid OrganizationId,
    string FullName,
    string Niche,
    DateTimeOffset OccurredOn) : IDomainEvent;

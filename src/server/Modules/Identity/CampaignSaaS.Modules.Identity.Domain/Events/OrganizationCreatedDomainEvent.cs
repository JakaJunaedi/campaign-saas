namespace CampaignSaaS.Modules.Identity.Domain.Events;

using CampaignSaaS.SharedKernel.Domain;

public record OrganizationCreatedDomainEvent(
    Guid OrganizationId,
    string Name,
    string Slug,
    DateTimeOffset OccurredOn) : IDomainEvent;

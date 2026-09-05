namespace CampaignSaaS.Modules.Identity.Domain.Events;

using CampaignSaaS.Modules.Identity.Domain.Enums;
using CampaignSaaS.SharedKernel.Domain;

public record UserRegisteredDomainEvent(
    Guid UserId,
    Guid OrganizationId,
    string Email,
    UserRole Role,
    DateTimeOffset OccurredOn) : IDomainEvent;

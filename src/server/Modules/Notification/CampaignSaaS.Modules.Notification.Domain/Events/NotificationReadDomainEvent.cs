namespace CampaignSaaS.Modules.Notification.Domain.Events;

using CampaignSaaS.SharedKernel.Domain;

public record NotificationReadDomainEvent(
    Guid NotificationId,
    Guid OrganizationId,
    Guid UserId,
    DateTimeOffset ReadAt
) : IDomainEvent;

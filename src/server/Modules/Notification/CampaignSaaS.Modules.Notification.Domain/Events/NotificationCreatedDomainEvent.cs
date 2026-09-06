namespace CampaignSaaS.Modules.Notification.Domain.Events;

using CampaignSaaS.SharedKernel.Domain;

public record NotificationCreatedDomainEvent(
    Guid NotificationId,
    Guid OrganizationId,
    Guid UserId,
    string Title,
    string Message,
    DateTimeOffset CreatedAt
) : IDomainEvent;

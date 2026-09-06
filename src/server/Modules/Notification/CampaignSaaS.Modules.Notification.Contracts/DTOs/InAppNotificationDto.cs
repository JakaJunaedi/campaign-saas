namespace CampaignSaaS.Modules.Notification.Contracts.DTOs;

public record InAppNotificationDto(
    Guid Id,
    Guid OrganizationId,
    Guid UserId,
    string Title,
    string Message,
    string? LinkUrl,
    bool IsRead,
    DateTimeOffset CreatedAt,
    DateTimeOffset? ReadAt
);

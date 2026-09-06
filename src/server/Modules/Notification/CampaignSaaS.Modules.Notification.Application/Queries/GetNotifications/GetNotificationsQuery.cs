namespace CampaignSaaS.Modules.Notification.Application.Queries.GetNotifications;

using CampaignSaaS.Modules.Notification.Contracts.DTOs;
using ErrorOr;
using MediatR;

public record GetNotificationsQuery(
    Guid UserId,
    bool? UnreadOnly = null,
    int Take = 50
) : IRequest<ErrorOr<IReadOnlyList<InAppNotificationDto>>>;

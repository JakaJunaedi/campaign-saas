namespace CampaignSaaS.Modules.Notification.Application.Commands.MarkAllNotificationsAsRead;

using ErrorOr;
using MediatR;

public record MarkAllNotificationsAsReadCommand(
    Guid UserId
) : IRequest<ErrorOr<Success>>;

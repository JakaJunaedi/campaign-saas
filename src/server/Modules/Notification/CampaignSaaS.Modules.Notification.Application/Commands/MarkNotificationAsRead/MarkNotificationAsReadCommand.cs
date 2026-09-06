namespace CampaignSaaS.Modules.Notification.Application.Commands.MarkNotificationAsRead;

using ErrorOr;
using MediatR;

public record MarkNotificationAsReadCommand(
    Guid NotificationId
) : IRequest<ErrorOr<Success>>;

namespace CampaignSaaS.Modules.Notification.Application.Commands.CreateNotification;

using CampaignSaaS.Modules.Notification.Contracts.DTOs;
using ErrorOr;
using MediatR;

public record CreateNotificationCommand(
    Guid OrganizationId,
    Guid UserId,
    string Title,
    string Message,
    string? LinkUrl = null
) : IRequest<ErrorOr<InAppNotificationDto>>;

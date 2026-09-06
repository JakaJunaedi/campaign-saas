namespace CampaignSaaS.Modules.Notification.Application.Queries.GetNotifications;

using CampaignSaaS.Modules.Notification.Application.Abstractions;
using CampaignSaaS.Modules.Notification.Contracts.DTOs;
using ErrorOr;
using MediatR;

public class GetNotificationsQueryHandler : IRequestHandler<GetNotificationsQuery, ErrorOr<IReadOnlyList<InAppNotificationDto>>>
{
    private readonly INotificationRepository _repository;

    public GetNotificationsQueryHandler(INotificationRepository repository)
    {
        _repository = repository;
    }

    public async Task<ErrorOr<IReadOnlyList<InAppNotificationDto>>> Handle(
        GetNotificationsQuery request,
        CancellationToken cancellationToken)
    {
        var notifications = await _repository.GetByUserIdAsync(
            request.UserId,
            request.UnreadOnly,
            request.Take,
            cancellationToken);

        var dtos = notifications.Select(n => new InAppNotificationDto(
            n.Id,
            n.OrganizationId,
            n.UserId,
            n.Title,
            n.Message,
            n.LinkUrl,
            n.IsRead,
            n.CreatedAt,
            n.ReadAt
        )).ToList();

        return dtos;
    }
}

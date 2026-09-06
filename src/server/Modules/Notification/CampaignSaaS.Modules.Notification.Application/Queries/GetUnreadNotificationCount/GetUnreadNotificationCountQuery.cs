namespace CampaignSaaS.Modules.Notification.Application.Queries.GetUnreadNotificationCount;

using ErrorOr;
using MediatR;

public record GetUnreadNotificationCountQuery(
    Guid UserId
) : IRequest<ErrorOr<int>>;

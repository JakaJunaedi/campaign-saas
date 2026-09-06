namespace CampaignSaaS.Modules.Notification.Application.Queries.GetUnreadNotificationCount;

using CampaignSaaS.Modules.Notification.Application.Abstractions;
using ErrorOr;
using MediatR;

public class GetUnreadNotificationCountQueryHandler : IRequestHandler<GetUnreadNotificationCountQuery, ErrorOr<int>>
{
    private readonly INotificationRepository _repository;

    public GetUnreadNotificationCountQueryHandler(INotificationRepository repository)
    {
        _repository = repository;
    }

    public async Task<ErrorOr<int>> Handle(
        GetUnreadNotificationCountQuery request,
        CancellationToken cancellationToken)
    {
        var count = await _repository.GetUnreadCountAsync(request.UserId, cancellationToken);
        return count;
    }
}

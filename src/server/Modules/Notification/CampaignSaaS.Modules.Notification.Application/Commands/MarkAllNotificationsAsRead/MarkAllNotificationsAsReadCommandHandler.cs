namespace CampaignSaaS.Modules.Notification.Application.Commands.MarkAllNotificationsAsRead;

using CampaignSaaS.Modules.Notification.Application.Abstractions;
using ErrorOr;
using MediatR;

public class MarkAllNotificationsAsReadCommandHandler : IRequestHandler<MarkAllNotificationsAsReadCommand, ErrorOr<Success>>
{
    private readonly INotificationRepository _repository;
    private readonly INotificationUnitOfWork _unitOfWork;

    public MarkAllNotificationsAsReadCommandHandler(
        INotificationRepository repository,
        INotificationUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<Success>> Handle(
        MarkAllNotificationsAsReadCommand request,
        CancellationToken cancellationToken)
    {
        await _repository.MarkAllAsReadAsync(request.UserId, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }
}

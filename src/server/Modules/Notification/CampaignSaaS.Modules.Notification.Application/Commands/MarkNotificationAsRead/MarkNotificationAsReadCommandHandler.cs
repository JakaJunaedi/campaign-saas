namespace CampaignSaaS.Modules.Notification.Application.Commands.MarkNotificationAsRead;

using CampaignSaaS.Modules.Notification.Application.Abstractions;
using ErrorOr;
using MediatR;

public class MarkNotificationAsReadCommandHandler : IRequestHandler<MarkNotificationAsReadCommand, ErrorOr<Success>>
{
    private readonly INotificationRepository _repository;
    private readonly INotificationUnitOfWork _unitOfWork;

    public MarkNotificationAsReadCommandHandler(
        INotificationRepository repository,
        INotificationUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<Success>> Handle(
        MarkNotificationAsReadCommand request,
        CancellationToken cancellationToken)
    {
        var notification = await _repository.GetByIdAsync(request.NotificationId, cancellationToken);
        if (notification == null)
        {
            return Error.NotFound("Notification.NotFound", $"Notification with ID '{request.NotificationId}' was not found.");
        }

        notification.MarkAsRead();
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }
}

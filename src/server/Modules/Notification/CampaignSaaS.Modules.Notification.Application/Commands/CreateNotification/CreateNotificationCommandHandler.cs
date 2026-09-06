namespace CampaignSaaS.Modules.Notification.Application.Commands.CreateNotification;

using CampaignSaaS.Modules.Notification.Application.Abstractions;
using CampaignSaaS.Modules.Notification.Contracts.DTOs;
using CampaignSaaS.Modules.Notification.Domain.Entities;
using ErrorOr;
using MediatR;

public class CreateNotificationCommandHandler : IRequestHandler<CreateNotificationCommand, ErrorOr<InAppNotificationDto>>
{
    private readonly INotificationRepository _repository;
    private readonly INotificationUnitOfWork _unitOfWork;

    public CreateNotificationCommandHandler(
        INotificationRepository repository,
        INotificationUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<InAppNotificationDto>> Handle(
        CreateNotificationCommand request,
        CancellationToken cancellationToken)
    {
        var notification = InAppNotification.Create(
            request.OrganizationId,
            request.UserId,
            request.Title,
            request.Message,
            request.LinkUrl);

        await _repository.AddAsync(notification, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new InAppNotificationDto(
            notification.Id,
            notification.OrganizationId,
            notification.UserId,
            notification.Title,
            notification.Message,
            notification.LinkUrl,
            notification.IsRead,
            notification.CreatedAt,
            notification.ReadAt);
    }
}

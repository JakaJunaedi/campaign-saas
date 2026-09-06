namespace CampaignSaaS.Modules.Notification.Application.Consumers;

using CampaignSaaS.Modules.Notification.Application.Abstractions;
using CampaignSaaS.Modules.Notification.Domain.Entities;
using CampaignSaaS.SharedKernel.IntegrationEvents;
using MassTransit;
using Microsoft.Extensions.Logging;

public class DeliverableNotificationConsumer :
    IConsumer<DeliverableSubmittedIntegrationEvent>
{
    private readonly INotificationRepository _repository;
    private readonly INotificationUnitOfWork _unitOfWork;
    private readonly ILogger<DeliverableNotificationConsumer> _logger;

    public DeliverableNotificationConsumer(
        INotificationRepository repository,
        INotificationUnitOfWork unitOfWork,
        ILogger<DeliverableNotificationConsumer> logger)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<DeliverableSubmittedIntegrationEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("Creating notification for deliverable submission {DeliverableId}", message.DeliverableId);

        var notification = InAppNotification.Create(
            message.OrganizationId,
            message.CreatorId,
            "Deliverable Submitted",
            $"New content submission was uploaded for deliverable {message.DeliverableId}.",
            $"/deliverables/{message.DeliverableId}");

        await _repository.AddAsync(notification, context.CancellationToken);
        await _unitOfWork.SaveChangesAsync(context.CancellationToken);
    }
}

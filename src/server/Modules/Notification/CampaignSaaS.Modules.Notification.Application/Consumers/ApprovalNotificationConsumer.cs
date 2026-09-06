namespace CampaignSaaS.Modules.Notification.Application.Consumers;

using CampaignSaaS.Modules.Notification.Application.Abstractions;
using CampaignSaaS.Modules.Notification.Domain.Entities;
using CampaignSaaS.SharedKernel.IntegrationEvents;
using MassTransit;
using Microsoft.Extensions.Logging;

public class ApprovalNotificationConsumer :
    IConsumer<ContentApprovedIntegrationEvent>,
    IConsumer<RevisionRequestedIntegrationEvent>,
    IConsumer<ContentRejectedIntegrationEvent>
{
    private readonly INotificationRepository _repository;
    private readonly INotificationUnitOfWork _unitOfWork;
    private readonly ILogger<ApprovalNotificationConsumer> _logger;

    public ApprovalNotificationConsumer(
        INotificationRepository repository,
        INotificationUnitOfWork unitOfWork,
        ILogger<ApprovalNotificationConsumer> logger)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<ContentApprovedIntegrationEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("Creating notification for approved content submission {SubmissionId}", message.ContentSubmissionId);

        var notification = InAppNotification.Create(
            message.OrganizationId,
            message.ReviewerId,
            "Content Approved",
            "Content submission has been approved.",
            $"/submissions/{message.ContentSubmissionId}");

        await _repository.AddAsync(notification, context.CancellationToken);
        await _unitOfWork.SaveChangesAsync(context.CancellationToken);
    }

    public async Task Consume(ConsumeContext<RevisionRequestedIntegrationEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("Creating notification for revision requested on submission {SubmissionId}", message.ContentSubmissionId);

        var notification = InAppNotification.Create(
            message.OrganizationId,
            message.ReviewerId,
            "Revision Requested",
            $"Revision requested: {message.FeedbackNotes}",
            $"/submissions/{message.ContentSubmissionId}");

        await _repository.AddAsync(notification, context.CancellationToken);
        await _unitOfWork.SaveChangesAsync(context.CancellationToken);
    }

    public async Task Consume(ConsumeContext<ContentRejectedIntegrationEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("Creating notification for rejected content submission {SubmissionId}", message.ContentSubmissionId);

        var notification = InAppNotification.Create(
            message.OrganizationId,
            message.ReviewerId,
            "Content Rejected",
            $"Content submission was rejected: {message.FeedbackNotes}",
            $"/submissions/{message.ContentSubmissionId}");

        await _repository.AddAsync(notification, context.CancellationToken);
        await _unitOfWork.SaveChangesAsync(context.CancellationToken);
    }
}

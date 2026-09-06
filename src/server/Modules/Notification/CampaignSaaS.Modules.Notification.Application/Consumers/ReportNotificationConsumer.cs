namespace CampaignSaaS.Modules.Notification.Application.Consumers;

using CampaignSaaS.Modules.Notification.Application.Abstractions;
using CampaignSaaS.Modules.Notification.Domain.Entities;
using CampaignSaaS.SharedKernel.IntegrationEvents;
using MassTransit;
using Microsoft.Extensions.Logging;

public class ReportNotificationConsumer :
    IConsumer<CampaignReportGeneratedIntegrationEvent>,
    IConsumer<CampaignReportFailedIntegrationEvent>
{
    private readonly INotificationRepository _repository;
    private readonly INotificationUnitOfWork _unitOfWork;
    private readonly ILogger<ReportNotificationConsumer> _logger;

    public ReportNotificationConsumer(
        INotificationRepository repository,
        INotificationUnitOfWork unitOfWork,
        ILogger<ReportNotificationConsumer> logger)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<CampaignReportGeneratedIntegrationEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("Creating notification for generated report {ReportId}", message.ReportId);

        var notification = InAppNotification.Create(
            message.OrganizationId,
            Guid.Empty, // Broadcast to campaign team/managers
            "Campaign Report Ready",
            $"PDF report for campaign {message.CampaignId} is ready.",
            message.DownloadUrl);

        await _repository.AddAsync(notification, context.CancellationToken);
        await _unitOfWork.SaveChangesAsync(context.CancellationToken);
    }

    public async Task Consume(ConsumeContext<CampaignReportFailedIntegrationEvent> context)
    {
        var message = context.Message;
        _logger.LogWarning("Creating notification for failed report {ReportId}: {Error}", message.ReportId, message.ErrorMessage);

        var notification = InAppNotification.Create(
            message.OrganizationId,
            Guid.Empty,
            "Campaign Report Failed",
            $"Report generation for campaign {message.CampaignId} failed: {message.ErrorMessage}",
            $"/campaigns/{message.CampaignId}");

        await _repository.AddAsync(notification, context.CancellationToken);
        await _unitOfWork.SaveChangesAsync(context.CancellationToken);
    }
}

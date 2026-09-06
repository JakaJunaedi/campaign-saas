namespace CampaignSaaS.Modules.Audit.Application.Consumers;

using System.Text.Json;
using CampaignSaaS.Modules.Audit.Application.Abstractions;
using CampaignSaaS.Modules.Audit.Domain.Entities;
using CampaignSaaS.SharedKernel.IntegrationEvents;
using MassTransit;
using Microsoft.Extensions.Logging;

public class AuditIntegrationEventConsumer :
    IConsumer<ContentApprovedIntegrationEvent>,
    IConsumer<RevisionRequestedIntegrationEvent>,
    IConsumer<ContentRejectedIntegrationEvent>,
    IConsumer<DeliverableSubmittedIntegrationEvent>,
    IConsumer<CampaignReportGeneratedIntegrationEvent>,
    IConsumer<CampaignReportFailedIntegrationEvent>
{
    private readonly IAuditLogRepository _repository;
    private readonly IAuditUnitOfWork _unitOfWork;
    private readonly ILogger<AuditIntegrationEventConsumer> _logger;

    public AuditIntegrationEventConsumer(
        IAuditLogRepository repository,
        IAuditUnitOfWork unitOfWork,
        ILogger<AuditIntegrationEventConsumer> logger)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<ContentApprovedIntegrationEvent> context)
    {
        var msg = context.Message;
        _logger.LogInformation("Audit: Content Approved for submission {SubmissionId}", msg.ContentSubmissionId);

        var log = AuditLog.Create(
            msg.OrganizationId,
            msg.ReviewerId,
            null,
            "ContentApproved",
            "Approval",
            "ContentSubmission",
            msg.ContentSubmissionId,
            JsonSerializer.Serialize(new { msg.ReviewId, msg.FeedbackNotes }));

        await _repository.AddAsync(log, context.CancellationToken);
        await _unitOfWork.SaveChangesAsync(context.CancellationToken);
    }

    public async Task Consume(ConsumeContext<RevisionRequestedIntegrationEvent> context)
    {
        var msg = context.Message;
        _logger.LogInformation("Audit: Revision Requested for submission {SubmissionId}", msg.ContentSubmissionId);

        var log = AuditLog.Create(
            msg.OrganizationId,
            msg.ReviewerId,
            null,
            "RevisionRequested",
            "Approval",
            "ContentSubmission",
            msg.ContentSubmissionId,
            JsonSerializer.Serialize(new { msg.ReviewId, msg.FeedbackNotes }));

        await _repository.AddAsync(log, context.CancellationToken);
        await _unitOfWork.SaveChangesAsync(context.CancellationToken);
    }

    public async Task Consume(ConsumeContext<ContentRejectedIntegrationEvent> context)
    {
        var msg = context.Message;
        _logger.LogInformation("Audit: Content Rejected for submission {SubmissionId}", msg.ContentSubmissionId);

        var log = AuditLog.Create(
            msg.OrganizationId,
            msg.ReviewerId,
            null,
            "ContentRejected",
            "Approval",
            "ContentSubmission",
            msg.ContentSubmissionId,
            JsonSerializer.Serialize(new { msg.ReviewId, msg.FeedbackNotes }));

        await _repository.AddAsync(log, context.CancellationToken);
        await _unitOfWork.SaveChangesAsync(context.CancellationToken);
    }

    public async Task Consume(ConsumeContext<DeliverableSubmittedIntegrationEvent> context)
    {
        var msg = context.Message;
        _logger.LogInformation("Audit: Deliverable Submitted for {DeliverableId}", msg.DeliverableId);

        var log = AuditLog.Create(
            msg.OrganizationId,
            msg.CreatorId,
            null,
            "DeliverableSubmitted",
            "Deliverable",
            "Deliverable",
            msg.DeliverableId,
            JsonSerializer.Serialize(new { msg.SubmissionId, msg.CampaignId, msg.CreatorId }));

        await _repository.AddAsync(log, context.CancellationToken);
        await _unitOfWork.SaveChangesAsync(context.CancellationToken);
    }

    public async Task Consume(ConsumeContext<CampaignReportGeneratedIntegrationEvent> context)
    {
        var msg = context.Message;
        _logger.LogInformation("Audit: Report Generated for campaign {CampaignId}", msg.CampaignId);

        var log = AuditLog.Create(
            msg.OrganizationId,
            null,
            null,
            "CampaignReportGenerated",
            "Reporting",
            "CampaignReport",
            msg.ReportId,
            JsonSerializer.Serialize(new { msg.CampaignId, msg.FileObjectKey, msg.DownloadUrl }));

        await _repository.AddAsync(log, context.CancellationToken);
        await _unitOfWork.SaveChangesAsync(context.CancellationToken);
    }

    public async Task Consume(ConsumeContext<CampaignReportFailedIntegrationEvent> context)
    {
        var msg = context.Message;
        _logger.LogWarning("Audit: Report Failed for campaign {CampaignId}", msg.CampaignId);

        var log = AuditLog.Create(
            msg.OrganizationId,
            null,
            null,
            "CampaignReportFailed",
            "Reporting",
            "CampaignReport",
            msg.ReportId,
            JsonSerializer.Serialize(new { msg.CampaignId, msg.ErrorMessage }));

        await _repository.AddAsync(log, context.CancellationToken);
        await _unitOfWork.SaveChangesAsync(context.CancellationToken);
    }
}

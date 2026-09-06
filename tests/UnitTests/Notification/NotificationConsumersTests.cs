namespace CampaignSaaS.UnitTests.Notification;

using CampaignSaaS.Modules.Notification.Application.Abstractions;
using CampaignSaaS.Modules.Notification.Application.Consumers;
using CampaignSaaS.Modules.Notification.Domain.Entities;
using CampaignSaaS.SharedKernel.IntegrationEvents;
using FluentAssertions;
using MassTransit;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

public class NotificationConsumersTests
{
    private readonly INotificationRepository _repository = Substitute.For<INotificationRepository>();
    private readonly INotificationUnitOfWork _unitOfWork = Substitute.For<INotificationUnitOfWork>();

    [Fact]
    public async Task ApprovalNotificationConsumer_ConsumeContentApproved_ShouldAddNotification()
    {
        // Arrange
        var logger = Substitute.For<ILogger<ApprovalNotificationConsumer>>();
        var consumer = new ApprovalNotificationConsumer(_repository, _unitOfWork, logger);

        var context = Substitute.For<ConsumeContext<ContentApprovedIntegrationEvent>>();
        context.Message.Returns(ContentApprovedIntegrationEvent.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Great work!"));

        // Act
        await consumer.Consume(context);

        // Assert
        await _repository.Received(1).AddAsync(Arg.Is<InAppNotification>(n => n.Title == "Content Approved"), Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ApprovalNotificationConsumer_ConsumeRevisionRequested_ShouldAddNotification()
    {
        // Arrange
        var logger = Substitute.For<ILogger<ApprovalNotificationConsumer>>();
        var consumer = new ApprovalNotificationConsumer(_repository, _unitOfWork, logger);

        var context = Substitute.For<ConsumeContext<RevisionRequestedIntegrationEvent>>();
        context.Message.Returns(RevisionRequestedIntegrationEvent.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Please fix audio"));

        // Act
        await consumer.Consume(context);

        // Assert
        await _repository.Received(1).AddAsync(Arg.Is<InAppNotification>(n => n.Title == "Revision Requested"), Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeliverableNotificationConsumer_ConsumeDeliverableSubmitted_ShouldAddNotification()
    {
        // Arrange
        var logger = Substitute.For<ILogger<DeliverableNotificationConsumer>>();
        var consumer = new DeliverableNotificationConsumer(_repository, _unitOfWork, logger);

        var context = Substitute.For<ConsumeContext<DeliverableSubmittedIntegrationEvent>>();
        context.Message.Returns(DeliverableSubmittedIntegrationEvent.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid()));

        // Act
        await consumer.Consume(context);

        // Assert
        await _repository.Received(1).AddAsync(Arg.Is<InAppNotification>(n => n.Title == "Deliverable Submitted"), Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ReportNotificationConsumer_ConsumeCampaignReportGenerated_ShouldAddNotification()
    {
        // Arrange
        var logger = Substitute.For<ILogger<ReportNotificationConsumer>>();
        var consumer = new ReportNotificationConsumer(_repository, _unitOfWork, logger);

        var context = Substitute.For<ConsumeContext<CampaignReportGeneratedIntegrationEvent>>();
        context.Message.Returns(CampaignReportGeneratedIntegrationEvent.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "reports/key.pdf",
            "https://minio/url"));

        // Act
        await consumer.Consume(context);

        // Assert
        await _repository.Received(1).AddAsync(Arg.Is<InAppNotification>(n => n.Title == "Campaign Report Ready"), Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}

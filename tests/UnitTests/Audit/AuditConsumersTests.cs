namespace CampaignSaaS.UnitTests.Audit;

using CampaignSaaS.Modules.Audit.Application.Abstractions;
using CampaignSaaS.Modules.Audit.Application.Consumers;
using CampaignSaaS.Modules.Audit.Domain.Entities;
using CampaignSaaS.SharedKernel.IntegrationEvents;
using FluentAssertions;
using MassTransit;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

public class AuditConsumersTests
{
    private readonly IAuditLogRepository _repository = Substitute.For<IAuditLogRepository>();
    private readonly IAuditUnitOfWork _unitOfWork = Substitute.For<IAuditUnitOfWork>();

    [Fact]
    public async Task AuditIntegrationEventConsumer_ConsumeContentApproved_ShouldAddAuditLog()
    {
        // Arrange
        var logger = Substitute.For<ILogger<AuditIntegrationEventConsumer>>();
        var consumer = new AuditIntegrationEventConsumer(_repository, _unitOfWork, logger);

        var context = Substitute.For<ConsumeContext<ContentApprovedIntegrationEvent>>();
        context.Message.Returns(ContentApprovedIntegrationEvent.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Approved notes"));

        // Act
        await consumer.Consume(context);

        // Assert
        await _repository.Received(1).AddAsync(Arg.Is<AuditLog>(a =>
            a.Action == "ContentApproved" &&
            a.Module == "Approval" &&
            a.EntityName == "ContentSubmission"), Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task AuditIntegrationEventConsumer_ConsumeDeliverableSubmitted_ShouldAddAuditLog()
    {
        // Arrange
        var logger = Substitute.For<ILogger<AuditIntegrationEventConsumer>>();
        var consumer = new AuditIntegrationEventConsumer(_repository, _unitOfWork, logger);

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
        await _repository.Received(1).AddAsync(Arg.Is<AuditLog>(a =>
            a.Action == "DeliverableSubmitted" &&
            a.Module == "Deliverable"), Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task AuditIntegrationEventConsumer_ConsumeCampaignReportGenerated_ShouldAddAuditLog()
    {
        // Arrange
        var logger = Substitute.For<ILogger<AuditIntegrationEventConsumer>>();
        var consumer = new AuditIntegrationEventConsumer(_repository, _unitOfWork, logger);

        var context = Substitute.For<ConsumeContext<CampaignReportGeneratedIntegrationEvent>>();
        context.Message.Returns(CampaignReportGeneratedIntegrationEvent.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "key.pdf",
            "https://minio/key.pdf"));

        // Act
        await consumer.Consume(context);

        // Assert
        await _repository.Received(1).AddAsync(Arg.Is<AuditLog>(a =>
            a.Action == "CampaignReportGenerated" &&
            a.Module == "Reporting"), Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}

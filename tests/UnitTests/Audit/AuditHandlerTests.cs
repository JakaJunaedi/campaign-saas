namespace CampaignSaaS.UnitTests.Audit;

using CampaignSaaS.Modules.Audit.Application.Abstractions;
using CampaignSaaS.Modules.Audit.Application.Commands.RecordAuditLog;
using CampaignSaaS.Modules.Audit.Application.Queries.GetAuditLogs;
using CampaignSaaS.Modules.Audit.Domain.Entities;
using FluentAssertions;
using NSubstitute;
using Xunit;

public class AuditHandlerTests
{
    private readonly IAuditLogRepository _repository = Substitute.For<IAuditLogRepository>();
    private readonly IAuditUnitOfWork _unitOfWork = Substitute.For<IAuditUnitOfWork>();

    [Fact]
    public async Task RecordAuditLogCommandHandler_ShouldSaveAuditLog()
    {
        // Arrange
        var handler = new RecordAuditLogCommandHandler(_repository, _unitOfWork);
        var command = new RecordAuditLogCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "user@example.com",
            "ContentApproved",
            "Approval",
            "ContentSubmission",
            Guid.NewGuid(),
            "{\"approved\":true}");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Action.Should().Be("ContentApproved");
        result.Value.Module.Should().Be("Approval");
        await _repository.Received(1).AddAsync(Arg.Any<AuditLog>(), Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetAuditLogsQueryHandler_ShouldReturnPagedLogs()
    {
        // Arrange
        var orgId = Guid.NewGuid();
        var logs = new List<AuditLog>
        {
            AuditLog.Create(orgId, null, null, "Action1", "Module1", "Entity1", Guid.NewGuid()),
            AuditLog.Create(orgId, null, null, "Action2", "Module2", "Entity2", Guid.NewGuid())
        };

        _repository.GetLogsAsync("Module1", null, null, 1, 50, Arg.Any<CancellationToken>())
            .Returns(logs);
        _repository.GetTotalCountAsync("Module1", null, null, Arg.Any<CancellationToken>())
            .Returns(2);

        var handler = new GetAuditLogsQueryHandler(_repository);
        var query = new GetAuditLogsQuery(Module: "Module1");

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Items.Should().HaveCount(2);
        result.Value.TotalCount.Should().Be(2);
        result.Value.Page.Should().Be(1);
    }
}

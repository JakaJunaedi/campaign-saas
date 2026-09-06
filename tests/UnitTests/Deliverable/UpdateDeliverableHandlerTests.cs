namespace CampaignSaaS.UnitTests.Deliverable;

using CampaignSaaS.Modules.Deliverable.Application.Abstractions;
using CampaignSaaS.Modules.Deliverable.Application.Commands.UpdateDeliverable;
using CampaignSaaS.Modules.Deliverable.Domain.Entities;
using CampaignSaaS.Modules.Deliverable.Domain.Enums;
using CampaignSaaS.SharedKernel.MultiTenancy;
using FluentAssertions;
using NSubstitute;
using Xunit;

public class UpdateDeliverableHandlerTests
{
    private readonly IDeliverableRepository _deliverableRepo = Substitute.For<IDeliverableRepository>();
    private readonly IContentSubmissionRepository _submissionRepo = Substitute.For<IContentSubmissionRepository>();
    private readonly IDeliverableUnitOfWork _uow = Substitute.For<IDeliverableUnitOfWork>();
    private readonly ICurrentTenantContext _tenantContext = Substitute.For<ICurrentTenantContext>();
    private readonly UpdateDeliverableCommandHandler _sut;

    public UpdateDeliverableHandlerTests()
    {
        _sut = new UpdateDeliverableCommandHandler(_deliverableRepo, _submissionRepo, _uow, _tenantContext);
    }

    [Fact]
    public async Task Handle_WithValidRequest_ShouldUpdateDeliverable()
    {
        // Arrange
        var orgId = Guid.NewGuid();
        _tenantContext.OrganizationId.Returns(orgId);

        var deliverable = Deliverable.Create(
            orgId,
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Old Title",
            PlatformType.TikTok,
            ContentType.DedicatedVideo,
            "Old brief",
            new DateOnly(2026, 6, 10));

        _deliverableRepo.GetByIdAsync(deliverable.Id, Arg.Any<CancellationToken>()).Returns(deliverable);
        _submissionRepo.GetLatestVersionNumberAsync(deliverable.Id, Arg.Any<CancellationToken>()).Returns(1);

        var command = new UpdateDeliverableCommand(
            deliverable.Id,
            "Updated Title",
            "TikTok",
            "DedicatedVideo",
            "Updated brief notes",
            new DateOnly(2026, 6, 20));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Title.Should().Be("Updated Title");
        result.Value.DueDate.Should().Be(new DateOnly(2026, 6, 20));

        _deliverableRepo.Received(1).Update(deliverable);
        await _uow.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenNotFound_ShouldReturnNotFoundError()
    {
        // Arrange
        _tenantContext.OrganizationId.Returns(Guid.NewGuid());
        _deliverableRepo.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((Deliverable?)null);

        var command = new UpdateDeliverableCommand(Guid.NewGuid(), "Title", "Instagram", "Reel", null, new DateOnly(2026, 6, 20));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("Deliverable.NotFound");
    }
}


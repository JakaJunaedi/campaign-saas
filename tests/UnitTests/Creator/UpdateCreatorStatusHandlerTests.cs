namespace CampaignSaaS.UnitTests.Creator;

using CampaignSaaS.Modules.Creator.Application.Abstractions;
using CampaignSaaS.Modules.Creator.Application.Commands.UpdateCreatorStatus;
using CampaignSaaS.Modules.Creator.Domain.Entities;
using CampaignSaaS.Modules.Creator.Domain.Enums;
using CampaignSaaS.SharedKernel.MultiTenancy;
using FluentAssertions;
using NSubstitute;
using Xunit;

public class UpdateCreatorStatusHandlerTests
{
    private readonly ICreatorRepository _creatorRepo = Substitute.For<ICreatorRepository>();
    private readonly ICreatorUnitOfWork _uow = Substitute.For<ICreatorUnitOfWork>();
    private readonly ICurrentTenantContext _tenantContext = Substitute.For<ICurrentTenantContext>();
    private readonly UpdateCreatorStatusCommandHandler _sut;

    public UpdateCreatorStatusHandlerTests()
    {
        _sut = new UpdateCreatorStatusCommandHandler(_creatorRepo, _uow, _tenantContext);
    }

    [Fact]
    public async Task Handle_WithValidStatus_ShouldUpdateStatus()
    {
        // Arrange
        var orgId = Guid.NewGuid();
        var creator = Creator.Create(orgId, "Deddy Corbuzier", "Podcast", status: CreatorStatus.Active);
        _tenantContext.OrganizationId.Returns(orgId);
        _creatorRepo.GetByIdAsync(creator.Id, Arg.Any<CancellationToken>()).Returns(creator);

        var command = new UpdateCreatorStatusCommand(creator.Id, "Inactive");

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Status.Should().Be("Inactive");
        creator.Status.Should().Be(CreatorStatus.Inactive);

        _creatorRepo.Received(1).Update(creator);
        await _uow.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithInvalidStatus_ShouldReturnValidationError()
    {
        // Arrange
        var orgId = Guid.NewGuid();
        _tenantContext.OrganizationId.Returns(orgId);
        var command = new UpdateCreatorStatusCommand(Guid.NewGuid(), "InvalidStatusString");

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("Creator.InvalidStatus");
    }
}

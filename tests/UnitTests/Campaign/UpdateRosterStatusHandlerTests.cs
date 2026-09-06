namespace CampaignSaaS.UnitTests.Campaign;

using CampaignSaaS.Modules.Campaign.Application.Abstractions;
using CampaignSaaS.Modules.Campaign.Application.Commands.UpdateRosterStatus;
using CampaignSaaS.Modules.Campaign.Domain.Entities;
using CampaignSaaS.Modules.Campaign.Domain.Enums;
using CampaignSaaS.SharedKernel.MultiTenancy;
using FluentAssertions;
using NSubstitute;
using Xunit;

public class UpdateRosterStatusHandlerTests
{
    private readonly ICampaignCreatorRepository _rosterRepo = Substitute.For<ICampaignCreatorRepository>();
    private readonly ICampaignUnitOfWork _uow = Substitute.For<ICampaignUnitOfWork>();
    private readonly ICurrentTenantContext _tenantContext = Substitute.For<ICurrentTenantContext>();
    private readonly UpdateRosterStatusCommandHandler _sut;

    public UpdateRosterStatusHandlerTests()
    {
        _sut = new UpdateRosterStatusCommandHandler(_rosterRepo, _uow, _tenantContext);
    }

    [Fact]
    public async Task Handle_WithValidRequest_ShouldUpdateRosterStatus()
    {
        // Arrange
        var orgId = Guid.NewGuid();
        var campaignId = Guid.NewGuid();
        var creatorId = Guid.NewGuid();
        _tenantContext.OrganizationId.Returns(orgId);

        var rosterItem = CampaignCreator.Create(orgId, campaignId, creatorId, CampaignCreatorStatus.Shortlisted, 1000m);
        _rosterRepo.GetByCampaignAndCreatorAsync(campaignId, creatorId, Arg.Any<CancellationToken>()).Returns(rosterItem);

        var command = new UpdateRosterStatusCommand(campaignId, creatorId, "Confirmed", 15000000m);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Status.Should().Be("Confirmed");
        result.Value.AgreedRate.Should().Be(15000000m);
        rosterItem.Status.Should().Be(CampaignCreatorStatus.Confirmed);

        _rosterRepo.Received(1).Update(rosterItem);
        await _uow.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithInvalidStatus_ShouldReturnValidationError()
    {
        // Arrange
        _tenantContext.OrganizationId.Returns(Guid.NewGuid());
        var command = new UpdateRosterStatusCommand(Guid.NewGuid(), Guid.NewGuid(), "InvalidStatus", null);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("CampaignRoster.InvalidStatus");
    }

    [Fact]
    public async Task Handle_WhenRosterNotFound_ShouldReturnNotFoundError()
    {
        // Arrange
        _tenantContext.OrganizationId.Returns(Guid.NewGuid());
        _rosterRepo.GetByCampaignAndCreatorAsync(Arg.Any<Guid>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((CampaignCreator?)null);

        var command = new UpdateRosterStatusCommand(Guid.NewGuid(), Guid.NewGuid(), "Confirmed", null);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("CampaignRoster.NotFound");
    }
}


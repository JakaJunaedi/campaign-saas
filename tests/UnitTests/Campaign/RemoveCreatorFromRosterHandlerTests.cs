namespace CampaignSaaS.UnitTests.Campaign;

using CampaignSaaS.Modules.Campaign.Application.Abstractions;
using CampaignSaaS.Modules.Campaign.Application.Commands.RemoveCreatorFromRoster;
using CampaignSaaS.Modules.Campaign.Domain.Entities;
using CampaignSaaS.Modules.Campaign.Domain.Enums;
using CampaignSaaS.SharedKernel.MultiTenancy;
using FluentAssertions;
using NSubstitute;
using Xunit;

public class RemoveCreatorFromRosterHandlerTests
{
    private readonly ICampaignCreatorRepository _rosterRepo = Substitute.For<ICampaignCreatorRepository>();
    private readonly ICampaignUnitOfWork _uow = Substitute.For<ICampaignUnitOfWork>();
    private readonly ICurrentTenantContext _tenantContext = Substitute.For<ICurrentTenantContext>();
    private readonly RemoveCreatorFromRosterCommandHandler _sut;

    public RemoveCreatorFromRosterHandlerTests()
    {
        _sut = new RemoveCreatorFromRosterCommandHandler(_rosterRepo, _uow, _tenantContext);
    }

    [Fact]
    public async Task Handle_WithExistingRosterItem_ShouldRemove()
    {
        // Arrange
        var orgId = Guid.NewGuid();
        var campaignId = Guid.NewGuid();
        var creatorId = Guid.NewGuid();
        _tenantContext.OrganizationId.Returns(orgId);

        var rosterItem = CampaignCreator.Create(orgId, campaignId, creatorId, CampaignCreatorStatus.Shortlisted, 1000m);
        _rosterRepo.GetByCampaignAndCreatorAsync(campaignId, creatorId, Arg.Any<CancellationToken>()).Returns(rosterItem);

        var command = new RemoveCreatorFromRosterCommand(campaignId, creatorId);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();

        _rosterRepo.Received(1).Remove(rosterItem);
        await _uow.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenRosterNotFound_ShouldReturnNotFoundError()
    {
        // Arrange
        _tenantContext.OrganizationId.Returns(Guid.NewGuid());
        _rosterRepo.GetByCampaignAndCreatorAsync(Arg.Any<Guid>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((CampaignCreator?)null);

        var command = new RemoveCreatorFromRosterCommand(Guid.NewGuid(), Guid.NewGuid());

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("CampaignRoster.NotFound");
    }
}


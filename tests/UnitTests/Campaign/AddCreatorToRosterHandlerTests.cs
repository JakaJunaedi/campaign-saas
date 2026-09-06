namespace CampaignSaaS.UnitTests.Campaign;

using CampaignSaaS.Modules.Campaign.Application.Abstractions;
using CampaignSaaS.Modules.Campaign.Application.Commands.AddCreatorToRoster;
using CampaignSaaS.Modules.Campaign.Domain.Entities;
using CampaignSaaS.Modules.Campaign.Domain.Enums;
using CampaignSaaS.SharedKernel.MultiTenancy;
using FluentAssertions;
using NSubstitute;
using Xunit;

public class AddCreatorToRosterHandlerTests
{
    private readonly ICampaignRepository _campaignRepo = Substitute.For<ICampaignRepository>();
    private readonly ICampaignCreatorRepository _rosterRepo = Substitute.For<ICampaignCreatorRepository>();
    private readonly ICampaignUnitOfWork _uow = Substitute.For<ICampaignUnitOfWork>();
    private readonly ICurrentTenantContext _tenantContext = Substitute.For<ICurrentTenantContext>();
    private readonly AddCreatorToRosterCommandHandler _sut;

    public AddCreatorToRosterHandlerTests()
    {
        _sut = new AddCreatorToRosterCommandHandler(_campaignRepo, _rosterRepo, _uow, _tenantContext);
    }

    [Fact]
    public async Task Handle_WithValidRequest_ShouldAddToRoster()
    {
        // Arrange
        var orgId = Guid.NewGuid();
        var campaignId = Guid.NewGuid();
        var creatorId = Guid.NewGuid();
        _tenantContext.OrganizationId.Returns(orgId);

        var campaign = Campaign.Create(orgId, Guid.NewGuid(), "Title", "Desc", 1000m, new DateOnly(2026, 6, 1), new DateOnly(2026, 6, 30), CampaignStatus.Draft);
        _campaignRepo.GetByIdAsync(campaignId, Arg.Any<CancellationToken>()).Returns(campaign);
        _rosterRepo.ExistsInCampaignAsync(campaignId, creatorId, Arg.Any<CancellationToken>()).Returns(false);

        var command = new AddCreatorToRosterCommand(campaignId, creatorId, 5000000m);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.CampaignId.Should().Be(campaignId);
        result.Value.CreatorId.Should().Be(creatorId);
        result.Value.AgreedRate.Should().Be(5000000m);
        result.Value.Status.Should().Be(CampaignCreatorStatus.Shortlisted.ToString());

        await _rosterRepo.Received(1).AddAsync(Arg.Is<CampaignCreator>(cc => cc.CampaignId == campaignId && cc.CreatorId == creatorId), Arg.Any<CancellationToken>());
        await _uow.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenCreatorAlreadyInRoster_ShouldReturnConflictError()
    {
        // Arrange
        var orgId = Guid.NewGuid();
        var campaignId = Guid.NewGuid();
        var creatorId = Guid.NewGuid();
        _tenantContext.OrganizationId.Returns(orgId);

        var campaign = Campaign.Create(orgId, Guid.NewGuid(), "Title", "Desc", 1000m, new DateOnly(2026, 6, 1), new DateOnly(2026, 6, 30), CampaignStatus.Draft);
        _campaignRepo.GetByIdAsync(campaignId, Arg.Any<CancellationToken>()).Returns(campaign);
        _rosterRepo.ExistsInCampaignAsync(campaignId, creatorId, Arg.Any<CancellationToken>()).Returns(true);

        var command = new AddCreatorToRosterCommand(campaignId, creatorId, 5000000m);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("CampaignRoster.DuplicateCreator");
    }

    [Fact]
    public async Task Handle_WhenCampaignNotFound_ShouldReturnNotFoundError()
    {
        // Arrange
        _tenantContext.OrganizationId.Returns(Guid.NewGuid());
        _campaignRepo.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((Campaign?)null);

        var command = new AddCreatorToRosterCommand(Guid.NewGuid(), Guid.NewGuid(), 100m);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("Campaign.NotFound");
    }
}

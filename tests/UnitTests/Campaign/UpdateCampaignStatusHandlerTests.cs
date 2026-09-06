namespace CampaignSaaS.UnitTests.Campaign;

using CampaignSaaS.Modules.Campaign.Application.Abstractions;
using CampaignSaaS.Modules.Campaign.Application.Commands.UpdateCampaignStatus;
using CampaignSaaS.Modules.Campaign.Domain.Entities;
using CampaignSaaS.Modules.Campaign.Domain.Enums;
using CampaignSaaS.SharedKernel.MultiTenancy;
using FluentAssertions;
using NSubstitute;
using Xunit;

public class UpdateCampaignStatusHandlerTests
{
    private readonly ICampaignRepository _campaignRepo = Substitute.For<ICampaignRepository>();
    private readonly ICampaignUnitOfWork _uow = Substitute.For<ICampaignUnitOfWork>();
    private readonly ICurrentTenantContext _tenantContext = Substitute.For<ICurrentTenantContext>();
    private readonly UpdateCampaignStatusCommandHandler _sut;

    public UpdateCampaignStatusHandlerTests()
    {
        _sut = new UpdateCampaignStatusCommandHandler(_campaignRepo, _uow, _tenantContext);
    }

    [Fact]
    public async Task Handle_WithValidStatus_ShouldUpdateStatus()
    {
        // Arrange
        var orgId = Guid.NewGuid();
        _tenantContext.OrganizationId.Returns(orgId);

        var campaign = Campaign.Create(orgId, Guid.NewGuid(), "Title", "Desc", 1000m, new DateOnly(2026, 6, 1), new DateOnly(2026, 6, 30), CampaignStatus.Draft);
        _campaignRepo.GetByIdAsync(campaign.Id, Arg.Any<CancellationToken>()).Returns(campaign);

        var command = new UpdateCampaignStatusCommand(campaign.Id, "Active");

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Status.Should().Be("Active");
        campaign.Status.Should().Be(CampaignStatus.Active);

        _campaignRepo.Received(1).Update(campaign);
        await _uow.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithInvalidStatus_ShouldReturnValidationError()
    {
        // Arrange
        _tenantContext.OrganizationId.Returns(Guid.NewGuid());
        var command = new UpdateCampaignStatusCommand(Guid.NewGuid(), "InvalidStatus123");

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("Campaign.InvalidStatus");
    }
}

namespace CampaignSaaS.UnitTests.Campaign;

using CampaignSaaS.Modules.Campaign.Application.Abstractions;
using CampaignSaaS.Modules.Campaign.Application.Commands.UpdateCampaign;
using CampaignSaaS.Modules.Campaign.Domain.Entities;
using CampaignSaaS.Modules.Campaign.Domain.Enums;
using CampaignSaaS.SharedKernel.MultiTenancy;
using FluentAssertions;
using NSubstitute;
using Xunit;

public class UpdateCampaignHandlerTests
{
    private readonly ICampaignRepository _campaignRepo = Substitute.For<ICampaignRepository>();
    private readonly ICampaignUnitOfWork _uow = Substitute.For<ICampaignUnitOfWork>();
    private readonly ICurrentTenantContext _tenantContext = Substitute.For<ICurrentTenantContext>();
    private readonly UpdateCampaignCommandHandler _sut;

    public UpdateCampaignHandlerTests()
    {
        _sut = new UpdateCampaignCommandHandler(_campaignRepo, _uow, _tenantContext);
    }

    [Fact]
    public async Task Handle_WithValidRequest_ShouldUpdateCampaign()
    {
        // Arrange
        var orgId = Guid.NewGuid();
        var clientId = Guid.NewGuid();
        _tenantContext.OrganizationId.Returns(orgId);

        var campaign = Campaign.Create(orgId, clientId, "Old Title", "Old Desc", 1000m, new DateOnly(2026, 6, 1), new DateOnly(2026, 6, 30), CampaignStatus.Draft);
        _campaignRepo.GetByIdAsync(campaign.Id, Arg.Any<CancellationToken>()).Returns(campaign);

        var command = new UpdateCampaignCommand(
            campaign.Id,
            "Updated Title",
            "Updated Desc",
            20000000m,
            new DateOnly(2026, 7, 1),
            new DateOnly(2026, 7, 31));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Title.Should().Be("Updated Title");
        result.Value.Budget.Should().Be(20000000m);

        _campaignRepo.Received(1).Update(campaign);
        await _uow.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenNotFound_ShouldReturnNotFoundError()
    {
        // Arrange
        _tenantContext.OrganizationId.Returns(Guid.NewGuid());
        _campaignRepo.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((Campaign?)null);

        var command = new UpdateCampaignCommand(Guid.NewGuid(), "Title", null, 100m, new DateOnly(2026, 7, 1), new DateOnly(2026, 7, 31));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("Campaign.NotFound");
    }
}

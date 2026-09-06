namespace CampaignSaaS.UnitTests.Campaign;

using CampaignSaaS.Modules.Campaign.Application.Abstractions;
using CampaignSaaS.Modules.Campaign.Application.Commands.DeleteCampaign;
using CampaignSaaS.Modules.Campaign.Domain.Entities;
using CampaignSaaS.Modules.Campaign.Domain.Enums;
using CampaignSaaS.SharedKernel.MultiTenancy;
using FluentAssertions;
using NSubstitute;
using Xunit;

public class DeleteCampaignHandlerTests
{
    private readonly ICampaignRepository _campaignRepo = Substitute.For<ICampaignRepository>();
    private readonly ICampaignUnitOfWork _uow = Substitute.For<ICampaignUnitOfWork>();
    private readonly ICurrentTenantContext _tenantContext = Substitute.For<ICurrentTenantContext>();
    private readonly DeleteCampaignCommandHandler _sut;

    public DeleteCampaignHandlerTests()
    {
        _sut = new DeleteCampaignCommandHandler(_campaignRepo, _uow, _tenantContext);
    }

    [Fact]
    public async Task Handle_WithExistingCampaign_ShouldSoftDelete()
    {
        // Arrange
        var orgId = Guid.NewGuid();
        _tenantContext.OrganizationId.Returns(orgId);

        var campaign = Campaign.Create(orgId, Guid.NewGuid(), "Title", "Desc", 1000m, new DateOnly(2026, 6, 1), new DateOnly(2026, 6, 30), CampaignStatus.Draft);
        _campaignRepo.GetByIdAsync(campaign.Id, Arg.Any<CancellationToken>()).Returns(campaign);

        var command = new DeleteCampaignCommand(campaign.Id);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        campaign.IsDeleted.Should().BeTrue();

        _campaignRepo.Received(1).Update(campaign);
        await _uow.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenNotFound_ShouldReturnNotFoundError()
    {
        // Arrange
        _tenantContext.OrganizationId.Returns(Guid.NewGuid());
        _campaignRepo.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((Campaign?)null);

        var command = new DeleteCampaignCommand(Guid.NewGuid());

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("Campaign.NotFound");
    }
}

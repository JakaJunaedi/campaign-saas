namespace CampaignSaaS.UnitTests.Campaign;

using CampaignSaaS.Modules.Campaign.Application.Abstractions;
using CampaignSaaS.Modules.Campaign.Application.Queries.GetCampaignById;
using CampaignSaaS.Modules.Campaign.Domain.Entities;
using CampaignSaaS.Modules.Campaign.Domain.Enums;
using CampaignSaaS.SharedKernel.MultiTenancy;
using FluentAssertions;
using NSubstitute;
using Xunit;

public class GetCampaignByIdQueryHandlerTests
{
    private readonly ICampaignRepository _campaignRepo = Substitute.For<ICampaignRepository>();
    private readonly ICurrentTenantContext _tenantContext = Substitute.For<ICurrentTenantContext>();
    private readonly GetCampaignByIdQueryHandler _sut;

    public GetCampaignByIdQueryHandlerTests()
    {
        _sut = new GetCampaignByIdQueryHandler(_campaignRepo, _tenantContext);
    }

    [Fact]
    public async Task Handle_WhenExists_ShouldReturnCampaignDto()
    {
        // Arrange
        var orgId = Guid.NewGuid();
        var clientId = Guid.NewGuid();
        _tenantContext.OrganizationId.Returns(orgId);

        var campaign = Campaign.Create(orgId, clientId, "Target Campaign", "Details", 15000000m, new DateOnly(2026, 6, 1), new DateOnly(2026, 6, 30), CampaignStatus.Active);
        _campaignRepo.GetByIdAsync(campaign.Id, Arg.Any<CancellationToken>()).Returns(campaign);

        var query = new GetCampaignByIdQuery(campaign.Id);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Id.Should().Be(campaign.Id);
        result.Value.Title.Should().Be("Target Campaign");
        result.Value.Status.Should().Be(CampaignStatus.Active.ToString());
    }

    [Fact]
    public async Task Handle_WhenNotFound_ShouldReturnNotFoundError()
    {
        // Arrange
        _tenantContext.OrganizationId.Returns(Guid.NewGuid());
        _campaignRepo.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((Campaign?)null);

        var query = new GetCampaignByIdQuery(Guid.NewGuid());

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("Campaign.NotFound");
    }
}

namespace CampaignSaaS.UnitTests.Campaign;

using CampaignSaaS.Modules.Campaign.Application.Abstractions;
using CampaignSaaS.Modules.Campaign.Application.Queries.GetCampaigns;
using CampaignSaaS.Modules.Campaign.Domain.Entities;
using CampaignSaaS.Modules.Campaign.Domain.Enums;
using CampaignSaaS.SharedKernel.MultiTenancy;
using FluentAssertions;
using NSubstitute;
using Xunit;

public class GetCampaignsQueryHandlerTests
{
    private readonly ICampaignRepository _campaignRepo = Substitute.For<ICampaignRepository>();
    private readonly ICurrentTenantContext _tenantContext = Substitute.For<ICurrentTenantContext>();
    private readonly GetCampaignsQueryHandler _sut;

    public GetCampaignsQueryHandlerTests()
    {
        _sut = new GetCampaignsQueryHandler(_campaignRepo, _tenantContext);
    }

    [Fact]
    public async Task Handle_WithValidRequest_ShouldReturnPagedCampaigns()
    {
        // Arrange
        var orgId = Guid.NewGuid();
        var clientId = Guid.NewGuid();
        _tenantContext.OrganizationId.Returns(orgId);

        var c1 = Campaign.Create(orgId, clientId, "Campaign Alpha", "Desc Alpha", 10000000m, new DateOnly(2026, 6, 1), new DateOnly(2026, 6, 30), CampaignStatus.Planned);
        var c2 = Campaign.Create(orgId, clientId, "Campaign Beta", "Desc Beta", 25000000m, new DateOnly(2026, 7, 1), new DateOnly(2026, 7, 31), CampaignStatus.Active);
        var items = new List<Campaign> { c1, c2 };

        _campaignRepo.GetPagedAsync(orgId, "Alpha", clientId, null, 1, 10, Arg.Any<CancellationToken>())
            .Returns((items, 2));

        _campaignRepo.GetCreatorsCountAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(3);

        var query = new GetCampaignsQuery("Alpha", clientId, null, 1, 10);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.TotalCount.Should().Be(2);
        result.Value.Items.Should().HaveCount(2);
        result.Value.Items[0].Title.Should().Be("Campaign Alpha");
        result.Value.Items[0].CreatorsCount.Should().Be(3);
    }

    [Fact]
    public async Task Handle_WithoutTenantContext_ShouldReturnForbidden()
    {
        // Arrange
        _tenantContext.OrganizationId.Returns((Guid?)null);
        var query = new GetCampaignsQuery(null, null, null, 1, 10);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("Tenant.Required");
    }
}

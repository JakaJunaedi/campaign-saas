namespace CampaignSaaS.UnitTests.Campaign;

using CampaignSaaS.Modules.Campaign.Application.Abstractions;
using CampaignSaaS.Modules.Campaign.Application.Queries.GetCampaignRoster;
using CampaignSaaS.Modules.Campaign.Domain.Entities;
using CampaignSaaS.Modules.Campaign.Domain.Enums;
using CampaignSaaS.SharedKernel.MultiTenancy;
using FluentAssertions;
using NSubstitute;
using Xunit;

public class GetCampaignRosterQueryHandlerTests
{
    private readonly ICampaignCreatorRepository _rosterRepo = Substitute.For<ICampaignCreatorRepository>();
    private readonly ICurrentTenantContext _tenantContext = Substitute.For<ICurrentTenantContext>();
    private readonly GetCampaignRosterQueryHandler _sut;

    public GetCampaignRosterQueryHandlerTests()
    {
        _sut = new GetCampaignRosterQueryHandler(_rosterRepo, _tenantContext);
    }

    [Fact]
    public async Task Handle_WithValidRequest_ShouldReturnRosterList()
    {
        // Arrange
        var orgId = Guid.NewGuid();
        var campaignId = Guid.NewGuid();
        var creator1 = Guid.NewGuid();
        var creator2 = Guid.NewGuid();
        _tenantContext.OrganizationId.Returns(orgId);

        var r1 = CampaignCreator.Create(orgId, campaignId, creator1, CampaignCreatorStatus.Confirmed, 10000000m);
        var r2 = CampaignCreator.Create(orgId, campaignId, creator2, CampaignCreatorStatus.Shortlisted, 5000000m);

        _rosterRepo.GetRosterByCampaignIdAsync(campaignId, Arg.Any<CancellationToken>())
            .Returns(new List<CampaignCreator> { r1, r2 });

        var query = new GetCampaignRosterQuery(campaignId);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().HaveCount(2);
        result.Value[0].CreatorId.Should().Be(creator1);
        result.Value[0].Status.Should().Be("Confirmed");
        result.Value[1].CreatorId.Should().Be(creator2);
        result.Value[1].Status.Should().Be("Shortlisted");
    }

    [Fact]
    public async Task Handle_WithoutTenantContext_ShouldReturnForbidden()
    {
        // Arrange
        _tenantContext.OrganizationId.Returns((Guid?)null);
        var query = new GetCampaignRosterQuery(Guid.NewGuid());

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("Tenant.Required");
    }
}


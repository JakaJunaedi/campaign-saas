namespace CampaignSaaS.UnitTests.Creator;

using CampaignSaaS.Modules.Creator.Application.Abstractions;
using CampaignSaaS.Modules.Creator.Application.Queries.GetCreators;
using CampaignSaaS.Modules.Creator.Domain.Entities;
using CampaignSaaS.Modules.Creator.Domain.Enums;
using CampaignSaaS.Modules.Creator.Domain.ValueObjects;
using CampaignSaaS.SharedKernel.MultiTenancy;
using FluentAssertions;
using NSubstitute;
using Xunit;

public class GetCreatorsQueryHandlerTests
{
    private readonly ICreatorRepository _creatorRepo = Substitute.For<ICreatorRepository>();
    private readonly ICurrentTenantContext _tenantContext = Substitute.For<ICurrentTenantContext>();
    private readonly GetCreatorsQueryHandler _sut;

    public GetCreatorsQueryHandlerTests()
    {
        _sut = new GetCreatorsQueryHandler(_creatorRepo, _tenantContext);
    }

    [Fact]
    public async Task Handle_WithFilters_ShouldReturnPagedResult()
    {
        // Arrange
        var orgId = Guid.NewGuid();
        _tenantContext.OrganizationId.Returns(orgId);

        var socials = new List<SocialAccount>
        {
            new(PlatformType.Instagram, "raditya_dika", "https://instagram.com/raditya_dika", 21000000)
        };
        var creator = Creator.Create(orgId, "Raditya Dika", "Comedy", socialAccounts: socials);
        var list = new List<Creator> { creator };

        _creatorRepo.GetPagedAsync(
            orgId,
            "Radit",
            "Comedy",
            PlatformType.Instagram,
            CreatorStatus.Active,
            1,
            10,
            Arg.Any<CancellationToken>())
            .Returns(((IReadOnlyList<Creator>)list, 1));

        var query = new GetCreatorsQuery("Radit", "Comedy", "Instagram", "Active", 1, 10);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Items.Should().HaveCount(1);
        result.Value.Items[0].FullName.Should().Be("Raditya Dika");
        result.Value.Items[0].TotalFollowers.Should().Be(21000000);
        result.Value.TotalCount.Should().Be(1);
    }
}

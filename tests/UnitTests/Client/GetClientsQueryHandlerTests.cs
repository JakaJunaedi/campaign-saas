namespace CampaignSaaS.UnitTests.Client;

using CampaignSaaS.Modules.Client.Application.Abstractions;
using CampaignSaaS.Modules.Client.Application.Queries.GetClients;
using CampaignSaaS.Modules.Client.Domain.Entities;
using CampaignSaaS.SharedKernel.MultiTenancy;
using FluentAssertions;
using NSubstitute;
using Xunit;

public class GetClientsQueryHandlerTests
{
    private readonly IClientRepository _clientRepo = Substitute.For<IClientRepository>();
    private readonly ICurrentTenantContext _tenantContext = Substitute.For<ICurrentTenantContext>();
    private readonly GetClientsQueryHandler _sut;

    public GetClientsQueryHandlerTests()
    {
        _sut = new GetClientsQueryHandler(_clientRepo, _tenantContext);
    }

    [Fact]
    public async Task Handle_WithSearchTerm_ShouldReturnPagedResult()
    {
        // Arrange
        var orgId = Guid.NewGuid();
        _tenantContext.OrganizationId.Returns(orgId);

        var clients = new List<Client>
        {
            Client.Create(orgId, "Unilever", "PT Unilever Indonesia"),
            Client.Create(orgId, "Danone", "PT Danone Indonesia")
        };

        _clientRepo.GetPagedAsync(orgId, "Indo", 1, 10, Arg.Any<CancellationToken>())
            .Returns(((IReadOnlyList<Client>)clients, 2));

        var query = new GetClientsQuery("Indo", 1, 10);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Items.Should().HaveCount(2);
        result.Value.TotalCount.Should().Be(2);
        result.Value.TotalPages.Should().Be(1);
    }
}

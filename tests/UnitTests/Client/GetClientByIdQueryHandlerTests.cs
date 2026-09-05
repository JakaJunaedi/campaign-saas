namespace CampaignSaaS.UnitTests.Client;

using CampaignSaaS.Modules.Client.Application.Abstractions;
using CampaignSaaS.Modules.Client.Application.Queries.GetClientById;
using CampaignSaaS.Modules.Client.Domain.Entities;
using CampaignSaaS.SharedKernel.MultiTenancy;
using FluentAssertions;
using NSubstitute;
using Xunit;

public class GetClientByIdQueryHandlerTests
{
    private readonly IClientRepository _clientRepo = Substitute.For<IClientRepository>();
    private readonly ICurrentTenantContext _tenantContext = Substitute.For<ICurrentTenantContext>();
    private readonly GetClientByIdQueryHandler _sut;

    public GetClientByIdQueryHandlerTests()
    {
        _sut = new GetClientByIdQueryHandler(_clientRepo, _tenantContext);
    }

    [Fact]
    public async Task Handle_WithExistingClient_ShouldReturnDto()
    {
        // Arrange
        var orgId = Guid.NewGuid();
        var client = Client.Create(orgId, "Nestle", "PT Nestle Indonesia");
        _tenantContext.OrganizationId.Returns(orgId);
        _clientRepo.GetByIdAsync(client.Id, Arg.Any<CancellationToken>()).Returns(client);

        var query = new GetClientByIdQuery(client.Id);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Name.Should().Be("Nestle");
        result.Value.CompanyName.Should().Be("PT Nestle Indonesia");
    }

    [Fact]
    public async Task Handle_WithNonExistentClient_ShouldReturnNotFound()
    {
        // Arrange
        var orgId = Guid.NewGuid();
        var clientId = Guid.NewGuid();
        _tenantContext.OrganizationId.Returns(orgId);
        _clientRepo.GetByIdAsync(clientId, Arg.Any<CancellationToken>()).Returns((Client?)null);

        var query = new GetClientByIdQuery(clientId);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("Client.NotFound");
    }
}

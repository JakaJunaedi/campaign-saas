namespace CampaignSaaS.UnitTests.Client;

using CampaignSaaS.Modules.Client.Application.Abstractions;
using CampaignSaaS.Modules.Client.Application.Commands.DeleteClient;
using CampaignSaaS.Modules.Client.Domain.Entities;
using CampaignSaaS.SharedKernel.MultiTenancy;
using FluentAssertions;
using NSubstitute;
using Xunit;

public class DeleteClientHandlerTests
{
    private readonly IClientRepository _clientRepo = Substitute.For<IClientRepository>();
    private readonly IClientUnitOfWork _uow = Substitute.For<IClientUnitOfWork>();
    private readonly ICurrentTenantContext _tenantContext = Substitute.For<ICurrentTenantContext>();
    private readonly DeleteClientCommandHandler _sut;

    public DeleteClientHandlerTests()
    {
        _sut = new DeleteClientCommandHandler(_clientRepo, _uow, _tenantContext);
    }

    [Fact]
    public async Task Handle_WithValidId_ShouldSoftDeleteClient()
    {
        // Arrange
        var orgId = Guid.NewGuid();
        var client = Client.Create(orgId, "BrandToDelete", null);
        _tenantContext.OrganizationId.Returns(orgId);
        _clientRepo.GetByIdAsync(client.Id, Arg.Any<CancellationToken>()).Returns(client);

        var command = new DeleteClientCommand(client.Id);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        client.IsDeleted.Should().BeTrue();

        _clientRepo.Received(1).Update(client);
        await _uow.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithNonExistentClient_ShouldReturnNotFound()
    {
        // Arrange
        var orgId = Guid.NewGuid();
        var clientId = Guid.NewGuid();
        _tenantContext.OrganizationId.Returns(orgId);
        _clientRepo.GetByIdAsync(clientId, Arg.Any<CancellationToken>()).Returns((Client?)null);

        var command = new DeleteClientCommand(clientId);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("Client.NotFound");
    }
}

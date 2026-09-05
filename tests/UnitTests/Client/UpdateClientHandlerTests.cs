namespace CampaignSaaS.UnitTests.Client;

using CampaignSaaS.Modules.Client.Application.Abstractions;
using CampaignSaaS.Modules.Client.Application.Commands.UpdateClient;
using CampaignSaaS.Modules.Client.Contracts.DTOs;
using CampaignSaaS.Modules.Client.Domain.Entities;
using CampaignSaaS.SharedKernel.MultiTenancy;
using FluentAssertions;
using NSubstitute;
using Xunit;

public class UpdateClientHandlerTests
{
    private readonly IClientRepository _clientRepo = Substitute.For<IClientRepository>();
    private readonly IClientUnitOfWork _uow = Substitute.For<IClientUnitOfWork>();
    private readonly ICurrentTenantContext _tenantContext = Substitute.For<ICurrentTenantContext>();
    private readonly UpdateClientCommandHandler _sut;

    public UpdateClientHandlerTests()
    {
        _sut = new UpdateClientCommandHandler(_clientRepo, _uow, _tenantContext);
    }

    [Fact]
    public async Task Handle_WithValidRequest_ShouldUpdateClient()
    {
        // Arrange
        var orgId = Guid.NewGuid();
        var client = Client.Create(orgId, "Tokopedia", "PT Tokopedia");
        _tenantContext.OrganizationId.Returns(orgId);
        _clientRepo.GetByIdAsync(client.Id, Arg.Any<CancellationToken>()).Returns(client);
        _clientRepo.ExistsByNameAsync(orgId, "Tokopedia Updated", client.Id, Arg.Any<CancellationToken>()).Returns(false);

        var contacts = new List<ClientContactDto>
        {
            new("Siti PIC", "siti@tokopedia.com", null, "Brand Manager")
        };
        var command = new UpdateClientCommand(client.Id, "Tokopedia Updated", "PT Tokopedia Corp", contacts);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Name.Should().Be("Tokopedia Updated");
        result.Value.CompanyName.Should().Be("PT Tokopedia Corp");
        result.Value.Contacts.Should().HaveCount(1);
        result.Value.Contacts[0].Position.Should().Be("Brand Manager");

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

        var command = new UpdateClientCommand(clientId, "New Name", null, null);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("Client.NotFound");
    }

    [Fact]
    public async Task Handle_WithDuplicateName_ShouldReturnConflict()
    {
        // Arrange
        var orgId = Guid.NewGuid();
        var client = Client.Create(orgId, "Shopee", "PT Shopee");
        _tenantContext.OrganizationId.Returns(orgId);
        _clientRepo.GetByIdAsync(client.Id, Arg.Any<CancellationToken>()).Returns(client);
        _clientRepo.ExistsByNameAsync(orgId, "Existing Brand", client.Id, Arg.Any<CancellationToken>()).Returns(true);

        var command = new UpdateClientCommand(client.Id, "Existing Brand", null, null);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("Client.DuplicateName");
    }
}

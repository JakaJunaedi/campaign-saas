namespace CampaignSaaS.UnitTests.Client;

using CampaignSaaS.Modules.Client.Application.Abstractions;
using CampaignSaaS.Modules.Client.Application.Commands.CreateClient;
using CampaignSaaS.Modules.Client.Contracts.DTOs;
using CampaignSaaS.Modules.Client.Domain.Entities;
using CampaignSaaS.SharedKernel.MultiTenancy;
using FluentAssertions;
using NSubstitute;
using Xunit;

public class CreateClientHandlerTests
{
    private readonly IClientRepository _clientRepo = Substitute.For<IClientRepository>();
    private readonly IClientUnitOfWork _uow = Substitute.For<IClientUnitOfWork>();
    private readonly ICurrentTenantContext _tenantContext = Substitute.For<ICurrentTenantContext>();
    private readonly CreateClientCommandHandler _sut;

    public CreateClientHandlerTests()
    {
        _sut = new CreateClientCommandHandler(_clientRepo, _uow, _tenantContext);
    }

    [Fact]
    public async Task Handle_WithValidRequest_ShouldCreateClient()
    {
        // Arrange
        var orgId = Guid.NewGuid();
        _tenantContext.OrganizationId.Returns(orgId);
        _clientRepo.ExistsByNameAsync(orgId, "Gojek", null, Arg.Any<CancellationToken>()).Returns(false);

        var contacts = new List<ClientContactDto>
        {
            new("Budi PIC", "budi@gojek.com", "+62812345678", "Marketing Lead")
        };
        var command = new CreateClientCommand("Gojek", "PT Aplikasi Karya Anak Bangsa", contacts);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Name.Should().Be("Gojek");
        result.Value.CompanyName.Should().Be("PT Aplikasi Karya Anak Bangsa");
        result.Value.Contacts.Should().HaveCount(1);
        result.Value.Contacts[0].Email.Should().Be("budi@gojek.com");

        await _clientRepo.Received(1).AddAsync(Arg.Is<Client>(c => c.Name == "Gojek" && c.OrganizationId == orgId), Arg.Any<CancellationToken>());
        await _uow.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithoutTenantContext_ShouldReturnForbidden()
    {
        // Arrange
        _tenantContext.OrganizationId.Returns((Guid?)null);
        var command = new CreateClientCommand("Gojek", null, null);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("Tenant.Required");
    }

    [Fact]
    public async Task Handle_WithDuplicateNameInSameTenant_ShouldReturnConflict()
    {
        // Arrange
        var orgId = Guid.NewGuid();
        _tenantContext.OrganizationId.Returns(orgId);
        _clientRepo.ExistsByNameAsync(orgId, "Gojek", null, Arg.Any<CancellationToken>()).Returns(true);

        var command = new CreateClientCommand("Gojek", null, null);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("Client.DuplicateName");
    }
}

namespace CampaignSaaS.UnitTests.Campaign;

using CampaignSaaS.Modules.Campaign.Application.Abstractions;
using CampaignSaaS.Modules.Campaign.Application.Commands.CreateCampaign;
using CampaignSaaS.Modules.Campaign.Domain.Entities;
using CampaignSaaS.Modules.Campaign.Domain.Enums;
using CampaignSaaS.SharedKernel.MultiTenancy;
using FluentAssertions;
using NSubstitute;
using Xunit;

public class CreateCampaignHandlerTests
{
    private readonly ICampaignRepository _campaignRepo = Substitute.For<ICampaignRepository>();
    private readonly ICampaignUnitOfWork _uow = Substitute.For<ICampaignUnitOfWork>();
    private readonly ICurrentTenantContext _tenantContext = Substitute.For<ICurrentTenantContext>();
    private readonly CreateCampaignCommandHandler _sut;

    public CreateCampaignHandlerTests()
    {
        _sut = new CreateCampaignCommandHandler(_campaignRepo, _uow, _tenantContext);
    }

    [Fact]
    public async Task Handle_WithValidRequest_ShouldCreateCampaign()
    {
        // Arrange
        var orgId = Guid.NewGuid();
        var clientId = Guid.NewGuid();
        _tenantContext.OrganizationId.Returns(orgId);

        var command = new CreateCampaignCommand(
            clientId,
            "Summer Launch 2026",
            "Summer product launch campaign",
            50000000m,
            new DateOnly(2026, 6, 1),
            new DateOnly(2026, 6, 30));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Title.Should().Be("Summer Launch 2026");
        result.Value.ClientId.Should().Be(clientId);
        result.Value.Budget.Should().Be(50000000m);
        result.Value.Status.Should().Be(CampaignStatus.Draft.ToString());

        await _campaignRepo.Received(1).AddAsync(Arg.Is<Campaign>(c => c.Title == "Summer Launch 2026" && c.OrganizationId == orgId), Arg.Any<CancellationToken>());
        await _uow.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithoutTenantContext_ShouldReturnForbidden()
    {
        // Arrange
        _tenantContext.OrganizationId.Returns((Guid?)null);
        var command = new CreateCampaignCommand(Guid.NewGuid(), "Test Campaign", null, 1000m, new DateOnly(2026, 6, 1), new DateOnly(2026, 6, 30));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("Tenant.Required");
    }
}

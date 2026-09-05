namespace CampaignSaaS.UnitTests.Creator;

using CampaignSaaS.Modules.Creator.Application.Abstractions;
using CampaignSaaS.Modules.Creator.Application.Commands.CreateCreator;
using CampaignSaaS.Modules.Creator.Contracts.DTOs;
using CampaignSaaS.Modules.Creator.Domain.Entities;
using CampaignSaaS.SharedKernel.MultiTenancy;
using FluentAssertions;
using NSubstitute;
using Xunit;

public class CreateCreatorHandlerTests
{
    private readonly ICreatorRepository _creatorRepo = Substitute.For<ICreatorRepository>();
    private readonly ICreatorUnitOfWork _uow = Substitute.For<ICreatorUnitOfWork>();
    private readonly ICurrentTenantContext _tenantContext = Substitute.For<ICurrentTenantContext>();
    private readonly CreateCreatorCommandHandler _sut;

    public CreateCreatorHandlerTests()
    {
        _sut = new CreateCreatorCommandHandler(_creatorRepo, _uow, _tenantContext);
    }

    [Fact]
    public async Task Handle_WithValidRequest_ShouldCreateCreator()
    {
        // Arrange
        var orgId = Guid.NewGuid();
        _tenantContext.OrganizationId.Returns(orgId);

        var socials = new List<SocialAccountDto>
        {
            new("Instagram", "raffi_ahmad", "https://instagram.com/raffi_ahmad", 75000000),
            new("TikTok", "raffi_official", "https://tiktok.com/@raffi_official", 50000000)
        };

        var command = new CreateCreatorCommand(
            "Raffi Ahmad",
            "Entertainment",
            "raffi@rans.com",
            "+628111222333",
            socials);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.FullName.Should().Be("Raffi Ahmad");
        result.Value.Niche.Should().Be("Entertainment");
        result.Value.SocialAccounts.Should().HaveCount(2);

        await _creatorRepo.Received(1).AddAsync(Arg.Is<Creator>(c => c.FullName == "Raffi Ahmad" && c.OrganizationId == orgId), Arg.Any<CancellationToken>());
        await _uow.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithoutTenantContext_ShouldReturnForbidden()
    {
        // Arrange
        _tenantContext.OrganizationId.Returns((Guid?)null);
        var command = new CreateCreatorCommand("Raffi Ahmad", "Entertainment", null, null, null);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("Tenant.Required");
    }
}

namespace CampaignSaaS.UnitTests.Creator;

using CampaignSaaS.Modules.Creator.Application.Abstractions;
using CampaignSaaS.Modules.Creator.Application.Commands.UpdateCreator;
using CampaignSaaS.Modules.Creator.Contracts.DTOs;
using CampaignSaaS.Modules.Creator.Domain.Entities;
using CampaignSaaS.SharedKernel.MultiTenancy;
using FluentAssertions;
using NSubstitute;
using Xunit;

public class UpdateCreatorHandlerTests
{
    private readonly ICreatorRepository _creatorRepo = Substitute.For<ICreatorRepository>();
    private readonly ICreatorUnitOfWork _uow = Substitute.For<ICreatorUnitOfWork>();
    private readonly ICurrentTenantContext _tenantContext = Substitute.For<ICurrentTenantContext>();
    private readonly UpdateCreatorCommandHandler _sut;

    public UpdateCreatorHandlerTests()
    {
        _sut = new UpdateCreatorCommandHandler(_creatorRepo, _uow, _tenantContext);
    }

    [Fact]
    public async Task Handle_WithValidRequest_ShouldUpdateCreator()
    {
        // Arrange
        var orgId = Guid.NewGuid();
        var creator = Creator.Create(orgId, "Nagita Slavina", "Lifestyle");
        _tenantContext.OrganizationId.Returns(orgId);
        _creatorRepo.GetByIdAsync(creator.Id, Arg.Any<CancellationToken>()).Returns(creator);

        var socials = new List<SocialAccountDto>
        {
            new("Instagram", "nagita_slavina", "https://instagram.com/nagita_slavina", 40000000)
        };

        var command = new UpdateCreatorCommand(
            creator.Id,
            "Nagita Slavina Updated",
            "Beauty & Lifestyle",
            "gigi@rans.com",
            "+628999888777",
            socials);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.FullName.Should().Be("Nagita Slavina Updated");
        result.Value.Niche.Should().Be("Beauty & Lifestyle");
        result.Value.SocialAccounts.Should().HaveCount(1);

        _creatorRepo.Received(1).Update(creator);
        await _uow.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithNonExistentCreator_ShouldReturnNotFound()
    {
        // Arrange
        var orgId = Guid.NewGuid();
        var creatorId = Guid.NewGuid();
        _tenantContext.OrganizationId.Returns(orgId);
        _creatorRepo.GetByIdAsync(creatorId, Arg.Any<CancellationToken>()).Returns((Creator?)null);

        var command = new UpdateCreatorCommand(creatorId, "Name", "Niche", null, null, null);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("Creator.NotFound");
    }
}

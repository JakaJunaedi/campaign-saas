namespace CampaignSaaS.UnitTests.Creator;

using CampaignSaaS.Modules.Creator.Application.Abstractions;
using CampaignSaaS.Modules.Creator.Application.Commands.DeleteCreator;
using CampaignSaaS.Modules.Creator.Domain.Entities;
using CampaignSaaS.SharedKernel.MultiTenancy;
using FluentAssertions;
using NSubstitute;
using Xunit;

public class DeleteCreatorHandlerTests
{
    private readonly ICreatorRepository _creatorRepo = Substitute.For<ICreatorRepository>();
    private readonly ICreatorUnitOfWork _uow = Substitute.For<ICreatorUnitOfWork>();
    private readonly ICurrentTenantContext _tenantContext = Substitute.For<ICurrentTenantContext>();
    private readonly DeleteCreatorCommandHandler _sut;

    public DeleteCreatorHandlerTests()
    {
        _sut = new DeleteCreatorCommandHandler(_creatorRepo, _uow, _tenantContext);
    }

    [Fact]
    public async Task Handle_WithValidId_ShouldSoftDeleteCreator()
    {
        // Arrange
        var orgId = Guid.NewGuid();
        var creator = Creator.Create(orgId, "Creator To Delete", "Gaming");
        _tenantContext.OrganizationId.Returns(orgId);
        _creatorRepo.GetByIdAsync(creator.Id, Arg.Any<CancellationToken>()).Returns(creator);

        var command = new DeleteCreatorCommand(creator.Id);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        creator.IsDeleted.Should().BeTrue();

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

        var command = new DeleteCreatorCommand(creatorId);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("Creator.NotFound");
    }
}

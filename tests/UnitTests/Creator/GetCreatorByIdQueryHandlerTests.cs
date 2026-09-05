namespace CampaignSaaS.UnitTests.Creator;

using CampaignSaaS.Modules.Creator.Application.Abstractions;
using CampaignSaaS.Modules.Creator.Application.Queries.GetCreatorById;
using CampaignSaaS.Modules.Creator.Domain.Entities;
using CampaignSaaS.SharedKernel.MultiTenancy;
using FluentAssertions;
using NSubstitute;
using Xunit;

public class GetCreatorByIdQueryHandlerTests
{
    private readonly ICreatorRepository _creatorRepo = Substitute.For<ICreatorRepository>();
    private readonly ICurrentTenantContext _tenantContext = Substitute.For<ICurrentTenantContext>();
    private readonly GetCreatorByIdQueryHandler _sut;

    public GetCreatorByIdQueryHandlerTests()
    {
        _sut = new GetCreatorByIdQueryHandler(_creatorRepo, _tenantContext);
    }

    [Fact]
    public async Task Handle_WithExistingCreator_ShouldReturnDto()
    {
        // Arrange
        var orgId = Guid.NewGuid();
        var creator = Creator.Create(orgId, "Jerome Polin", "Education");
        _tenantContext.OrganizationId.Returns(orgId);
        _creatorRepo.GetByIdAsync(creator.Id, Arg.Any<CancellationToken>()).Returns(creator);

        var query = new GetCreatorByIdQuery(creator.Id);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.FullName.Should().Be("Jerome Polin");
        result.Value.Niche.Should().Be("Education");
    }

    [Fact]
    public async Task Handle_WithNonExistentCreator_ShouldReturnNotFound()
    {
        // Arrange
        var orgId = Guid.NewGuid();
        var creatorId = Guid.NewGuid();
        _tenantContext.OrganizationId.Returns(orgId);
        _creatorRepo.GetByIdAsync(creatorId, Arg.Any<CancellationToken>()).Returns((Creator?)null);

        var query = new GetCreatorByIdQuery(creatorId);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("Creator.NotFound");
    }
}

namespace CampaignSaaS.UnitTests.Deliverable;

using CampaignSaaS.Modules.Deliverable.Application.Abstractions;
using CampaignSaaS.Modules.Deliverable.Application.Queries.GetDeliverableById;
using CampaignSaaS.Modules.Deliverable.Domain.Entities;
using CampaignSaaS.Modules.Deliverable.Domain.Enums;
using CampaignSaaS.SharedKernel.MultiTenancy;
using FluentAssertions;
using NSubstitute;
using Xunit;

public class GetDeliverableByIdQueryHandlerTests
{
    private readonly IDeliverableRepository _deliverableRepo = Substitute.For<IDeliverableRepository>();
    private readonly IContentSubmissionRepository _submissionRepo = Substitute.For<IContentSubmissionRepository>();
    private readonly ICurrentTenantContext _tenantContext = Substitute.For<ICurrentTenantContext>();
    private readonly GetDeliverableByIdQueryHandler _sut;

    public GetDeliverableByIdQueryHandlerTests()
    {
        _sut = new GetDeliverableByIdQueryHandler(_deliverableRepo, _submissionRepo, _tenantContext);
    }

    [Fact]
    public async Task Handle_WhenExists_ShouldReturnDeliverableDto()
    {
        // Arrange
        var orgId = Guid.NewGuid();
        _tenantContext.OrganizationId.Returns(orgId);

        var deliverable = Deliverable.Create(orgId, Guid.NewGuid(), Guid.NewGuid(), "Target Deliverable", PlatformType.YouTube, ContentType.DedicatedVideo, "Notes", new DateOnly(2026, 6, 25));
        _deliverableRepo.GetByIdAsync(deliverable.Id, Arg.Any<CancellationToken>()).Returns(deliverable);
        _submissionRepo.GetLatestVersionNumberAsync(deliverable.Id, Arg.Any<CancellationToken>()).Returns(2);

        var query = new GetDeliverableByIdQuery(deliverable.Id);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Id.Should().Be(deliverable.Id);
        result.Value.Title.Should().Be("Target Deliverable");
        result.Value.Platform.Should().Be("YouTube");
        result.Value.ContentType.Should().Be("DedicatedVideo");
        result.Value.LatestVersion.Should().Be(2);
    }

    [Fact]
    public async Task Handle_WhenNotFound_ShouldReturnNotFoundError()
    {
        // Arrange
        _tenantContext.OrganizationId.Returns(Guid.NewGuid());
        _deliverableRepo.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((Deliverable?)null);

        var query = new GetDeliverableByIdQuery(Guid.NewGuid());

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("Deliverable.NotFound");
    }
}


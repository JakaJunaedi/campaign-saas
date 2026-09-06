namespace CampaignSaaS.UnitTests.Deliverable;

using CampaignSaaS.Modules.Deliverable.Application.Abstractions;
using CampaignSaaS.Modules.Deliverable.Application.Queries.GetDeliverablesByCampaign;
using CampaignSaaS.Modules.Deliverable.Domain.Entities;
using CampaignSaaS.Modules.Deliverable.Domain.Enums;
using CampaignSaaS.SharedKernel.MultiTenancy;
using FluentAssertions;
using NSubstitute;
using Xunit;

public class GetDeliverablesByCampaignQueryHandlerTests
{
    private readonly IDeliverableRepository _deliverableRepo = Substitute.For<IDeliverableRepository>();
    private readonly IContentSubmissionRepository _submissionRepo = Substitute.For<IContentSubmissionRepository>();
    private readonly ICurrentTenantContext _tenantContext = Substitute.For<ICurrentTenantContext>();
    private readonly GetDeliverablesByCampaignQueryHandler _sut;

    public GetDeliverablesByCampaignQueryHandlerTests()
    {
        _sut = new GetDeliverablesByCampaignQueryHandler(_deliverableRepo, _submissionRepo, _tenantContext);
    }

    [Fact]
    public async Task Handle_WithValidRequest_ShouldReturnDeliverables()
    {
        // Arrange
        var orgId = Guid.NewGuid();
        var campaignId = Guid.NewGuid();
        _tenantContext.OrganizationId.Returns(orgId);

        var d1 = Deliverable.Create(orgId, campaignId, Guid.NewGuid(), "Item 1", PlatformType.Instagram, ContentType.Reel, null, new DateOnly(2026, 6, 15));
        var d2 = Deliverable.Create(orgId, campaignId, Guid.NewGuid(), "Item 2", PlatformType.TikTok, ContentType.FeedPost, null, new DateOnly(2026, 6, 18));

        _deliverableRepo.GetByCampaignIdAsync(campaignId, Arg.Any<CancellationToken>())
            .Returns(new List<Deliverable> { d1, d2 });

        _submissionRepo.GetLatestVersionNumberAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(1);

        var query = new GetDeliverablesByCampaignQuery(campaignId);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().HaveCount(2);
        result.Value[0].Title.Should().Be("Item 1");
        result.Value[1].Title.Should().Be("Item 2");
    }

    [Fact]
    public async Task Handle_WithoutTenantContext_ShouldReturnForbidden()
    {
        // Arrange
        _tenantContext.OrganizationId.Returns((Guid?)null);
        var query = new GetDeliverablesByCampaignQuery(Guid.NewGuid());

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("Tenant.Required");
    }
}


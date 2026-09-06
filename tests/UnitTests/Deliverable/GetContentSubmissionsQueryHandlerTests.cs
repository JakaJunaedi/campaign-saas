namespace CampaignSaaS.UnitTests.Deliverable;

using CampaignSaaS.Modules.Deliverable.Application.Abstractions;
using CampaignSaaS.Modules.Deliverable.Application.Queries.GetContentSubmissions;
using CampaignSaaS.Modules.Deliverable.Domain.Entities;
using CampaignSaaS.SharedKernel.MultiTenancy;
using FluentAssertions;
using NSubstitute;
using Xunit;

public class GetContentSubmissionsQueryHandlerTests
{
    private readonly IContentSubmissionRepository _submissionRepo = Substitute.For<IContentSubmissionRepository>();
    private readonly IStorageService _storageService = Substitute.For<IStorageService>();
    private readonly ICurrentTenantContext _tenantContext = Substitute.For<ICurrentTenantContext>();
    private readonly GetContentSubmissionsQueryHandler _sut;

    public GetContentSubmissionsQueryHandlerTests()
    {
        _sut = new GetContentSubmissionsQueryHandler(_submissionRepo, _storageService, _tenantContext);
    }

    [Fact]
    public async Task Handle_WithValidRequest_ShouldReturnSubmissions()
    {
        // Arrange
        var orgId = Guid.NewGuid();
        var deliverableId = Guid.NewGuid();
        _tenantContext.OrganizationId.Returns(orgId);

        var s1 = ContentSubmission.Create(orgId, deliverableId, 1, "key1.mp4", "file1.mp4", 10000L, "Draft 1");
        var s2 = ContentSubmission.Create(orgId, deliverableId, 2, "key2.mp4", "file2.mp4", 12000L, "Draft 2 with revisions");

        _submissionRepo.GetByDeliverableIdAsync(deliverableId, Arg.Any<CancellationToken>())
            .Returns(new List<ContentSubmission> { s1, s2 });

        _storageService.GeneratePresignedDownloadUrlAsync(Arg.Any<string>(), Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns("https://minio.local/presigned-download-url");

        var query = new GetContentSubmissionsQuery(deliverableId);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().HaveCount(2);
        result.Value[0].VersionNumber.Should().Be(1);
        result.Value[1].VersionNumber.Should().Be(2);
        result.Value[1].Caption.Should().Be("Draft 2 with revisions");
    }

    [Fact]
    public async Task Handle_WithoutTenantContext_ShouldReturnForbidden()
    {
        // Arrange
        _tenantContext.OrganizationId.Returns((Guid?)null);
        var query = new GetContentSubmissionsQuery(Guid.NewGuid());

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("Tenant.Required");
    }
}


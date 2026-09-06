namespace CampaignSaaS.UnitTests.Deliverable;

using CampaignSaaS.Modules.Deliverable.Application.Abstractions;
using CampaignSaaS.Modules.Deliverable.Application.Commands.GeneratePresignedUploadUrl;
using CampaignSaaS.Modules.Deliverable.Contracts.DTOs;
using CampaignSaaS.SharedKernel.MultiTenancy;
using FluentAssertions;
using NSubstitute;
using Xunit;

public class GeneratePresignedUploadUrlHandlerTests
{
    private readonly IStorageService _storageService = Substitute.For<IStorageService>();
    private readonly ICurrentTenantContext _tenantContext = Substitute.For<ICurrentTenantContext>();
    private readonly GeneratePresignedUploadUrlCommandHandler _sut;

    public GeneratePresignedUploadUrlHandlerTests()
    {
        _sut = new GeneratePresignedUploadUrlCommandHandler(_storageService, _tenantContext);
    }

    [Fact]
    public async Task Handle_WithValidRequest_ShouldReturnPresignedUrlDto()
    {
        // Arrange
        var orgId = Guid.NewGuid();
        var campaignId = Guid.NewGuid();
        var deliverableId = Guid.NewGuid();
        _tenantContext.OrganizationId.Returns(orgId);

        var expectedDto = new PresignedUploadUrlDto(
            "https://minio.local/upload?signature=xyz",
            $"orgs/{orgId}/campaigns/{campaignId}/{deliverableId}/drafts/video.mp4",
            15);

        _storageService.GeneratePresignedUploadUrlAsync(
            orgId,
            "video.mp4",
            "video/mp4",
            25000000L,
            campaignId,
            deliverableId,
            null,
            Arg.Any<CancellationToken>())
            .Returns(expectedDto);

        var command = new GeneratePresignedUploadUrlCommand("video.mp4", "video/mp4", 25000000L, campaignId, deliverableId);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.UploadUrl.Should().Be("https://minio.local/upload?signature=xyz");
        result.Value.ExpiresInMinutes.Should().Be(15);
    }

    [Fact]
    public async Task Handle_WithoutTenantContext_ShouldReturnForbidden()
    {
        // Arrange
        _tenantContext.OrganizationId.Returns((Guid?)null);
        var command = new GeneratePresignedUploadUrlCommand("video.mp4", "video/mp4", 1000L);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("Tenant.Required");
    }
}


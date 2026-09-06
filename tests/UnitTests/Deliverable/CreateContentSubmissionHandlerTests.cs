namespace CampaignSaaS.UnitTests.Deliverable;

using CampaignSaaS.Modules.Deliverable.Application.Abstractions;
using CampaignSaaS.Modules.Deliverable.Application.Commands.CreateContentSubmission;
using CampaignSaaS.Modules.Deliverable.Domain.Entities;
using CampaignSaaS.Modules.Deliverable.Domain.Enums;
using CampaignSaaS.SharedKernel.MultiTenancy;
using FluentAssertions;
using NSubstitute;
using Xunit;

public class CreateContentSubmissionHandlerTests
{
    private readonly IDeliverableRepository _deliverableRepo = Substitute.For<IDeliverableRepository>();
    private readonly IContentSubmissionRepository _submissionRepo = Substitute.For<IContentSubmissionRepository>();
    private readonly IStorageService _storageService = Substitute.For<IStorageService>();
    private readonly IDeliverableUnitOfWork _uow = Substitute.For<IDeliverableUnitOfWork>();
    private readonly ICurrentTenantContext _tenantContext = Substitute.For<ICurrentTenantContext>();
    private readonly CreateContentSubmissionCommandHandler _sut;

    public CreateContentSubmissionHandlerTests()
    {
        _sut = new CreateContentSubmissionCommandHandler(
            _deliverableRepo,
            _submissionRepo,
            _storageService,
            _uow,
            _tenantContext);
    }

    [Fact]
    public async Task Handle_WithValidRequest_ShouldCreateSubmissionAndIncrementVersion()
    {
        // Arrange
        var orgId = Guid.NewGuid();
        _tenantContext.OrganizationId.Returns(orgId);

        var deliverable = Deliverable.Create(
            orgId,
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Title",
            PlatformType.Instagram,
            ContentType.Reel,
            "Notes",
            new DateOnly(2026, 6, 20));

        _deliverableRepo.GetByIdAsync(deliverable.Id, Arg.Any<CancellationToken>()).Returns(deliverable);
        _submissionRepo.GetLatestVersionNumberAsync(deliverable.Id, Arg.Any<CancellationToken>()).Returns(1);
        _storageService.GeneratePresignedDownloadUrlAsync(Arg.Any<string>(), Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns("https://minio.local/presigned-download-url");

        var command = new CreateContentSubmissionCommand(
            deliverable.Id,
            "orgs/123/video_v2.mp4",
            "video_v2.mp4",
            15000000L,
            "Check out my new unboxing! #ad #collab");

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.VersionNumber.Should().Be(2);
        result.Value.MediaObjectKey.Should().Be("orgs/123/video_v2.mp4");
        result.Value.Caption.Should().Be("Check out my new unboxing! #ad #collab");
        deliverable.Status.Should().Be(DeliverableStatus.Submitted);

        await _submissionRepo.Received(1).AddAsync(Arg.Is<ContentSubmission>(s => s.VersionNumber == 2 && s.DeliverableId == deliverable.Id), Arg.Any<CancellationToken>());
        _deliverableRepo.Received(1).Update(deliverable);
        await _uow.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenDeliverableNotFound_ShouldReturnNotFoundError()
    {
        // Arrange
        _tenantContext.OrganizationId.Returns(Guid.NewGuid());
        _deliverableRepo.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((Deliverable?)null);

        var command = new CreateContentSubmissionCommand(Guid.NewGuid(), "key", "file.mp4", 1000L, null);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("Deliverable.NotFound");
    }
}


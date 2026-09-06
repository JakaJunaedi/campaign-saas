namespace CampaignSaaS.UnitTests.Deliverable;

using CampaignSaaS.Modules.Deliverable.Application.Abstractions;
using CampaignSaaS.Modules.Deliverable.Application.Commands.SubmitPublishProof;
using CampaignSaaS.Modules.Deliverable.Domain.Entities;
using CampaignSaaS.Modules.Deliverable.Domain.Enums;
using CampaignSaaS.SharedKernel.MultiTenancy;
using FluentAssertions;
using NSubstitute;
using Xunit;

public class SubmitPublishProofHandlerTests
{
    private readonly IDeliverableRepository _deliverableRepo = Substitute.For<IDeliverableRepository>();
    private readonly IContentSubmissionRepository _submissionRepo = Substitute.For<IContentSubmissionRepository>();
    private readonly IDeliverableUnitOfWork _uow = Substitute.For<IDeliverableUnitOfWork>();
    private readonly ICurrentTenantContext _tenantContext = Substitute.For<ICurrentTenantContext>();
    private readonly SubmitPublishProofCommandHandler _sut;

    public SubmitPublishProofHandlerTests()
    {
        _sut = new SubmitPublishProofCommandHandler(_deliverableRepo, _submissionRepo, _uow, _tenantContext);
    }

    [Fact]
    public async Task Handle_WithValidProof_ShouldMarkDeliverableAsPublished()
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

        var command = new SubmitPublishProofCommand(
            deliverable.Id,
            "https://www.instagram.com/reel/C8abc123/",
            "proofs/screenshot_123.jpg",
            new DateOnly(2026, 6, 21));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Status.Should().Be(DeliverableStatus.Published.ToString());
        result.Value.LiveUrl.Should().Be("https://www.instagram.com/reel/C8abc123/");
        result.Value.ProofMediaKey.Should().Be("proofs/screenshot_123.jpg");
        result.Value.PostingDate.Should().Be(new DateOnly(2026, 6, 21));

        _deliverableRepo.Received(1).Update(deliverable);
        await _uow.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenDeliverableNotFound_ShouldReturnNotFoundError()
    {
        // Arrange
        _tenantContext.OrganizationId.Returns(Guid.NewGuid());
        _deliverableRepo.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((Deliverable?)null);

        var command = new SubmitPublishProofCommand(Guid.NewGuid(), "https://instagram.com/p/123", null, null);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("Deliverable.NotFound");
    }
}


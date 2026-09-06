namespace CampaignSaaS.UnitTests.Approval;

using CampaignSaaS.Modules.Approval.Application.Abstractions;
using CampaignSaaS.Modules.Approval.Application.Commands.SubmitReview;
using CampaignSaaS.Modules.Approval.Domain.Entities;
using CampaignSaaS.Modules.Approval.Domain.Enums;
using CampaignSaaS.SharedKernel.MultiTenancy;
using FluentAssertions;
using NSubstitute;
using Xunit;

public class SubmitReviewHandlerTests
{
    private readonly IApprovalReviewRepository _reviewRepo = Substitute.For<IApprovalReviewRepository>();
    private readonly IApprovalUnitOfWork _uow = Substitute.For<IApprovalUnitOfWork>();
    private readonly ICurrentTenantContext _tenantContext = Substitute.For<ICurrentTenantContext>();
    private readonly SubmitReviewCommandHandler _sut;

    public SubmitReviewHandlerTests()
    {
        _sut = new SubmitReviewCommandHandler(
            _reviewRepo,
            _uow,
            _tenantContext);
    }

    [Fact]
    public async Task Handle_WithValidApprovedDecision_ShouldCreateReviewAndReturnDto()
    {
        // Arrange
        var orgId = Guid.NewGuid();
        _tenantContext.OrganizationId.Returns(orgId);

        var submissionId = Guid.NewGuid();
        var reviewerId = Guid.NewGuid();
        var command = new SubmitReviewCommand(
            submissionId,
            reviewerId,
            ReviewDecision.Approved,
            "Looks great! Approved.");

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.OrganizationId.Should().Be(orgId);
        result.Value.ContentSubmissionId.Should().Be(submissionId);
        result.Value.ReviewerId.Should().Be(reviewerId);
        result.Value.Decision.Should().Be("Approved");
        result.Value.FeedbackNotes.Should().Be("Looks great! Approved.");

        await _reviewRepo.Received(1).AddAsync(Arg.Is<ApprovalReview>(r =>
            r.OrganizationId == orgId &&
            r.ContentSubmissionId == submissionId &&
            r.ReviewerId == reviewerId &&
            r.Decision == ReviewDecision.Approved &&
            r.FeedbackNotes == "Looks great! Approved."),
            Arg.Any<CancellationToken>());

        await _uow.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithRevisionRequestedDecision_ShouldCreateReview()
    {
        // Arrange
        var orgId = Guid.NewGuid();
        _tenantContext.OrganizationId.Returns(orgId);

        var submissionId = Guid.NewGuid();
        var reviewerId = Guid.NewGuid();
        var command = new SubmitReviewCommand(
            submissionId,
            reviewerId,
            ReviewDecision.RevisionRequested,
            "Please fix the audio level at 0:15 and update brand logo in intro.");

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Decision.Should().Be("RevisionRequested");
        result.Value.FeedbackNotes.Should().Be("Please fix the audio level at 0:15 and update brand logo in intro.");

        await _reviewRepo.Received(1).AddAsync(Arg.Is<ApprovalReview>(r =>
            r.Decision == ReviewDecision.RevisionRequested),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithRejectedDecision_ShouldCreateReview()
    {
        // Arrange
        var orgId = Guid.NewGuid();
        _tenantContext.OrganizationId.Returns(orgId);

        var submissionId = Guid.NewGuid();
        var reviewerId = Guid.NewGuid();
        var command = new SubmitReviewCommand(
            submissionId,
            reviewerId,
            ReviewDecision.Rejected,
            "Does not comply with campaign guidelines.");

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Decision.Should().Be("Rejected");
    }

    [Fact]
    public async Task Handle_WhenNoActiveTenantContext_ShouldReturnForbiddenError()
    {
        // Arrange
        _tenantContext.OrganizationId.Returns((Guid?)null);

        var command = new SubmitReviewCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            ReviewDecision.Approved,
            null);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("Tenant.Required");
    }
}

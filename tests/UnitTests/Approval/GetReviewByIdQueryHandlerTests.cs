namespace CampaignSaaS.UnitTests.Approval;

using CampaignSaaS.Modules.Approval.Application.Abstractions;
using CampaignSaaS.Modules.Approval.Application.Queries.GetReviewById;
using CampaignSaaS.Modules.Approval.Domain.Entities;
using CampaignSaaS.Modules.Approval.Domain.Enums;
using CampaignSaaS.SharedKernel.MultiTenancy;
using FluentAssertions;
using NSubstitute;
using Xunit;

public class GetReviewByIdQueryHandlerTests
{
    private readonly IApprovalReviewRepository _reviewRepo = Substitute.For<IApprovalReviewRepository>();
    private readonly ICurrentTenantContext _tenantContext = Substitute.For<ICurrentTenantContext>();
    private readonly GetReviewByIdQueryHandler _sut;

    public GetReviewByIdQueryHandlerTests()
    {
        _sut = new GetReviewByIdQueryHandler(_reviewRepo, _tenantContext);
    }

    [Fact]
    public async Task Handle_WithExistingId_ShouldReturnReviewDto()
    {
        // Arrange
        var orgId = Guid.NewGuid();
        var reviewId = Guid.NewGuid();
        var submissionId = Guid.NewGuid();
        var reviewerId = Guid.NewGuid();
        _tenantContext.OrganizationId.Returns(orgId);

        var review = ApprovalReview.Create(orgId, submissionId, reviewerId, ReviewDecision.Approved, "Great work");
        _reviewRepo.GetByIdAsync(review.Id, Arg.Any<CancellationToken>()).Returns(review);

        var query = new GetReviewByIdQuery(review.Id);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Id.Should().Be(review.Id);
        result.Value.Decision.Should().Be("Approved");
        result.Value.FeedbackNotes.Should().Be("Great work");
    }

    [Fact]
    public async Task Handle_WhenNotFound_ShouldReturnNotFoundError()
    {
        // Arrange
        _tenantContext.OrganizationId.Returns(Guid.NewGuid());
        var nonExistentId = Guid.NewGuid();
        _reviewRepo.GetByIdAsync(nonExistentId, Arg.Any<CancellationToken>()).Returns((ApprovalReview?)null);

        var query = new GetReviewByIdQuery(nonExistentId);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("ApprovalReview.NotFound");
    }
}

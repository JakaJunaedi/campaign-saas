namespace CampaignSaaS.UnitTests.Approval;

using CampaignSaaS.Modules.Approval.Application.Abstractions;
using CampaignSaaS.Modules.Approval.Application.Queries.GetReviewsBySubmissionId;
using CampaignSaaS.Modules.Approval.Domain.Entities;
using CampaignSaaS.Modules.Approval.Domain.Enums;
using CampaignSaaS.SharedKernel.MultiTenancy;
using FluentAssertions;
using NSubstitute;
using Xunit;

public class GetReviewsBySubmissionIdQueryHandlerTests
{
    private readonly IApprovalReviewRepository _reviewRepo = Substitute.For<IApprovalReviewRepository>();
    private readonly ICurrentTenantContext _tenantContext = Substitute.For<ICurrentTenantContext>();
    private readonly GetReviewsBySubmissionIdQueryHandler _sut;

    public GetReviewsBySubmissionIdQueryHandlerTests()
    {
        _sut = new GetReviewsBySubmissionIdQueryHandler(_reviewRepo, _tenantContext);
    }

    [Fact]
    public async Task Handle_WithValidSubmissionId_ShouldReturnReviewsList()
    {
        // Arrange
        var orgId = Guid.NewGuid();
        var submissionId = Guid.NewGuid();
        _tenantContext.OrganizationId.Returns(orgId);

        var review1 = ApprovalReview.Create(orgId, submissionId, Guid.NewGuid(), ReviewDecision.RevisionRequested, "Need edits");
        var review2 = ApprovalReview.Create(orgId, submissionId, Guid.NewGuid(), ReviewDecision.Approved, "LGTM");

        _reviewRepo.GetBySubmissionIdAsync(submissionId, Arg.Any<CancellationToken>())
            .Returns(new List<ApprovalReview> { review2, review1 });

        var query = new GetReviewsBySubmissionIdQuery(submissionId);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().HaveCount(2);
        result.Value[0].Decision.Should().Be("Approved");
        result.Value[1].Decision.Should().Be("RevisionRequested");
    }

    [Fact]
    public async Task Handle_WhenNoActiveTenant_ShouldReturnForbiddenError()
    {
        // Arrange
        _tenantContext.OrganizationId.Returns((Guid?)null);
        var query = new GetReviewsBySubmissionIdQuery(Guid.NewGuid());

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("Tenant.Required");
    }
}

namespace CampaignSaaS.UnitTests.Reporting;

using CampaignSaaS.Modules.Reporting.Application.Abstractions;
using CampaignSaaS.Modules.Reporting.Application.Commands.RecordDeliverableMetrics;
using CampaignSaaS.Modules.Reporting.Domain.Entities;
using CampaignSaaS.SharedKernel.MultiTenancy;
using FluentAssertions;
using NSubstitute;
using Xunit;

public class RecordDeliverableMetricsHandlerTests
{
    private readonly IReportingMetricRepository _metricRepo = Substitute.For<IReportingMetricRepository>();
    private readonly IReportingUnitOfWork _uow = Substitute.For<IReportingUnitOfWork>();
    private readonly ICurrentTenantContext _tenantContext = Substitute.For<ICurrentTenantContext>();
    private readonly RecordDeliverableMetricsCommandHandler _sut;

    public RecordDeliverableMetricsHandlerTests()
    {
        _sut = new RecordDeliverableMetricsCommandHandler(_metricRepo, _uow, _tenantContext);
    }

    [Fact]
    public async Task Handle_WhenNewMetric_ShouldCreateAndCalculateEngagementCorrectly()
    {
        // Arrange
        var orgId = Guid.NewGuid();
        var deliverableId = Guid.NewGuid();
        _tenantContext.OrganizationId.Returns(orgId);

        _metricRepo.GetByDeliverableIdAsync(deliverableId, Arg.Any<CancellationToken>())
            .Returns((CampaignMetric?)null);

        var command = new RecordDeliverableMetricsCommand(
            deliverableId,
            Reach: 100000,
            Impressions: 150000,
            Views: 80000,
            Likes: 10000,
            Comments: 500,
            Shares: 1200,
            Clicks: 300);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Reach.Should().Be(100000);
        result.Value.Impressions.Should().Be(150000);
        result.Value.Views.Should().Be(80000);
        result.Value.Likes.Should().Be(10000);
        result.Value.Comments.Should().Be(500);
        result.Value.Shares.Should().Be(1200);
        result.Value.Clicks.Should().Be(300);
        result.Value.TotalEngagement.Should().Be(12000); // 10000 + 500 + 1200 + 300
        result.Value.EngagementRate.Should().Be(12.0); // 12000 / 100000 * 100

        await _metricRepo.Received(1).AddAsync(Arg.Is<CampaignMetric>(m =>
            m.DeliverableId == deliverableId &&
            m.TotalEngagement == 12000),
            Arg.Any<CancellationToken>());

        await _uow.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenExistingMetric_ShouldUpdateValues()
    {
        // Arrange
        var orgId = Guid.NewGuid();
        var deliverableId = Guid.NewGuid();
        _tenantContext.OrganizationId.Returns(orgId);

        var existing = CampaignMetric.Record(orgId, deliverableId, 50000, 70000, 40000, 5000, 200, 400, 100);
        _metricRepo.GetByDeliverableIdAsync(deliverableId, Arg.Any<CancellationToken>())
            .Returns(existing);

        var command = new RecordDeliverableMetricsCommand(
            deliverableId,
            Reach: 60000,
            Impressions: 80000,
            Views: 50000,
            Likes: 6000,
            Comments: 300,
            Shares: 500,
            Clicks: 200);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Reach.Should().Be(60000);
        result.Value.TotalEngagement.Should().Be(7000); // 6000 + 300 + 500 + 200

        _metricRepo.Received(1).Update(existing);
        await _uow.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenNoActiveTenant_ShouldReturnForbiddenError()
    {
        // Arrange
        _tenantContext.OrganizationId.Returns((Guid?)null);
        var command = new RecordDeliverableMetricsCommand(Guid.NewGuid(), 0, 0, 0, 0, 0, 0, 0);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("Tenant.Required");
    }
}

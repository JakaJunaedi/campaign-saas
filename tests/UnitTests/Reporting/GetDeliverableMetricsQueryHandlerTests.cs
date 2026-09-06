namespace CampaignSaaS.UnitTests.Reporting;

using CampaignSaaS.Modules.Reporting.Application.Abstractions;
using CampaignSaaS.Modules.Reporting.Application.Queries.GetDeliverableMetrics;
using CampaignSaaS.Modules.Reporting.Domain.Entities;
using CampaignSaaS.SharedKernel.MultiTenancy;
using FluentAssertions;
using NSubstitute;
using Xunit;

public class GetDeliverableMetricsQueryHandlerTests
{
    private readonly IReportingMetricRepository _metricRepo = Substitute.For<IReportingMetricRepository>();
    private readonly ICurrentTenantContext _tenantContext = Substitute.For<ICurrentTenantContext>();
    private readonly GetDeliverableMetricsQueryHandler _sut;

    public GetDeliverableMetricsQueryHandlerTests()
    {
        _sut = new GetDeliverableMetricsQueryHandler(_metricRepo, _tenantContext);
    }

    [Fact]
    public async Task Handle_WithExistingDeliverableId_ShouldReturnMetricDto()
    {
        // Arrange
        var orgId = Guid.NewGuid();
        var deliverableId = Guid.NewGuid();
        _tenantContext.OrganizationId.Returns(orgId);

        var metric = CampaignMetric.Record(orgId, deliverableId, 10000, 20000, 5000, 1000, 50, 100, 50);
        _metricRepo.GetByDeliverableIdAsync(deliverableId, Arg.Any<CancellationToken>())
            .Returns(metric);

        var query = new GetDeliverableMetricsQuery(deliverableId);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.DeliverableId.Should().Be(deliverableId);
        result.Value.TotalEngagement.Should().Be(1200);
    }

    [Fact]
    public async Task Handle_WhenNotFound_ShouldReturnNotFoundError()
    {
        // Arrange
        _tenantContext.OrganizationId.Returns(Guid.NewGuid());
        _metricRepo.GetByDeliverableIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((CampaignMetric?)null);

        var query = new GetDeliverableMetricsQuery(Guid.NewGuid());

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("CampaignMetric.NotFound");
    }
}

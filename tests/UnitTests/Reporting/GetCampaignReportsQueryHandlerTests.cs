namespace CampaignSaaS.UnitTests.Reporting;

using CampaignSaaS.Modules.Reporting.Application.Abstractions;
using CampaignSaaS.Modules.Reporting.Application.Queries.GetCampaignReports;
using CampaignSaaS.Modules.Reporting.Domain.Entities;
using CampaignSaaS.SharedKernel.MultiTenancy;
using FluentAssertions;
using NSubstitute;
using Xunit;

public class GetCampaignReportsQueryHandlerTests
{
    private readonly ICampaignReportRepository _reportRepo = Substitute.For<ICampaignReportRepository>();
    private readonly IReportStorageService _storageService = Substitute.For<IReportStorageService>();
    private readonly ICurrentTenantContext _tenantContext = Substitute.For<ICurrentTenantContext>();
    private readonly GetCampaignReportsQueryHandler _sut;

    public GetCampaignReportsQueryHandlerTests()
    {
        _sut = new GetCampaignReportsQueryHandler(_reportRepo, _storageService, _tenantContext);
    }

    [Fact]
    public async Task Handle_WithValidCampaignId_ShouldReturnReportsList()
    {
        // Arrange
        var orgId = Guid.NewGuid();
        var campaignId = Guid.NewGuid();
        _tenantContext.OrganizationId.Returns(orgId);

        var report1 = CampaignReport.RequestReport(orgId, campaignId);
        report1.MarkAsCompleted("key1.pdf");
        var report2 = CampaignReport.RequestReport(orgId, campaignId);

        _reportRepo.GetByCampaignIdAsync(campaignId, Arg.Any<CancellationToken>())
            .Returns(new List<CampaignReport> { report1, report2 });

        var query = new GetCampaignReportsQuery(campaignId);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().HaveCount(2);
        result.Value[0].Status.Should().Be("Completed");
        result.Value[1].Status.Should().Be("Pending");
    }

    [Fact]
    public async Task Handle_WhenNoActiveTenant_ShouldReturnForbiddenError()
    {
        // Arrange
        _tenantContext.OrganizationId.Returns((Guid?)null);
        var query = new GetCampaignReportsQuery(Guid.NewGuid());

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("Tenant.Required");
    }
}

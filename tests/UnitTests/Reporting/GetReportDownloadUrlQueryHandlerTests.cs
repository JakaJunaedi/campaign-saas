namespace CampaignSaaS.UnitTests.Reporting;

using CampaignSaaS.Modules.Reporting.Application.Abstractions;
using CampaignSaaS.Modules.Reporting.Application.Queries.GetReportDownloadUrl;
using CampaignSaaS.Modules.Reporting.Domain.Entities;
using CampaignSaaS.SharedKernel.MultiTenancy;
using FluentAssertions;
using NSubstitute;
using Xunit;

public class GetReportDownloadUrlQueryHandlerTests
{
    private readonly ICampaignReportRepository _reportRepo = Substitute.For<ICampaignReportRepository>();
    private readonly IReportStorageService _storageService = Substitute.For<IReportStorageService>();
    private readonly ICurrentTenantContext _tenantContext = Substitute.For<ICurrentTenantContext>();
    private readonly GetReportDownloadUrlQueryHandler _sut;

    public GetReportDownloadUrlQueryHandlerTests()
    {
        _sut = new GetReportDownloadUrlQueryHandler(_reportRepo, _storageService, _tenantContext);
    }

    [Fact]
    public async Task Handle_WhenReportCompleted_ShouldReturnDownloadUrl()
    {
        // Arrange
        var orgId = Guid.NewGuid();
        var campaignId = Guid.NewGuid();
        _tenantContext.OrganizationId.Returns(orgId);

        var report = CampaignReport.RequestReport(orgId, campaignId);
        report.MarkAsCompleted("orgs/123/reports/rep.pdf");

        _reportRepo.GetByIdAsync(report.Id, Arg.Any<CancellationToken>())
            .Returns(report);
        _storageService.GeneratePresignedDownloadUrlAsync("orgs/123/reports/rep.pdf", 60, Arg.Any<CancellationToken>())
            .Returns("https://minio.local/presigned-rep.pdf");

        var query = new GetReportDownloadUrlQuery(report.Id);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().Be("https://minio.local/presigned-rep.pdf");
    }

    [Fact]
    public async Task Handle_WhenReportNotCompletedYet_ShouldReturnNotReadyError()
    {
        // Arrange
        var orgId = Guid.NewGuid();
        var campaignId = Guid.NewGuid();
        _tenantContext.OrganizationId.Returns(orgId);

        var report = CampaignReport.RequestReport(orgId, campaignId); // Pending
        _reportRepo.GetByIdAsync(report.Id, Arg.Any<CancellationToken>())
            .Returns(report);

        var query = new GetReportDownloadUrlQuery(report.Id);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("CampaignReport.NotReady");
    }
}

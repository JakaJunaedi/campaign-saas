namespace CampaignSaaS.UnitTests.Reporting;

using CampaignSaaS.Modules.Reporting.Application.Abstractions;
using CampaignSaaS.Modules.Reporting.Application.Queries.GetReportById;
using CampaignSaaS.Modules.Reporting.Domain.Entities;
using CampaignSaaS.SharedKernel.MultiTenancy;
using FluentAssertions;
using NSubstitute;
using Xunit;

public class GetReportByIdQueryHandlerTests
{
    private readonly ICampaignReportRepository _reportRepo = Substitute.For<ICampaignReportRepository>();
    private readonly IReportStorageService _storageService = Substitute.For<IReportStorageService>();
    private readonly ICurrentTenantContext _tenantContext = Substitute.For<ICurrentTenantContext>();
    private readonly GetReportByIdQueryHandler _sut;

    public GetReportByIdQueryHandlerTests()
    {
        _sut = new GetReportByIdQueryHandler(_reportRepo, _storageService, _tenantContext);
    }

    [Fact]
    public async Task Handle_WithExistingReportId_ShouldReturnReportDto()
    {
        // Arrange
        var orgId = Guid.NewGuid();
        var campaignId = Guid.NewGuid();
        _tenantContext.OrganizationId.Returns(orgId);

        var report = CampaignReport.RequestReport(orgId, campaignId);
        report.MarkAsCompleted("reports/camp1.pdf");
        _reportRepo.GetByIdAsync(report.Id, Arg.Any<CancellationToken>())
            .Returns(report);

        var query = new GetReportByIdQuery(report.Id);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Id.Should().Be(report.Id);
        result.Value.Status.Should().Be("Completed");
        result.Value.FileObjectKey.Should().Be("reports/camp1.pdf");
    }

    [Fact]
    public async Task Handle_WhenNotFound_ShouldReturnNotFoundError()
    {
        // Arrange
        _tenantContext.OrganizationId.Returns(Guid.NewGuid());
        _reportRepo.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((CampaignReport?)null);

        var query = new GetReportByIdQuery(Guid.NewGuid());

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("CampaignReport.NotFound");
    }
}

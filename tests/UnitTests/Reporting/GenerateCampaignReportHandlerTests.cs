namespace CampaignSaaS.UnitTests.Reporting;

using CampaignSaaS.Modules.Reporting.Application.Abstractions;
using CampaignSaaS.Modules.Reporting.Application.Commands.GenerateCampaignReport;
using CampaignSaaS.Modules.Reporting.Contracts.DTOs;
using CampaignSaaS.Modules.Reporting.Domain.Entities;
using CampaignSaaS.Modules.Reporting.Domain.Enums;
using CampaignSaaS.SharedKernel.MultiTenancy;
using FluentAssertions;
using NSubstitute;
using Xunit;

public class GenerateCampaignReportHandlerTests
{
    private readonly ICampaignReportRepository _reportRepo = Substitute.For<ICampaignReportRepository>();
    private readonly IReportingUnitOfWork _uow = Substitute.For<IReportingUnitOfWork>();
    private readonly IJsReportService _jsReportService = Substitute.For<IJsReportService>();
    private readonly IReportStorageService _storageService = Substitute.For<IReportStorageService>();
    private readonly ICurrentTenantContext _tenantContext = Substitute.For<ICurrentTenantContext>();
    private readonly GenerateCampaignReportCommandHandler _sut;

    public GenerateCampaignReportHandlerTests()
    {
        _sut = new GenerateCampaignReportCommandHandler(
            _reportRepo,
            _uow,
            _jsReportService,
            _storageService,
            _tenantContext);
    }

    [Fact]
    public async Task Handle_WithValidRequest_ShouldRenderPdfUploadAndCompleteReport()
    {
        // Arrange
        var orgId = Guid.NewGuid();
        var campaignId = Guid.NewGuid();
        _tenantContext.OrganizationId.Returns(orgId);

        var pdfBytes = new byte[] { 1, 2, 3, 4, 5 };
        _jsReportService.RenderReportPdfAsync(Arg.Any<CampaignReportPayloadDto>(), Arg.Any<CancellationToken>())
            .Returns(pdfBytes);

        var objectKey = $"orgs/{orgId}/campaigns/{campaignId}/reports/report123.pdf";
        _storageService.UploadReportPdfAsync(orgId, campaignId, Arg.Any<Guid>(), pdfBytes, Arg.Any<CancellationToken>())
            .Returns(objectKey);

        _storageService.GeneratePresignedDownloadUrlAsync(objectKey, Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns("https://minio.local/presigned-report.pdf");

        var command = new GenerateCampaignReportCommand(campaignId);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.CampaignId.Should().Be(campaignId);
        result.Value.Status.Should().Be(ReportStatus.Completed.ToString());
        result.Value.FileObjectKey.Should().Be(objectKey);
        result.Value.DownloadUrl.Should().Be("https://minio.local/presigned-report.pdf");

        await _reportRepo.Received(1).AddAsync(Arg.Any<CampaignReport>(), Arg.Any<CancellationToken>());
        await _jsReportService.Received(1).RenderReportPdfAsync(Arg.Any<CampaignReportPayloadDto>(), Arg.Any<CancellationToken>());
        await _storageService.Received(1).UploadReportPdfAsync(orgId, campaignId, Arg.Any<Guid>(), pdfBytes, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenNoActiveTenant_ShouldReturnForbiddenError()
    {
        // Arrange
        _tenantContext.OrganizationId.Returns((Guid?)null);
        var command = new GenerateCampaignReportCommand(Guid.NewGuid());

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("Tenant.Required");
    }
}

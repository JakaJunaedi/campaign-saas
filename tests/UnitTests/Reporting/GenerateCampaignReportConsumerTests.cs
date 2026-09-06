namespace CampaignSaaS.UnitTests.Reporting;

using CampaignSaaS.Modules.Campaign.Contracts;
using CampaignSaaS.Modules.Campaign.Contracts.DTOs;
using CampaignSaaS.Modules.Client.Contracts;
using CampaignSaaS.Modules.Deliverable.Contracts;
using CampaignSaaS.Modules.Deliverable.Contracts.DTOs;
using CampaignSaaS.Modules.Reporting.Application.Abstractions;
using CampaignSaaS.Modules.Reporting.Application.Consumers;
using CampaignSaaS.Modules.Reporting.Contracts.DTOs;
using CampaignSaaS.Modules.Reporting.Domain.Entities;
using CampaignSaaS.Modules.Reporting.Domain.Enums;
using CampaignSaaS.SharedKernel.IntegrationEvents;
using FluentAssertions;
using MassTransit;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Xunit;

public class GenerateCampaignReportConsumerTests
{
    private readonly ICampaignReportRepository _reportRepo = Substitute.For<ICampaignReportRepository>();
    private readonly IReportingMetricRepository _metricRepo = Substitute.For<IReportingMetricRepository>();
    private readonly IReportingUnitOfWork _uow = Substitute.For<IReportingUnitOfWork>();
    private readonly IJsReportService _jsReportService = Substitute.For<IJsReportService>();
    private readonly IReportStorageService _storageService = Substitute.For<IReportStorageService>();
    private readonly ICampaignSummaryQueryService _campaignQueryService = Substitute.For<ICampaignSummaryQueryService>();
    private readonly IClientSummaryQueryService _clientQueryService = Substitute.For<IClientSummaryQueryService>();
    private readonly IDeliverableSummaryQueryService _deliverableQueryService = Substitute.For<IDeliverableSummaryQueryService>();
    private readonly IPublishEndpoint _publishEndpoint = Substitute.For<IPublishEndpoint>();
    private readonly GenerateCampaignReportConsumer _sut;

    public GenerateCampaignReportConsumerTests()
    {
        _sut = new GenerateCampaignReportConsumer(
            _reportRepo,
            _metricRepo,
            _uow,
            _jsReportService,
            _storageService,
            _campaignQueryService,
            _clientQueryService,
            _deliverableQueryService,
            _publishEndpoint,
            NullLogger<GenerateCampaignReportConsumer>.Instance);
    }

    [Fact]
    public async Task Consume_WithValidJob_ShouldAggregateDataRenderPdfUploadAndPublishCompletedEvent()
    {
        // Arrange
        var orgId = Guid.NewGuid();
        var campaignId = Guid.NewGuid();
        var report = CampaignReport.RequestReport(orgId, campaignId);

        _reportRepo.GetByIdAsync(report.Id, Arg.Any<CancellationToken>()).Returns(report);

        var campaignDto = new CampaignDto(
            campaignId,
            orgId,
            Guid.NewGuid(),
            "Summer Launch Campaign",
            "Description",
            50000000m,
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-10)),
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10)),
            "Active",
            DateTimeOffset.UtcNow,
            null);
        _campaignQueryService.GetCampaignByIdAsync(orgId, campaignId, Arg.Any<CancellationToken>()).Returns(campaignDto);

        _clientQueryService.GetClientNameAsync(orgId, campaignDto.ClientId, Arg.Any<CancellationToken>()).Returns("Brand Client A");

        var deliverableId = Guid.NewGuid();
        var deliverables = new List<DeliverableDto>
        {
            new(deliverableId, orgId, campaignId, Guid.NewGuid(), "Instagram Reel Review", "Instagram", "Reel", "Notes", DateOnly.FromDateTime(DateTime.UtcNow), null, "Published", "https://instagram.com/p/123", null, 1, DateTimeOffset.UtcNow, null)
        };
        _deliverableQueryService.GetDeliverablesByCampaignIdAsync(orgId, campaignId, Arg.Any<CancellationToken>()).Returns(deliverables);

        var metric = CampaignMetric.Record(orgId, deliverableId, 100000, 120000, 80000, 5000, 300, 200, 100);
        _metricRepo.GetByDeliverableIdsAsync(Arg.Any<IEnumerable<Guid>>(), Arg.Any<CancellationToken>()).Returns(new List<CampaignMetric> { metric });

        var pdfBytes = new byte[] { 10, 20, 30, 40 };
        _jsReportService.RenderReportPdfAsync(Arg.Any<CampaignReportPayloadDto>(), Arg.Any<CancellationToken>()).Returns(pdfBytes);

        var objectKey = $"reports/{orgId}/{campaignId}/{report.Id}.pdf";
        _storageService.UploadReportPdfAsync(orgId, campaignId, report.Id, pdfBytes, Arg.Any<CancellationToken>()).Returns(objectKey);
        _storageService.GeneratePresignedDownloadUrlAsync(objectKey, 60, Arg.Any<CancellationToken>()).Returns("https://minio.local/report.pdf");

        var context = Substitute.For<ConsumeContext<GenerateCampaignReportJob>>();
        context.Message.Returns(new GenerateCampaignReportJob(report.Id, orgId, campaignId, DateTime.UtcNow));
        context.CancellationToken.Returns(CancellationToken.None);

        // Act
        await _sut.Consume(context);

        // Assert
        report.Status.Should().Be(ReportStatus.Completed);
        report.FileObjectKey.Should().Be(objectKey);

        await _jsReportService.Received(1).RenderReportPdfAsync(
            Arg.Is<CampaignReportPayloadDto>(p =>
                p.Campaign.Title == "Summer Launch Campaign" &&
                p.Campaign.ClientName == "Brand Client A" &&
                p.SummaryMetrics.TotalReach == 100000 &&
                p.SummaryMetrics.TotalViews == 80000),
            Arg.Any<CancellationToken>());

        await _storageService.Received(1).UploadReportPdfAsync(orgId, campaignId, report.Id, pdfBytes, Arg.Any<CancellationToken>());

        await _publishEndpoint.Received(1).Publish(
            Arg.Is<CampaignReportGeneratedIntegrationEvent>(e =>
                e.ReportId == report.Id &&
                e.FileObjectKey == objectKey &&
                e.OrganizationId == orgId),
            Arg.Any<CancellationToken>());
    }
}

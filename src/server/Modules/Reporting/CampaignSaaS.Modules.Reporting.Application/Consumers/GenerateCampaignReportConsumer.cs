namespace CampaignSaaS.Modules.Reporting.Application.Consumers;

using CampaignSaaS.Modules.Campaign.Contracts;
using CampaignSaaS.Modules.Client.Contracts;
using CampaignSaaS.Modules.Deliverable.Contracts;
using CampaignSaaS.Modules.Reporting.Application.Abstractions;
using CampaignSaaS.Modules.Reporting.Contracts.DTOs;
using CampaignSaaS.SharedKernel.IntegrationEvents;
using MassTransit;
using Microsoft.Extensions.Logging;

public class GenerateCampaignReportConsumer : IConsumer<GenerateCampaignReportJob>
{
    private readonly ICampaignReportRepository _reportRepository;
    private readonly IReportingMetricRepository _metricRepository;
    private readonly IReportingUnitOfWork _unitOfWork;
    private readonly IJsReportService _jsReportService;
    private readonly IReportStorageService _storageService;
    private readonly ICampaignSummaryQueryService _campaignQueryService;
    private readonly IClientSummaryQueryService _clientQueryService;
    private readonly IDeliverableSummaryQueryService _deliverableQueryService;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<GenerateCampaignReportConsumer> _logger;

    public GenerateCampaignReportConsumer(
        ICampaignReportRepository reportRepository,
        IReportingMetricRepository metricRepository,
        IReportingUnitOfWork unitOfWork,
        IJsReportService jsReportService,
        IReportStorageService storageService,
        ICampaignSummaryQueryService campaignQueryService,
        IClientSummaryQueryService clientQueryService,
        IDeliverableSummaryQueryService deliverableQueryService,
        IPublishEndpoint publishEndpoint,
        ILogger<GenerateCampaignReportConsumer> logger)
    {
        _reportRepository = reportRepository;
        _metricRepository = metricRepository;
        _unitOfWork = unitOfWork;
        _jsReportService = jsReportService;
        _storageService = storageService;
        _campaignQueryService = campaignQueryService;
        _clientQueryService = clientQueryService;
        _deliverableQueryService = deliverableQueryService;
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<GenerateCampaignReportJob> context)
    {
        var message = context.Message;
        _logger.LogInformation("Processing GenerateCampaignReportJob for ReportId: {ReportId}, CampaignId: {CampaignId}",
            message.ReportId, message.CampaignId);

        var report = await _reportRepository.GetByIdAsync(message.ReportId, context.CancellationToken);
        if (report == null)
        {
            _logger.LogWarning("CampaignReport not found for ReportId: {ReportId}", message.ReportId);
            return;
        }

        try
        {
            report.MarkAsGenerating();
            _reportRepository.Update(report);
            await _unitOfWork.SaveChangesAsync(context.CancellationToken);

            var campaign = await _campaignQueryService.GetCampaignByIdAsync(message.OrganizationId, message.CampaignId, context.CancellationToken);
            var campaignTitle = campaign?.Title ?? "Campaign Report";
            var startDate = campaign?.StartDate ?? DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(-1));
            var endDate = campaign?.EndDate ?? DateOnly.FromDateTime(DateTime.UtcNow);
            var totalBudget = campaign?.Budget ?? 0;

            string clientName = "Client";
            if (campaign != null)
            {
                var fetchedClientName = await _clientQueryService.GetClientNameAsync(message.OrganizationId, campaign.ClientId, context.CancellationToken);
                if (!string.IsNullOrWhiteSpace(fetchedClientName))
                {
                    clientName = fetchedClientName;
                }
            }

            var deliverables = await _deliverableQueryService.GetDeliverablesByCampaignIdAsync(message.OrganizationId, message.CampaignId, context.CancellationToken);

            var deliverableIds = deliverables.Select(d => d.Id).ToList();
            var metrics = await _metricRepository.GetByDeliverableIdsAsync(deliverableIds, context.CancellationToken);
            var metricMap = metrics.ToDictionary(m => m.DeliverableId);

            long totalReach = 0;
            long totalImpressions = 0;
            long totalViews = 0;
            long totalEngagement = 0;

            var deliverableMetricItems = new List<DeliverableMetricItem>();
            foreach (var d in deliverables)
            {
                metricMap.TryGetValue(d.Id, out var m);
                var reach = m?.Reach ?? 0;
                var views = m?.Views ?? 0;
                var likes = m?.Likes ?? 0;
                var comments = m?.Comments ?? 0;
                var shares = m?.Shares ?? 0;
                var engagement = m?.TotalEngagement ?? (likes + comments + shares);
                var engagementRate = m?.CalculateEngagementRate() ?? 0.0;

                totalReach += reach;
                totalImpressions += m?.Impressions ?? 0;
                totalViews += views;
                totalEngagement += engagement;

                deliverableMetricItems.Add(new DeliverableMetricItem(
                    d.Title,
                    d.Platform,
                    d.ContentType,
                    d.LiveUrl,
                    reach,
                    views,
                    likes,
                    comments,
                    shares,
                    engagement,
                    engagementRate));
            }

            double avgEngagementRate = totalReach > 0 ? Math.Round((double)totalEngagement / totalReach * 100.0, 2) : 0.0;
            decimal cpe = totalEngagement > 0 ? Math.Round(totalBudget / totalEngagement, 2) : 0;
            decimal cpv = totalViews > 0 ? Math.Round(totalBudget / totalViews, 2) : 0;

            var summaryMetrics = new SummaryMetricsInfo(
                deliverables.Select(d => d.CampaignCreatorId).Distinct().Count(),
                deliverables.Count,
                totalReach,
                totalImpressions,
                totalViews,
                totalEngagement,
                avgEngagementRate,
                cpe,
                cpv);

            var reportPayload = new CampaignReportPayloadDto(
                new OrganizationInfo("Agency Organization", null),
                new CampaignInfo(campaignTitle, clientName, startDate, endDate, totalBudget, "IDR"),
                summaryMetrics,
                deliverableMetricItems);

            var pdfBytes = await _jsReportService.RenderReportPdfAsync(reportPayload, context.CancellationToken);

            var objectKey = await _storageService.UploadReportPdfAsync(message.OrganizationId, message.CampaignId, report.Id, pdfBytes, context.CancellationToken);

            report.MarkAsCompleted(objectKey);
            _reportRepository.Update(report);
            await _unitOfWork.SaveChangesAsync(context.CancellationToken);

            string downloadUrl = $"/api/v1/files/download?key={Uri.EscapeDataString(objectKey)}";
            try
            {
                downloadUrl = await _storageService.GeneratePresignedDownloadUrlAsync(objectKey, 60, context.CancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Could not generate presigned download URL for {ObjectKey}", objectKey);
            }

            await _publishEndpoint.Publish(CampaignReportGeneratedIntegrationEvent.Create(
                message.OrganizationId,
                message.CampaignId,
                report.Id,
                objectKey,
                downloadUrl), context.CancellationToken);

            _logger.LogInformation("Successfully generated report {ReportId} for Campaign {CampaignId}", report.Id, message.CampaignId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate report {ReportId} for Campaign {CampaignId}", message.ReportId, message.CampaignId);
            report.MarkAsFailed(ex.Message);
            _reportRepository.Update(report);
            await _unitOfWork.SaveChangesAsync(context.CancellationToken);

            await _publishEndpoint.Publish(CampaignReportFailedIntegrationEvent.Create(
                message.OrganizationId,
                message.CampaignId,
                report.Id,
                ex.Message), context.CancellationToken);
        }
    }
}

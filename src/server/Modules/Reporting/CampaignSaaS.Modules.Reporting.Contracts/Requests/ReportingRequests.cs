namespace CampaignSaaS.Modules.Reporting.Contracts.Requests;

public record RecordDeliverableMetricsRequest(
    long Reach,
    long Impressions,
    long Views,
    long Likes,
    long Comments,
    long Shares,
    long Clicks);

public record GenerateCampaignReportRequest(
    Guid? CampaignId = null);

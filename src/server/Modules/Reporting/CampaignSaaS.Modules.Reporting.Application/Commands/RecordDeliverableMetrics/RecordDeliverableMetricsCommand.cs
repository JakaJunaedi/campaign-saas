namespace CampaignSaaS.Modules.Reporting.Application.Commands.RecordDeliverableMetrics;

using CampaignSaaS.Modules.Reporting.Contracts.DTOs;
using ErrorOr;
using MediatR;

public record RecordDeliverableMetricsCommand(
    Guid DeliverableId,
    long Reach,
    long Impressions,
    long Views,
    long Likes,
    long Comments,
    long Shares,
    long Clicks) : IRequest<ErrorOr<CampaignMetricDto>>;

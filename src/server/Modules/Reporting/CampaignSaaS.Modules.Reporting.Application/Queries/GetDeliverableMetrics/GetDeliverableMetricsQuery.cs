namespace CampaignSaaS.Modules.Reporting.Application.Queries.GetDeliverableMetrics;

using CampaignSaaS.Modules.Reporting.Contracts.DTOs;
using ErrorOr;
using MediatR;

public record GetDeliverableMetricsQuery(Guid DeliverableId) : IRequest<ErrorOr<CampaignMetricDto>>;

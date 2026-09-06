namespace CampaignSaaS.Modules.Reporting.Application.Queries.GetCampaignReports;

using CampaignSaaS.Modules.Reporting.Contracts.DTOs;
using ErrorOr;
using MediatR;

public record GetCampaignReportsQuery(Guid CampaignId) : IRequest<ErrorOr<IReadOnlyList<CampaignReportDto>>>;

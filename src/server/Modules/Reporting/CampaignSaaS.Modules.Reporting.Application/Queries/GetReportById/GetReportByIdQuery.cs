namespace CampaignSaaS.Modules.Reporting.Application.Queries.GetReportById;

using CampaignSaaS.Modules.Reporting.Contracts.DTOs;
using ErrorOr;
using MediatR;

public record GetReportByIdQuery(Guid Id) : IRequest<ErrorOr<CampaignReportDto>>;

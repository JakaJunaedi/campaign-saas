namespace CampaignSaaS.Modules.Reporting.Application.Commands.GenerateCampaignReport;

using CampaignSaaS.Modules.Reporting.Contracts.DTOs;
using ErrorOr;
using MediatR;

public record GenerateCampaignReportCommand(
    Guid CampaignId) : IRequest<ErrorOr<CampaignReportDto>>;

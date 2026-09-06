namespace CampaignSaaS.Modules.Reporting.Application.Queries.GetReportDownloadUrl;

using ErrorOr;
using MediatR;

public record GetReportDownloadUrlQuery(Guid Id) : IRequest<ErrorOr<string>>;

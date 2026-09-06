namespace CampaignSaaS.Modules.Audit.Application.Queries.GetAuditLogs;

using CampaignSaaS.Modules.Audit.Contracts.DTOs;
using ErrorOr;
using MediatR;

public record GetAuditLogsQuery(
    string? Module = null,
    string? Action = null,
    Guid? EntityId = null,
    int Page = 1,
    int PageSize = 50
) : IRequest<ErrorOr<PagedResult<AuditLogDto>>>;

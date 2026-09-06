namespace CampaignSaaS.Modules.Audit.Application.Queries.GetAuditLogs;

using CampaignSaaS.Modules.Audit.Application.Abstractions;
using CampaignSaaS.Modules.Audit.Contracts.DTOs;
using ErrorOr;
using MediatR;

public class GetAuditLogsQueryHandler : IRequestHandler<GetAuditLogsQuery, ErrorOr<PagedResult<AuditLogDto>>>
{
    private readonly IAuditLogRepository _repository;

    public GetAuditLogsQueryHandler(IAuditLogRepository repository)
    {
        _repository = repository;
    }

    public async Task<ErrorOr<PagedResult<AuditLogDto>>> Handle(
        GetAuditLogsQuery request,
        CancellationToken cancellationToken)
    {
        var logs = await _repository.GetLogsAsync(
            request.Module,
            request.Action,
            request.EntityId,
            request.Page,
            request.PageSize,
            cancellationToken);

        var totalCount = await _repository.GetTotalCountAsync(
            request.Module,
            request.Action,
            request.EntityId,
            cancellationToken);

        var dtos = logs.Select(l => new AuditLogDto(
            l.Id,
            l.OrganizationId,
            l.UserId,
            l.ActorEmail,
            l.Action,
            l.Module,
            l.EntityName,
            l.EntityId,
            l.ChangesJson,
            l.IpAddress,
            l.Timestamp
        )).ToList();

        return new PagedResult<AuditLogDto>(dtos, totalCount, request.Page, request.PageSize);
    }
}

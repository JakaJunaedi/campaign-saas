namespace CampaignSaaS.Modules.Audit.Application.Abstractions;

using CampaignSaaS.Modules.Audit.Domain.Entities;

public interface IAuditLogRepository
{
    Task<AuditLog?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AuditLog>> GetLogsAsync(
        string? module = null,
        string? action = null,
        Guid? entityId = null,
        int page = 1,
        int pageSize = 50,
        CancellationToken cancellationToken = default);
    Task<int> GetTotalCountAsync(
        string? module = null,
        string? action = null,
        Guid? entityId = null,
        CancellationToken cancellationToken = default);
    Task AddAsync(AuditLog log, CancellationToken cancellationToken = default);
}

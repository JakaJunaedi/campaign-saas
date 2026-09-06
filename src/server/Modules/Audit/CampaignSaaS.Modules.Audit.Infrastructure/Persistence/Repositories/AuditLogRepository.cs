namespace CampaignSaaS.Modules.Audit.Infrastructure.Persistence.Repositories;

using CampaignSaaS.Modules.Audit.Application.Abstractions;
using CampaignSaaS.Modules.Audit.Domain.Entities;
using CampaignSaaS.Modules.Audit.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

public class AuditLogRepository : IAuditLogRepository
{
    private readonly AuditDbContext _dbContext;

    public AuditLogRepository(AuditDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<AuditLog?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.AuditLogs
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<AuditLog>> GetLogsAsync(
        string? module = null,
        string? action = null,
        Guid? entityId = null,
        int page = 1,
        int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.AuditLogs.AsQueryable();

        if (!string.IsNullOrWhiteSpace(module))
        {
            query = query.Where(a => a.Module == module);
        }

        if (!string.IsNullOrWhiteSpace(action))
        {
            query = query.Where(a => a.Action == action);
        }

        if (entityId.HasValue && entityId.Value != Guid.Empty)
        {
            query = query.Where(a => a.EntityId == entityId.Value);
        }

        return await query
            .OrderByDescending(a => a.Timestamp)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetTotalCountAsync(
        string? module = null,
        string? action = null,
        Guid? entityId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.AuditLogs.AsQueryable();

        if (!string.IsNullOrWhiteSpace(module))
        {
            query = query.Where(a => a.Module == module);
        }

        if (!string.IsNullOrWhiteSpace(action))
        {
            query = query.Where(a => a.Action == action);
        }

        if (entityId.HasValue && entityId.Value != Guid.Empty)
        {
            query = query.Where(a => a.EntityId == entityId.Value);
        }

        return await query.CountAsync(cancellationToken);
    }

    public async Task AddAsync(AuditLog log, CancellationToken cancellationToken = default)
    {
        await _dbContext.AuditLogs.AddAsync(log, cancellationToken);
    }
}

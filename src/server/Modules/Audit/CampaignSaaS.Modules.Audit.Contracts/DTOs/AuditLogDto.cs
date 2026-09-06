namespace CampaignSaaS.Modules.Audit.Contracts.DTOs;

public record AuditLogDto(
    Guid Id,
    Guid OrganizationId,
    Guid? UserId,
    string? ActorEmail,
    string Action,
    string Module,
    string EntityName,
    Guid EntityId,
    string? ChangesJson,
    string? IpAddress,
    DateTimeOffset Timestamp
);

public record PagedResult<T>(
    IReadOnlyList<T> Items,
    int TotalCount,
    int Page,
    int PageSize
);

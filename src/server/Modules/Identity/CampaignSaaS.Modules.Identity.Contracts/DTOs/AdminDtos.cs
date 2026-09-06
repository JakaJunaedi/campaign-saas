namespace CampaignSaaS.Modules.Identity.Contracts.DTOs;

public record AdminOverviewDto(
    int TotalTenants,
    int TotalUsers,
    int ActiveTenants,
    string SystemStatus,
    DateTimeOffset Timestamp);

public record AdminOrganizationItemDto(
    Guid Id,
    string Name,
    string Slug,
    string Status,
    int UsersCount,
    DateTimeOffset CreatedAt);

public record UpdateOrganizationStatusRequest(
    string Status);

public record PagedResult<T>(
    IReadOnlyList<T> Items,
    int TotalCount,
    int PageNumber,
    int PageSize,
    int TotalPages);

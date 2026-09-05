namespace CampaignSaaS.Modules.Creator.Contracts.DTOs;

public record SocialAccountDto(
    string Platform,
    string Handle,
    string ProfileUrl,
    long FollowerCount);

public record CreatorDto(
    Guid Id,
    Guid OrganizationId,
    string FullName,
    string? Email,
    string? PhoneNumber,
    string Niche,
    string Status,
    IReadOnlyList<SocialAccountDto> SocialAccounts,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt);

public record CreatorSummaryDto(
    Guid Id,
    Guid OrganizationId,
    string FullName,
    string? Email,
    string? PhoneNumber,
    string Niche,
    string Status,
    int SocialAccountsCount,
    long TotalFollowers,
    DateTimeOffset CreatedAt);

public record PagedResult<T>(
    IReadOnlyList<T> Items,
    int TotalCount,
    int PageNumber,
    int PageSize,
    int TotalPages);

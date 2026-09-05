namespace CampaignSaaS.Modules.Campaign.Contracts.DTOs;

public record CampaignDto(
    Guid Id,
    Guid OrganizationId,
    Guid ClientId,
    string Title,
    string? Description,
    decimal Budget,
    DateOnly StartDate,
    DateOnly EndDate,
    string Status,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt);

public record CampaignSummaryDto(
    Guid Id,
    Guid OrganizationId,
    Guid ClientId,
    string Title,
    decimal Budget,
    DateOnly StartDate,
    DateOnly EndDate,
    string Status,
    int CreatorsCount,
    DateTimeOffset CreatedAt);

public record CampaignCreatorDto(
    Guid Id,
    Guid OrganizationId,
    Guid CampaignId,
    Guid CreatorId,
    string Status,
    decimal AgreedRate,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt);

public record PagedResult<T>(
    IReadOnlyList<T> Items,
    int TotalCount,
    int PageNumber,
    int PageSize,
    int TotalPages);

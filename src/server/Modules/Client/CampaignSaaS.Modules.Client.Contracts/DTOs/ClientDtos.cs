namespace CampaignSaaS.Modules.Client.Contracts.DTOs;

public record ClientContactDto(
    string Name,
    string Email,
    string? PhoneNumber,
    string? Position);

public record ClientDto(
    Guid Id,
    Guid OrganizationId,
    string Name,
    string? CompanyName,
    IReadOnlyList<ClientContactDto> Contacts,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt);

public record ClientSummaryDto(
    Guid Id,
    Guid OrganizationId,
    string Name,
    string? CompanyName,
    int ContactsCount,
    DateTimeOffset CreatedAt);

public record PagedResult<T>(
    IReadOnlyList<T> Items,
    int TotalCount,
    int PageNumber,
    int PageSize,
    int TotalPages);

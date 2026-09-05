namespace CampaignSaaS.Modules.Identity.Contracts.DTOs;

public record UserDto(
    Guid Id,
    Guid OrganizationId,
    string Email,
    string FullName,
    string Role,
    bool IsActive,
    DateTimeOffset CreatedAt);

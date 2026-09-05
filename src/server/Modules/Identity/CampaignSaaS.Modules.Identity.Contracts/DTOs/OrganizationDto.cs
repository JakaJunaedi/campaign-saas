namespace CampaignSaaS.Modules.Identity.Contracts.DTOs;

public record OrganizationDto(
    Guid Id,
    string Name,
    string Slug,
    string Status,
    DateTimeOffset CreatedAt);

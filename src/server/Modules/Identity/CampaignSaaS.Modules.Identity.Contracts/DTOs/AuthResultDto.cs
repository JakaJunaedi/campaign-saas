namespace CampaignSaaS.Modules.Identity.Contracts.DTOs;

public record AuthResultDto(
    string AccessToken,
    string RefreshToken,
    int ExpiresIn,
    UserDto User,
    OrganizationDto Organization);

namespace CampaignSaaS.Modules.Creator.Contracts.Requests;

using CampaignSaaS.Modules.Creator.Contracts.DTOs;

public record CreateCreatorRequest(
    string FullName,
    string Niche,
    string? Email,
    string? PhoneNumber,
    List<SocialAccountDto>? SocialAccounts);

public record UpdateCreatorRequest(
    string FullName,
    string Niche,
    string? Email,
    string? PhoneNumber,
    List<SocialAccountDto>? SocialAccounts);

public record UpdateCreatorStatusRequest(
    string Status);

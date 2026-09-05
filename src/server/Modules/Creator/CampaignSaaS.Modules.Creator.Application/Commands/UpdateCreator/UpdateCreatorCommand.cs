namespace CampaignSaaS.Modules.Creator.Application.Commands.UpdateCreator;

using CampaignSaaS.Modules.Creator.Contracts.DTOs;
using CampaignSaaS.SharedKernel.Application;

public record UpdateCreatorCommand(
    Guid Id,
    string FullName,
    string Niche,
    string? Email,
    string? PhoneNumber,
    List<SocialAccountDto>? SocialAccounts) : ICommand<CreatorDto>;

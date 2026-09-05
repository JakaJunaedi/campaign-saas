namespace CampaignSaaS.Modules.Creator.Application.Commands.CreateCreator;

using CampaignSaaS.Modules.Creator.Contracts.DTOs;
using CampaignSaaS.SharedKernel.Application;

public record CreateCreatorCommand(
    string FullName,
    string Niche,
    string? Email,
    string? PhoneNumber,
    List<SocialAccountDto>? SocialAccounts) : ICommand<CreatorDto>;

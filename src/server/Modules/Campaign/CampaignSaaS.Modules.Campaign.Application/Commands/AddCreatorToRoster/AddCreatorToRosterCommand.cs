namespace CampaignSaaS.Modules.Campaign.Application.Commands.AddCreatorToRoster;

using CampaignSaaS.Modules.Campaign.Contracts.DTOs;
using CampaignSaaS.SharedKernel.Application;

public record AddCreatorToRosterCommand(
    Guid CampaignId,
    Guid CreatorId,
    decimal AgreedRate = 0) : ICommand<CampaignCreatorDto>;

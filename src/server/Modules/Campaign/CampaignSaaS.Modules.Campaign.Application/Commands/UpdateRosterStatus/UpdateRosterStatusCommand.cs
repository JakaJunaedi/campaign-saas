namespace CampaignSaaS.Modules.Campaign.Application.Commands.UpdateRosterStatus;

using CampaignSaaS.Modules.Campaign.Contracts.DTOs;
using CampaignSaaS.SharedKernel.Application;

public record UpdateRosterStatusCommand(
    Guid CampaignId,
    Guid CreatorId,
    string Status,
    decimal? AgreedRate = null) : ICommand<CampaignCreatorDto>;

namespace CampaignSaaS.Modules.Campaign.Application.Commands.UpdateCampaignStatus;

using CampaignSaaS.Modules.Campaign.Contracts.DTOs;
using CampaignSaaS.SharedKernel.Application;

public record UpdateCampaignStatusCommand(Guid Id, string Status) : ICommand<CampaignDto>;

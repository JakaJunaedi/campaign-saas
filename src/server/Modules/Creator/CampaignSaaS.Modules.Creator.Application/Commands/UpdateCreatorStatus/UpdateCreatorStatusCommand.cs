namespace CampaignSaaS.Modules.Creator.Application.Commands.UpdateCreatorStatus;

using CampaignSaaS.Modules.Creator.Contracts.DTOs;
using CampaignSaaS.SharedKernel.Application;

public record UpdateCreatorStatusCommand(Guid Id, string Status) : ICommand<CreatorDto>;

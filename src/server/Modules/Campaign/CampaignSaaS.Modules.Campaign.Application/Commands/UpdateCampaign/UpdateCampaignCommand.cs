namespace CampaignSaaS.Modules.Campaign.Application.Commands.UpdateCampaign;

using CampaignSaaS.Modules.Campaign.Contracts.DTOs;
using CampaignSaaS.SharedKernel.Application;

public record UpdateCampaignCommand(
    Guid Id,
    string Title,
    string? Description,
    decimal Budget,
    DateOnly StartDate,
    DateOnly EndDate) : ICommand<CampaignDto>;

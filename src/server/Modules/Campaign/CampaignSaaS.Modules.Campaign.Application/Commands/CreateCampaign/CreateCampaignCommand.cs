namespace CampaignSaaS.Modules.Campaign.Application.Commands.CreateCampaign;

using CampaignSaaS.Modules.Campaign.Contracts.DTOs;
using CampaignSaaS.SharedKernel.Application;

public record CreateCampaignCommand(
    Guid ClientId,
    string Title,
    string? Description,
    decimal Budget,
    DateOnly StartDate,
    DateOnly EndDate) : ICommand<CampaignDto>;

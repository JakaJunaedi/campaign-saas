namespace CampaignSaaS.Modules.Campaign.Application.Queries.GetCampaignById;

using CampaignSaaS.Modules.Campaign.Contracts.DTOs;
using CampaignSaaS.SharedKernel.Application;

public record GetCampaignByIdQuery(Guid Id) : IQuery<CampaignDto>;

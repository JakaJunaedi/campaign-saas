namespace CampaignSaaS.Modules.Campaign.Application.Queries.GetCampaignRoster;

using CampaignSaaS.Modules.Campaign.Contracts.DTOs;
using CampaignSaaS.SharedKernel.Application;

public record GetCampaignRosterQuery(Guid CampaignId) : IQuery<IReadOnlyList<CampaignCreatorDto>>;

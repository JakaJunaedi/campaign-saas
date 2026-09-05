namespace CampaignSaaS.Modules.Campaign.Application.Queries.GetCampaigns;

using CampaignSaaS.Modules.Campaign.Contracts.DTOs;
using CampaignSaaS.SharedKernel.Application;

public record GetCampaignsQuery(
    string? SearchTerm = null,
    Guid? ClientId = null,
    string? Status = null,
    int PageNumber = 1,
    int PageSize = 10) : IQuery<PagedResult<CampaignSummaryDto>>;

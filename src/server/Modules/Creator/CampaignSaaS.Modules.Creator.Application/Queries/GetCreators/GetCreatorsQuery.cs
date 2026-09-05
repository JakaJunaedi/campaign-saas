namespace CampaignSaaS.Modules.Creator.Application.Queries.GetCreators;

using CampaignSaaS.Modules.Creator.Contracts.DTOs;
using CampaignSaaS.SharedKernel.Application;

public record GetCreatorsQuery(
    string? SearchTerm = null,
    string? Niche = null,
    string? Platform = null,
    string? Status = null,
    int PageNumber = 1,
    int PageSize = 10) : IQuery<PagedResult<CreatorSummaryDto>>;

namespace CampaignSaaS.Modules.Client.Application.Queries.GetClients;

using CampaignSaaS.Modules.Client.Contracts.DTOs;
using CampaignSaaS.SharedKernel.Application;

public record GetClientsQuery(
    string? SearchTerm = null,
    int PageNumber = 1,
    int PageSize = 10) : IQuery<PagedResult<ClientSummaryDto>>;

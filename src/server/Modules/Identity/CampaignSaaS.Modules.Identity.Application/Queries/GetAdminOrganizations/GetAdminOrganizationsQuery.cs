namespace CampaignSaaS.Modules.Identity.Application.Queries.GetAdminOrganizations;

using CampaignSaaS.Modules.Identity.Contracts.DTOs;
using CampaignSaaS.SharedKernel.Application;

public record GetAdminOrganizationsQuery(
    string? Search = null,
    string? Status = null,
    int PageNumber = 1,
    int PageSize = 10) : IQuery<PagedResult<AdminOrganizationItemDto>>;

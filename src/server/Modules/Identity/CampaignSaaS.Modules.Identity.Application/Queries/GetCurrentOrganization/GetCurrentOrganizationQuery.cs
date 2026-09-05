namespace CampaignSaaS.Modules.Identity.Application.Queries.GetCurrentOrganization;

using CampaignSaaS.Modules.Identity.Contracts.DTOs;
using CampaignSaaS.SharedKernel.Application;

public record GetCurrentOrganizationQuery : IQuery<OrganizationDto>;

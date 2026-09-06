namespace CampaignSaaS.Modules.Identity.Application.Queries.GetAdminOverview;

using CampaignSaaS.Modules.Identity.Contracts.DTOs;
using CampaignSaaS.SharedKernel.Application;

public record GetAdminOverviewQuery() : IQuery<AdminOverviewDto>;

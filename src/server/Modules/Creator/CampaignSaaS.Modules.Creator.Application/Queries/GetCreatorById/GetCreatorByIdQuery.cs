namespace CampaignSaaS.Modules.Creator.Application.Queries.GetCreatorById;

using CampaignSaaS.Modules.Creator.Contracts.DTOs;
using CampaignSaaS.SharedKernel.Application;

public record GetCreatorByIdQuery(Guid Id) : IQuery<CreatorDto>;

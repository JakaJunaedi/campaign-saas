namespace CampaignSaaS.Modules.Client.Application.Queries.GetClientById;

using CampaignSaaS.Modules.Client.Contracts.DTOs;
using CampaignSaaS.SharedKernel.Application;

public record GetClientByIdQuery(Guid Id) : IQuery<ClientDto>;

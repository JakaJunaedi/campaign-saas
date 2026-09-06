namespace CampaignSaaS.Modules.Deliverable.Application.Queries.GetDeliverableById;

using CampaignSaaS.Modules.Deliverable.Contracts.DTOs;
using CampaignSaaS.SharedKernel.Application;

public record GetDeliverableByIdQuery(Guid Id) : IQuery<DeliverableDto>;


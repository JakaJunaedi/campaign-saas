namespace CampaignSaaS.Modules.Deliverable.Application.Queries.GetDeliverablesByCampaign;

using CampaignSaaS.Modules.Deliverable.Contracts.DTOs;
using CampaignSaaS.SharedKernel.Application;

public record GetDeliverablesByCampaignQuery(Guid CampaignId) : IQuery<IReadOnlyList<DeliverableDto>>;


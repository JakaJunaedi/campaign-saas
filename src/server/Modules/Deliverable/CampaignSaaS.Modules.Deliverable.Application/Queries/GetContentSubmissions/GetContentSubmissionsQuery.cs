namespace CampaignSaaS.Modules.Deliverable.Application.Queries.GetContentSubmissions;

using CampaignSaaS.Modules.Deliverable.Contracts.DTOs;
using CampaignSaaS.SharedKernel.Application;

public record GetContentSubmissionsQuery(Guid DeliverableId) : IQuery<IReadOnlyList<ContentSubmissionDto>>;


namespace CampaignSaaS.Modules.Deliverable.Application.Commands.CreateDeliverable;

using CampaignSaaS.Modules.Deliverable.Contracts.DTOs;
using CampaignSaaS.SharedKernel.Application;

public record CreateDeliverableCommand(
    Guid CampaignId,
    Guid CampaignCreatorId,
    string Title,
    string Platform,
    string ContentType,
    string? BriefNotes,
    DateOnly DueDate) : ICommand<DeliverableDto>;


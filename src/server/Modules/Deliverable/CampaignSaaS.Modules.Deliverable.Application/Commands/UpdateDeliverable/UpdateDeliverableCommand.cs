namespace CampaignSaaS.Modules.Deliverable.Application.Commands.UpdateDeliverable;

using CampaignSaaS.Modules.Deliverable.Contracts.DTOs;
using CampaignSaaS.SharedKernel.Application;

public record UpdateDeliverableCommand(
    Guid Id,
    string Title,
    string Platform,
    string ContentType,
    string? BriefNotes,
    DateOnly DueDate) : ICommand<DeliverableDto>;


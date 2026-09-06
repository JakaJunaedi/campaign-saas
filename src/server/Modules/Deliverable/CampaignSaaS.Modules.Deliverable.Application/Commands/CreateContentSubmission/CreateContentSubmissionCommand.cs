namespace CampaignSaaS.Modules.Deliverable.Application.Commands.CreateContentSubmission;

using CampaignSaaS.Modules.Deliverable.Contracts.DTOs;
using CampaignSaaS.SharedKernel.Application;

public record CreateContentSubmissionCommand(
    Guid DeliverableId,
    string MediaObjectKey,
    string MediaFileName,
    long MediaFileSize,
    string? Caption) : ICommand<ContentSubmissionDto>;


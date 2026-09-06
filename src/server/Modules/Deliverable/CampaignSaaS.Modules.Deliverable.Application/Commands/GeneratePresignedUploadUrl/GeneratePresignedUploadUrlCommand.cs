namespace CampaignSaaS.Modules.Deliverable.Application.Commands.GeneratePresignedUploadUrl;

using CampaignSaaS.Modules.Deliverable.Contracts.DTOs;
using CampaignSaaS.SharedKernel.Application;

public record GeneratePresignedUploadUrlCommand(
    string FileName,
    string ContentType,
    long FileSize,
    Guid? CampaignId = null,
    Guid? DeliverableId = null) : ICommand<PresignedUploadUrlDto>;


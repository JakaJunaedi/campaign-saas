namespace CampaignSaaS.Modules.Deliverable.Application.Abstractions;

using CampaignSaaS.Modules.Deliverable.Contracts.DTOs;

public interface IStorageService
{
    Task<PresignedUploadUrlDto> GeneratePresignedUploadUrlAsync(
        Guid organizationId,
        string fileName,
        string contentType,
        long fileSize,
        Guid? campaignId = null,
        Guid? deliverableId = null,
        int? version = null,
        CancellationToken cancellationToken = default);

    Task<string> GeneratePresignedDownloadUrlAsync(
        string objectKey,
        int expiryMinutes = 60,
        CancellationToken cancellationToken = default);
}


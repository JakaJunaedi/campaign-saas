namespace CampaignSaaS.Modules.Deliverable.Contracts.DTOs;

public record DeliverableDto(
    Guid Id,
    Guid OrganizationId,
    Guid CampaignId,
    Guid CampaignCreatorId,
    string Title,
    string Platform,
    string ContentType,
    string? BriefNotes,
    DateOnly DueDate,
    DateOnly? PostingDate,
    string Status,
    string? LiveUrl,
    string? ProofMediaKey,
    int LatestVersion,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt);

public record ContentSubmissionDto(
    Guid Id,
    Guid OrganizationId,
    Guid DeliverableId,
    int VersionNumber,
    string MediaObjectKey,
    string MediaFileName,
    long MediaFileSize,
    string? Caption,
    string? DownloadUrl,
    DateTimeOffset SubmittedAt);

public record PresignedUploadUrlDto(
    string UploadUrl,
    string ObjectKey,
    int ExpiresInMinutes);


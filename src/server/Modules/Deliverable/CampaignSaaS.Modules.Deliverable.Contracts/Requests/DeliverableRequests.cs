namespace CampaignSaaS.Modules.Deliverable.Contracts.Requests;

public record CreateDeliverableRequest(
    Guid CampaignCreatorId,
    string Title,
    string Platform,
    string ContentType,
    string? BriefNotes,
    DateOnly DueDate);

public record UpdateDeliverableRequest(
    string Title,
    string Platform,
    string ContentType,
    string? BriefNotes,
    DateOnly DueDate);

public record CreateContentSubmissionRequest(
    string MediaObjectKey,
    string MediaFileName,
    long MediaFileSize,
    string? Caption);

public record SubmitPublishProofRequest(
    string LiveUrl,
    string? ProofMediaKey,
    DateOnly? PostingDate);

public record GetPresignedUploadUrlRequest(
    string FileName,
    string ContentType,
    long FileSize,
    Guid? CampaignId = null,
    Guid? DeliverableId = null);


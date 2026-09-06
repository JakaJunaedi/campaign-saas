namespace CampaignSaaS.Modules.Approval.Contracts.DTOs;

public record ApprovalReviewDto(
    Guid Id,
    Guid OrganizationId,
    Guid ContentSubmissionId,
    Guid ReviewerId,
    string Decision,
    string? FeedbackNotes,
    DateTimeOffset ReviewedAt);

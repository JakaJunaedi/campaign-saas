namespace CampaignSaaS.Modules.Approval.Contracts.Requests;

public record SubmitReviewRequest(
    string Decision,
    string? FeedbackNotes,
    Guid? ReviewerId = null);

namespace CampaignSaaS.SharedKernel.IntegrationEvents;

public record DeliverableSubmittedIntegrationEvent(
    Guid EventId,
    DateTime OccurredOnUtc,
    Guid OrganizationId,
    Guid DeliverableId,
    Guid SubmissionId,
    Guid CampaignId,
    Guid CreatorId
) : IIntegrationEvent
{
    public static DeliverableSubmittedIntegrationEvent Create(
        Guid organizationId,
        Guid deliverableId,
        Guid submissionId,
        Guid campaignId,
        Guid creatorId) =>
        new(Guid.NewGuid(), DateTime.UtcNow, organizationId, deliverableId, submissionId, campaignId, creatorId);
}

namespace CampaignSaaS.Modules.Deliverable.Domain.Events;

using CampaignSaaS.SharedKernel.Domain;

public record DeliverablePublishedDomainEvent(
    Guid DeliverableId,
    Guid OrganizationId,
    string LiveUrl,
    string? ProofMediaKey,
    DateOnly PostingDate,
    DateTimeOffset PublishedAt) : IDomainEvent;


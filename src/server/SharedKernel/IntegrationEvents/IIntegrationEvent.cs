namespace CampaignSaaS.SharedKernel.IntegrationEvents;

public interface IIntegrationEvent
{
    Guid EventId { get; }
    DateTime OccurredOnUtc { get; }
    Guid OrganizationId { get; }
}

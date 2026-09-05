namespace CampaignSaaS.SharedKernel.Domain;

using MediatR;

public interface IDomainEvent : INotification
{
    Guid EventId => Guid.NewGuid();
    DateTime OccurredOn => DateTime.UtcNow;
}

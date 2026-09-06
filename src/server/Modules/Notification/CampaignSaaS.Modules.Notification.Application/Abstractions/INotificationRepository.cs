namespace CampaignSaaS.Modules.Notification.Application.Abstractions;

using CampaignSaaS.Modules.Notification.Domain.Entities;

public interface INotificationRepository
{
    Task<InAppNotification?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<InAppNotification>> GetByUserIdAsync(Guid userId, bool? unreadOnly = null, int take = 50, CancellationToken cancellationToken = default);
    Task<int> GetUnreadCountAsync(Guid userId, CancellationToken cancellationToken = default);
    Task AddAsync(InAppNotification notification, CancellationToken cancellationToken = default);
    Task MarkAllAsReadAsync(Guid userId, CancellationToken cancellationToken = default);
}

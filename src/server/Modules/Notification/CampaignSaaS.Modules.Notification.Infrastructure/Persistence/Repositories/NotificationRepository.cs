namespace CampaignSaaS.Modules.Notification.Infrastructure.Persistence.Repositories;

using CampaignSaaS.Modules.Notification.Application.Abstractions;
using CampaignSaaS.Modules.Notification.Domain.Entities;
using CampaignSaaS.Modules.Notification.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

public class NotificationRepository : INotificationRepository
{
    private readonly NotificationDbContext _dbContext;

    public NotificationRepository(NotificationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<InAppNotification?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Notifications
            .FirstOrDefaultAsync(n => n.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<InAppNotification>> GetByUserIdAsync(
        Guid userId,
        bool? unreadOnly = null,
        int take = 50,
        CancellationToken cancellationToken = default)
    {
        // Support specific user or broadcast notifications (UserId == Guid.Empty)
        var query = _dbContext.Notifications
            .Where(n => n.UserId == userId || n.UserId == Guid.Empty);

        if (unreadOnly.HasValue && unreadOnly.Value)
        {
            query = query.Where(n => !n.IsRead);
        }

        return await query
            .OrderByDescending(n => n.CreatedAt)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetUnreadCountAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Notifications
            .Where(n => (n.UserId == userId || n.UserId == Guid.Empty) && !n.IsRead)
            .CountAsync(cancellationToken);
    }

    public async Task AddAsync(InAppNotification notification, CancellationToken cancellationToken = default)
    {
        await _dbContext.Notifications.AddAsync(notification, cancellationToken);
    }

    public async Task MarkAllAsReadAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var unreadNotifications = await _dbContext.Notifications
            .Where(n => (n.UserId == userId || n.UserId == Guid.Empty) && !n.IsRead)
            .ToListAsync(cancellationToken);

        foreach (var notification in unreadNotifications)
        {
            notification.MarkAsRead();
        }
    }
}

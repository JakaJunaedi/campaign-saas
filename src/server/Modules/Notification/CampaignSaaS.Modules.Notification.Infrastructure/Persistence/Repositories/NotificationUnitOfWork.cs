namespace CampaignSaaS.Modules.Notification.Infrastructure.Persistence.Repositories;

using CampaignSaaS.Modules.Notification.Application.Abstractions;
using CampaignSaaS.Modules.Notification.Infrastructure.Persistence;

public class NotificationUnitOfWork : INotificationUnitOfWork
{
    private readonly NotificationDbContext _dbContext;

    public NotificationUnitOfWork(NotificationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.SaveChangesAsync(cancellationToken);
    }
}

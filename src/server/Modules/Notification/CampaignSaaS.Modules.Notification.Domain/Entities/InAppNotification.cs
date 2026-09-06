namespace CampaignSaaS.Modules.Notification.Domain.Entities;

using CampaignSaaS.Modules.Notification.Domain.Events;
using CampaignSaaS.SharedKernel.Domain;
using CampaignSaaS.SharedKernel.MultiTenancy;

public class InAppNotification : AggregateRoot<Guid>, ITenantEntity
{
    public Guid OrganizationId { get; private set; }
    public Guid UserId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Message { get; private set; } = string.Empty;
    public string? LinkUrl { get; private set; }
    public bool IsRead { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? ReadAt { get; private set; }

    private InAppNotification() { }

    private InAppNotification(
        Guid id,
        Guid organizationId,
        Guid userId,
        string title,
        string message,
        string? linkUrl) : base(id)
    {
        OrganizationId = organizationId;
        UserId = userId;
        Title = title.Trim();
        Message = message.Trim();
        LinkUrl = linkUrl?.Trim();
        IsRead = false;
        CreatedAt = DateTimeOffset.UtcNow;
        ReadAt = null;
    }

    public static InAppNotification Create(
        Guid organizationId,
        Guid userId,
        string title,
        string message,
        string? linkUrl = null)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty.", nameof(title));

        if (string.IsNullOrWhiteSpace(message))
            throw new ArgumentException("Message cannot be empty.", nameof(message));

        var notification = new InAppNotification(
            Guid.NewGuid(),
            organizationId,
            userId,
            title,
            message,
            linkUrl);

        notification.AddDomainEvent(new NotificationCreatedDomainEvent(
            notification.Id,
            notification.OrganizationId,
            notification.UserId,
            notification.Title,
            notification.Message,
            notification.CreatedAt));

        return notification;
    }

    public void MarkAsRead()
    {
        if (!IsRead)
        {
            IsRead = true;
            ReadAt = DateTimeOffset.UtcNow;

            AddDomainEvent(new NotificationReadDomainEvent(
                Id,
                OrganizationId,
                UserId,
                ReadAt.Value));
        }
    }
}

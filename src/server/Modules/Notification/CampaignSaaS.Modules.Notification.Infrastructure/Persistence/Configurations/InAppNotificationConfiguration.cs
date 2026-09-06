namespace CampaignSaaS.Modules.Notification.Infrastructure.Persistence.Configurations;

using CampaignSaaS.Modules.Notification.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class InAppNotificationConfiguration : IEntityTypeConfiguration<InAppNotification>
{
    public void Configure(EntityTypeBuilder<InAppNotification> builder)
    {
        builder.ToTable("notifications");

        builder.HasKey(n => n.Id);
        builder.Property(n => n.Id)
            .HasColumnName("id");

        builder.Property(n => n.OrganizationId)
            .HasColumnName("organization_id")
            .IsRequired();

        builder.Property(n => n.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.Property(n => n.Title)
            .HasColumnName("title")
            .HasMaxLength(250)
            .IsRequired();

        builder.Property(n => n.Message)
            .HasColumnName("message")
            .IsRequired();

        builder.Property(n => n.LinkUrl)
            .HasColumnName("link_url")
            .HasMaxLength(500);

        builder.Property(n => n.IsRead)
            .HasColumnName("is_read")
            .HasDefaultValue(false)
            .IsRequired();

        builder.Property(n => n.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(n => n.ReadAt)
            .HasColumnName("read_at");

        builder.Ignore(n => n.DomainEvents);

        builder.HasIndex(n => new { n.OrganizationId, n.UserId, n.IsRead })
            .HasDatabaseName("idx_notifications_user_unread");

        builder.HasIndex(n => new { n.OrganizationId, n.CreatedAt })
            .HasDatabaseName("idx_notifications_org_created");
    }
}

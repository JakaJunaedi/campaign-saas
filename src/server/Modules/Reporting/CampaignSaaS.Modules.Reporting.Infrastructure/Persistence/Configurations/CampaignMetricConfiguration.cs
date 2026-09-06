namespace CampaignSaaS.Modules.Reporting.Infrastructure.Persistence.Configurations;

using CampaignSaaS.Modules.Reporting.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class CampaignMetricConfiguration : IEntityTypeConfiguration<CampaignMetric>
{
    public void Configure(EntityTypeBuilder<CampaignMetric> builder)
    {
        builder.ToTable("campaign_metrics");

        builder.HasKey(m => m.Id);
        builder.Property(m => m.Id).HasColumnName("id");

        builder.Property(m => m.OrganizationId)
            .HasColumnName("organization_id")
            .IsRequired();

        builder.Property(m => m.DeliverableId)
            .HasColumnName("deliverable_id")
            .IsRequired();

        builder.Property(m => m.Reach)
            .HasColumnName("reach")
            .HasDefaultValue(0L)
            .IsRequired();

        builder.Property(m => m.Impressions)
            .HasColumnName("impressions")
            .HasDefaultValue(0L)
            .IsRequired();

        builder.Property(m => m.Views)
            .HasColumnName("views")
            .HasDefaultValue(0L)
            .IsRequired();

        builder.Property(m => m.Likes)
            .HasColumnName("likes")
            .HasDefaultValue(0L)
            .IsRequired();

        builder.Property(m => m.Comments)
            .HasColumnName("comments")
            .HasDefaultValue(0L)
            .IsRequired();

        builder.Property(m => m.Shares)
            .HasColumnName("shares")
            .HasDefaultValue(0L)
            .IsRequired();

        builder.Property(m => m.Clicks)
            .HasColumnName("clicks")
            .HasDefaultValue(0L)
            .IsRequired();

        builder.Property(m => m.TotalEngagement)
            .HasColumnName("total_engagement")
            .HasDefaultValue(0L)
            .IsRequired();

        builder.Property(m => m.RecordedAt)
            .HasColumnName("recorded_at")
            .IsRequired();

        builder.HasIndex(m => m.DeliverableId)
            .IsUnique()
            .HasDatabaseName("uq_metrics_deliverable");

        builder.HasIndex(m => m.OrganizationId)
            .HasDatabaseName("idx_metrics_org");
    }
}

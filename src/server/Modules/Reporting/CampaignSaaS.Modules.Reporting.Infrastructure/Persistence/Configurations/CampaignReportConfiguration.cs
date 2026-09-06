namespace CampaignSaaS.Modules.Reporting.Infrastructure.Persistence.Configurations;

using CampaignSaaS.Modules.Reporting.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class CampaignReportConfiguration : IEntityTypeConfiguration<CampaignReport>
{
    public void Configure(EntityTypeBuilder<CampaignReport> builder)
    {
        builder.ToTable("campaign_reports");

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).HasColumnName("id");

        builder.Property(r => r.OrganizationId)
            .HasColumnName("organization_id")
            .IsRequired();

        builder.Property(r => r.CampaignId)
            .HasColumnName("campaign_id")
            .IsRequired();

        builder.Property(r => r.Status)
            .HasColumnName("status")
            .HasMaxLength(50)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(r => r.FileObjectKey)
            .HasColumnName("file_object_key")
            .HasMaxLength(500);

        builder.Property(r => r.ErrorMessage)
            .HasColumnName("error_message")
            .HasMaxLength(2000);

        builder.Property(r => r.RequestedAt)
            .HasColumnName("requested_at")
            .IsRequired();

        builder.Property(r => r.CompletedAt)
            .HasColumnName("completed_at");

        builder.HasIndex(r => r.CampaignId)
            .HasDatabaseName("idx_reports_campaign");

        builder.HasIndex(r => r.OrganizationId)
            .HasDatabaseName("idx_reports_org");
    }
}

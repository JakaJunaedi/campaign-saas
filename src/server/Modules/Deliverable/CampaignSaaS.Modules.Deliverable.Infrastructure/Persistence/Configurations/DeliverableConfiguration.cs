namespace CampaignSaaS.Modules.Deliverable.Infrastructure.Persistence.Configurations;

using CampaignSaaS.Modules.Deliverable.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class DeliverableConfiguration : IEntityTypeConfiguration<Deliverable>
{
    public void Configure(EntityTypeBuilder<Deliverable> builder)
    {
        builder.ToTable("deliverables");

        builder.HasKey(d => d.Id);
        builder.Property(d => d.Id).HasColumnName("id");

        builder.Property(d => d.OrganizationId)
            .HasColumnName("organization_id")
            .IsRequired();

        builder.Property(d => d.CampaignId)
            .HasColumnName("campaign_id")
            .IsRequired();

        builder.Property(d => d.CampaignCreatorId)
            .HasColumnName("campaign_creator_id")
            .IsRequired();

        builder.Property(d => d.Title)
            .HasColumnName("title")
            .HasMaxLength(250)
            .IsRequired();

        builder.Property(d => d.Platform)
            .HasColumnName("platform")
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(d => d.ContentType)
            .HasColumnName("content_type")
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(d => d.BriefNotes)
            .HasColumnName("brief_notes");

        builder.Property(d => d.DueDate)
            .HasColumnName("due_date")
            .IsRequired();

        builder.Property(d => d.PostingDate)
            .HasColumnName("posting_date");

        builder.Property(d => d.Status)
            .HasColumnName("status")
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(d => d.LiveUrl)
            .HasColumnName("live_url");

        builder.Property(d => d.ProofMediaKey)
            .HasColumnName("proof_media_key")
            .HasMaxLength(500);

        builder.Property(d => d.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(d => d.UpdatedAt)
            .HasColumnName("updated_at");

        builder.HasIndex(d => new { d.OrganizationId, d.Status })
            .HasDatabaseName("idx_deliverables_org_status");

        builder.HasIndex(d => d.CampaignId)
            .HasDatabaseName("idx_deliverables_campaign");
    }
}


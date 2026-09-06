namespace CampaignSaaS.Modules.Deliverable.Infrastructure.Persistence.Configurations;

using CampaignSaaS.Modules.Deliverable.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class ContentSubmissionConfiguration : IEntityTypeConfiguration<ContentSubmission>
{
    public void Configure(EntityTypeBuilder<ContentSubmission> builder)
    {
        builder.ToTable("content_submissions");

        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).HasColumnName("id");

        builder.Property(s => s.OrganizationId)
            .HasColumnName("organization_id")
            .IsRequired();

        builder.Property(s => s.DeliverableId)
            .HasColumnName("deliverable_id")
            .IsRequired();

        builder.Property(s => s.VersionNumber)
            .HasColumnName("version_number")
            .IsRequired();

        builder.Property(s => s.MediaObjectKey)
            .HasColumnName("media_object_key")
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(s => s.MediaFileName)
            .HasColumnName("media_file_name")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(s => s.MediaFileSize)
            .HasColumnName("media_file_size")
            .IsRequired();

        builder.Property(s => s.Caption)
            .HasColumnName("caption");

        builder.Property(s => s.SubmittedAt)
            .HasColumnName("submitted_at")
            .IsRequired();

        builder.HasIndex(s => new { s.DeliverableId, s.VersionNumber })
            .IsUnique()
            .HasDatabaseName("uq_submission_version");

        builder.HasIndex(s => s.DeliverableId)
            .HasDatabaseName("idx_submissions_deliverable");
    }
}


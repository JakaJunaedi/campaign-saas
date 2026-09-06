namespace CampaignSaaS.Modules.Approval.Infrastructure.Persistence.Configurations;

using CampaignSaaS.Modules.Approval.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class ApprovalReviewConfiguration : IEntityTypeConfiguration<ApprovalReview>
{
    public void Configure(EntityTypeBuilder<ApprovalReview> builder)
    {
        builder.ToTable("approval_reviews");

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).HasColumnName("id");

        builder.Property(r => r.OrganizationId)
            .HasColumnName("organization_id")
            .IsRequired();

        builder.Property(r => r.ContentSubmissionId)
            .HasColumnName("content_submission_id")
            .IsRequired();

        builder.Property(r => r.ReviewerId)
            .HasColumnName("reviewer_id")
            .IsRequired();

        builder.Property(r => r.Decision)
            .HasColumnName("decision")
            .HasMaxLength(50)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(r => r.FeedbackNotes)
            .HasColumnName("feedback_notes")
            .HasMaxLength(2000);

        builder.Property(r => r.ReviewedAt)
            .HasColumnName("reviewed_at")
            .IsRequired();

        builder.HasIndex(r => r.ContentSubmissionId)
            .HasDatabaseName("idx_reviews_submission");

        builder.HasIndex(r => r.OrganizationId)
            .HasDatabaseName("idx_reviews_org");
    }
}

namespace CampaignSaaS.Modules.Campaign.Infrastructure.Persistence.Configurations;

using CampaignSaaS.Modules.Campaign.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class CampaignCreatorConfiguration : IEntityTypeConfiguration<CampaignCreator>
{
    public void Configure(EntityTypeBuilder<CampaignCreator> builder)
    {
        builder.ToTable("campaign_creators");

        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).HasColumnName("id");

        builder.Property(c => c.OrganizationId)
            .HasColumnName("organization_id")
            .IsRequired();

        builder.Property(c => c.CampaignId)
            .HasColumnName("campaign_id")
            .IsRequired();

        builder.Property(c => c.CreatorId)
            .HasColumnName("creator_id")
            .IsRequired();

        builder.Property(c => c.Status)
            .HasColumnName("status")
            .HasMaxLength(50)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(c => c.AgreedRate)
            .HasColumnName("agreed_rate")
            .HasPrecision(15, 2)
            .IsRequired();

        builder.Property(c => c.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(c => c.UpdatedAt)
            .HasColumnName("updated_at");

        builder.HasIndex(c => c.OrganizationId);
        builder.HasIndex(c => new { c.CampaignId, c.CreatorId }).IsUnique();
    }
}

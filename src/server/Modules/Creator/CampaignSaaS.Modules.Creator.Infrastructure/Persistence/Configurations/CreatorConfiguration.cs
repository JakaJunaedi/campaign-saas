namespace CampaignSaaS.Modules.Creator.Infrastructure.Persistence.Configurations;

using System.Text.Json;
using CampaignSaaS.Modules.Creator.Domain.Entities;
using CampaignSaaS.Modules.Creator.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class CreatorConfiguration : IEntityTypeConfiguration<Creator>
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public void Configure(EntityTypeBuilder<Creator> builder)
    {
        builder.ToTable("creators");

        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).HasColumnName("id");

        builder.Property(c => c.OrganizationId)
            .HasColumnName("organization_id")
            .IsRequired();

        builder.Property(c => c.FullName)
            .HasColumnName("full_name")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(c => c.Email)
            .HasColumnName("email")
            .HasMaxLength(255);

        builder.Property(c => c.PhoneNumber)
            .HasColumnName("phone_number")
            .HasMaxLength(50);

        builder.Property(c => c.Niche)
            .HasColumnName("niche")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(c => c.Status)
            .HasColumnName("status")
            .HasMaxLength(50)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(c => c.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(c => c.UpdatedAt)
            .HasColumnName("updated_at");

        builder.Property(c => c.IsDeleted)
            .HasColumnName("is_deleted")
            .HasDefaultValue(false)
            .IsRequired();

        var socialConverter = new Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<IReadOnlyList<SocialAccount>, string>(
            v => JsonSerializer.Serialize(v, JsonOptions),
            v => string.IsNullOrEmpty(v)
                ? new List<SocialAccount>()
                : JsonSerializer.Deserialize<List<SocialAccount>>(v, JsonOptions) ?? new List<SocialAccount>());

        var socialComparer = new ValueComparer<IReadOnlyList<SocialAccount>>(
            (c1, c2) => JsonSerializer.Serialize(c1, JsonOptions) == JsonSerializer.Serialize(c2, JsonOptions),
            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
            c => c.ToList());

        builder.Property(c => c.SocialAccounts)
            .HasColumnName("social_accounts_json")
            .HasColumnType("jsonb")
            .HasConversion(socialConverter)
            .Metadata.SetValueComparer(socialComparer);

        builder.HasIndex(c => c.OrganizationId);
        builder.HasIndex(c => new { c.OrganizationId, c.Niche });
        builder.HasIndex(c => new { c.OrganizationId, c.Status });
    }
}

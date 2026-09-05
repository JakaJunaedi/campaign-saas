namespace CampaignSaaS.Modules.Client.Infrastructure.Persistence.Configurations;

using System.Text.Json;
using CampaignSaaS.Modules.Client.Domain.Entities;
using CampaignSaaS.Modules.Client.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class ClientConfiguration : IEntityTypeConfiguration<Client>
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public void Configure(EntityTypeBuilder<Client> builder)
    {
        builder.ToTable("clients");

        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).HasColumnName("id");

        builder.Property(c => c.OrganizationId)
            .HasColumnName("organization_id")
            .IsRequired();

        builder.Property(c => c.Name)
            .HasColumnName("name")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(c => c.CompanyName)
            .HasColumnName("company_name")
            .HasMaxLength(200);

        builder.Property(c => c.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(c => c.UpdatedAt)
            .HasColumnName("updated_at");

        builder.Property(c => c.IsDeleted)
            .HasColumnName("is_deleted")
            .HasDefaultValue(false)
            .IsRequired();

        var contactsConverter = new Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<IReadOnlyList<ClientContact>, string>(
            v => JsonSerializer.Serialize(v, JsonOptions),
            v => string.IsNullOrEmpty(v)
                ? new List<ClientContact>()
                : JsonSerializer.Deserialize<List<ClientContact>>(v, JsonOptions) ?? new List<ClientContact>());

        var contactsComparer = new ValueComparer<IReadOnlyList<ClientContact>>(
            (c1, c2) => JsonSerializer.Serialize(c1, JsonOptions) == JsonSerializer.Serialize(c2, JsonOptions),
            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
            c => c.ToList());

        builder.Property(c => c.Contacts)
            .HasColumnName("contacts_json")
            .HasColumnType("jsonb")
            .HasConversion(contactsConverter)
            .Metadata.SetValueComparer(contactsComparer);

        builder.HasIndex(c => c.OrganizationId);
        builder.HasIndex(c => new { c.OrganizationId, c.Name });
    }
}

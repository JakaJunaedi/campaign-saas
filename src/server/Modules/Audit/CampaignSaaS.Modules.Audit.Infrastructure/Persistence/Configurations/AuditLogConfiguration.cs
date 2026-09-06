namespace CampaignSaaS.Modules.Audit.Infrastructure.Persistence.Configurations;

using CampaignSaaS.Modules.Audit.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("audit_logs");

        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id)
            .HasColumnName("id");

        builder.Property(a => a.OrganizationId)
            .HasColumnName("organization_id")
            .IsRequired();

        builder.Property(a => a.UserId)
            .HasColumnName("user_id");

        builder.Property(a => a.ActorEmail)
            .HasColumnName("actor_email")
            .HasMaxLength(255);

        builder.Property(a => a.Action)
            .HasColumnName("action")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(a => a.Module)
            .HasColumnName("module")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(a => a.EntityName)
            .HasColumnName("entity_name")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(a => a.EntityId)
            .HasColumnName("entity_id")
            .IsRequired();

        builder.Property(a => a.ChangesJson)
            .HasColumnName("changes_json")
            .HasColumnType("jsonb");

        builder.Property(a => a.IpAddress)
            .HasColumnName("ip_address")
            .HasMaxLength(45);

        builder.Property(a => a.Timestamp)
            .HasColumnName("timestamp")
            .IsRequired();

        builder.Ignore(a => a.DomainEvents);

        builder.HasIndex(a => new { a.OrganizationId, a.Timestamp })
            .HasDatabaseName("idx_audit_logs_org_timestamp");

        builder.HasIndex(a => new { a.OrganizationId, a.Module, a.Action })
            .HasDatabaseName("idx_audit_logs_org_module_action");
    }
}

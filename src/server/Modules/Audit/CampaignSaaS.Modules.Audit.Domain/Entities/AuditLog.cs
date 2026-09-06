namespace CampaignSaaS.Modules.Audit.Domain.Entities;

using CampaignSaaS.SharedKernel.Domain;
using CampaignSaaS.SharedKernel.MultiTenancy;

public class AuditLog : AggregateRoot<Guid>, ITenantEntity
{
    public Guid OrganizationId { get; private set; }
    public Guid? UserId { get; private set; }
    public string? ActorEmail { get; private set; }
    public string Action { get; private set; } = string.Empty;
    public string Module { get; private set; } = string.Empty;
    public string EntityName { get; private set; } = string.Empty;
    public Guid EntityId { get; private set; }
    public string? ChangesJson { get; private set; }
    public string? IpAddress { get; private set; }
    public DateTimeOffset Timestamp { get; private set; } = DateTimeOffset.UtcNow;

    private AuditLog() { }

    private AuditLog(
        Guid id,
        Guid organizationId,
        Guid? userId,
        string? actorEmail,
        string action,
        string module,
        string entityName,
        Guid entityId,
        string? changesJson,
        string? ipAddress) : base(id)
    {
        OrganizationId = organizationId;
        UserId = userId;
        ActorEmail = actorEmail?.Trim();
        Action = action.Trim();
        Module = module.Trim();
        EntityName = entityName.Trim();
        EntityId = entityId;
        ChangesJson = changesJson;
        IpAddress = ipAddress?.Trim();
        Timestamp = DateTimeOffset.UtcNow;
    }

    public static AuditLog Create(
        Guid organizationId,
        Guid? userId,
        string? actorEmail,
        string action,
        string module,
        string entityName,
        Guid entityId,
        string? changesJson = null,
        string? ipAddress = null)
    {
        if (string.IsNullOrWhiteSpace(action))
            throw new ArgumentException("Action cannot be empty.", nameof(action));

        if (string.IsNullOrWhiteSpace(module))
            throw new ArgumentException("Module cannot be empty.", nameof(module));

        if (string.IsNullOrWhiteSpace(entityName))
            throw new ArgumentException("Entity name cannot be empty.", nameof(entityName));

        return new AuditLog(
            Guid.NewGuid(),
            organizationId,
            userId,
            actorEmail,
            action,
            module,
            entityName,
            entityId,
            changesJson,
            ipAddress);
    }
}

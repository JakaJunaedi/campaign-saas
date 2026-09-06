namespace CampaignSaaS.Modules.Identity.Domain.Entities;

using CampaignSaaS.Modules.Identity.Domain.Enums;
using CampaignSaaS.Modules.Identity.Domain.Events;
using CampaignSaaS.SharedKernel.Domain;

public class Organization : AggregateRoot<Guid>
{
    public string Name { get; private set; } = string.Empty;
    public string Slug { get; private set; } = string.Empty;
    public OrganizationStatus Status { get; private set; } = OrganizationStatus.Active;
    public DateTimeOffset CreatedAt { get; private set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? UpdatedAt { get; private set; }
    public bool IsDeleted { get; private set; }

    private Organization() { }

    private Organization(Guid id, string name, string slug, OrganizationStatus status) : base(id)
    {
        Name = name;
        Slug = slug.ToLowerInvariant().Trim();
        Status = status;
        CreatedAt = DateTimeOffset.UtcNow;
        IsDeleted = false;
    }

    public static Organization Create(string name, string slug, OrganizationStatus status = OrganizationStatus.Active)
    {
        var org = new Organization(Guid.NewGuid(), name, slug, status);
        org.AddDomainEvent(new OrganizationCreatedDomainEvent(org.Id, org.Name, org.Slug, org.CreatedAt));
        return org;
    }

    public static Organization Create(Guid id, string name, string slug, OrganizationStatus status = OrganizationStatus.Active)
    {
        var org = new Organization(id, name, slug, status);
        org.AddDomainEvent(new OrganizationCreatedDomainEvent(org.Id, org.Name, org.Slug, org.CreatedAt));
        return org;
    }

    public void Update(string name, string slug)
    {
        Name = name;
        Slug = slug.ToLowerInvariant().Trim();
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void SetStatus(OrganizationStatus status)
    {
        Status = status;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void Delete()
    {
        IsDeleted = true;
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}

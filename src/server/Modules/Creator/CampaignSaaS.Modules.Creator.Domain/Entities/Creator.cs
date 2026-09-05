namespace CampaignSaaS.Modules.Creator.Domain.Entities;

using CampaignSaaS.Modules.Creator.Domain.Enums;
using CampaignSaaS.Modules.Creator.Domain.Events;
using CampaignSaaS.Modules.Creator.Domain.ValueObjects;
using CampaignSaaS.SharedKernel.Domain;
using CampaignSaaS.SharedKernel.MultiTenancy;

public class Creator : AggregateRoot<Guid>, ITenantEntity
{
    public Guid OrganizationId { get; private set; }
    public string FullName { get; private set; } = string.Empty;
    public string? Email { get; private set; }
    public string? PhoneNumber { get; private set; }
    public string Niche { get; private set; } = string.Empty;
    public CreatorStatus Status { get; private set; } = CreatorStatus.Active;
    private readonly List<SocialAccount> _socialAccounts = new();
    public IReadOnlyList<SocialAccount> SocialAccounts => _socialAccounts.AsReadOnly();
    public DateTimeOffset CreatedAt { get; private set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? UpdatedAt { get; private set; }
    public bool IsDeleted { get; private set; }

    private Creator() { }

    private Creator(
        Guid id,
        Guid organizationId,
        string fullName,
        string niche,
        string? email,
        string? phoneNumber,
        CreatorStatus status) : base(id)
    {
        OrganizationId = organizationId;
        FullName = fullName.Trim();
        Niche = niche.Trim();
        Email = email?.ToLowerInvariant().Trim();
        PhoneNumber = phoneNumber?.Trim();
        Status = status;
        CreatedAt = DateTimeOffset.UtcNow;
        IsDeleted = false;
    }

    public static Creator Create(
        Guid organizationId,
        string fullName,
        string niche,
        string? email = null,
        string? phoneNumber = null,
        IEnumerable<SocialAccount>? socialAccounts = null,
        CreatorStatus status = CreatorStatus.Active)
    {
        var creator = new Creator(Guid.NewGuid(), organizationId, fullName, niche, email, phoneNumber, status);
        if (socialAccounts != null)
        {
            creator._socialAccounts.AddRange(socialAccounts);
        }

        creator.AddDomainEvent(new CreatorCreatedDomainEvent(creator.Id, creator.OrganizationId, creator.FullName, creator.Niche, creator.CreatedAt));
        return creator;
    }

    public void UpdateDetails(string fullName, string niche, string? email, string? phoneNumber)
    {
        FullName = fullName.Trim();
        Niche = niche.Trim();
        Email = email?.ToLowerInvariant().Trim();
        PhoneNumber = phoneNumber?.Trim();
        UpdatedAt = DateTimeOffset.UtcNow;

        AddDomainEvent(new CreatorUpdatedDomainEvent(Id, OrganizationId, FullName, UpdatedAt.Value));
    }

    public void SetStatus(CreatorStatus status)
    {
        Status = status;
        UpdatedAt = DateTimeOffset.UtcNow;

        AddDomainEvent(new CreatorStatusChangedDomainEvent(Id, OrganizationId, Status, UpdatedAt.Value));
    }

    public void SetSocialAccounts(IEnumerable<SocialAccount> socialAccounts)
    {
        _socialAccounts.Clear();
        _socialAccounts.AddRange(socialAccounts);
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void AddSocialAccount(SocialAccount account)
    {
        _socialAccounts.Add(account);
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void RemoveSocialAccount(PlatformType platform, string handle)
    {
        var cleanHandle = handle.Trim().TrimStart('@');
        _socialAccounts.RemoveAll(s => s.Platform == platform && string.Equals(s.Handle, cleanHandle, StringComparison.OrdinalIgnoreCase));
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void Delete()
    {
        IsDeleted = true;
        UpdatedAt = DateTimeOffset.UtcNow;

        AddDomainEvent(new CreatorDeletedDomainEvent(Id, OrganizationId, UpdatedAt.Value));
    }
}

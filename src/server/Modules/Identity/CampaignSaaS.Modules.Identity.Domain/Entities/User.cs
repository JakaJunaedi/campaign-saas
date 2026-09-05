namespace CampaignSaaS.Modules.Identity.Domain.Entities;

using CampaignSaaS.Modules.Identity.Domain.Enums;
using CampaignSaaS.Modules.Identity.Domain.Events;
using CampaignSaaS.SharedKernel.Domain;
using CampaignSaaS.SharedKernel.MultiTenancy;

public class User : AggregateRoot<Guid>, ITenantEntity
{
    public Guid OrganizationId { get; private set; }
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public string FullName { get; private set; } = string.Empty;
    public UserRole Role { get; private set; } = UserRole.AgencyOwner;
    public bool IsActive { get; private set; } = true;
    public DateTimeOffset CreatedAt { get; private set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? UpdatedAt { get; private set; }

    private User() { }

    private User(Guid id, Guid organizationId, string email, string passwordHash, string fullName, UserRole role)
        : base(id)
    {
        OrganizationId = organizationId;
        Email = email.ToLowerInvariant().Trim();
        PasswordHash = passwordHash;
        FullName = fullName.Trim();
        Role = role;
        IsActive = true;
        CreatedAt = DateTimeOffset.UtcNow;
    }

    public static User Create(Guid organizationId, string email, string passwordHash, string fullName, UserRole role)
    {
        var user = new User(Guid.NewGuid(), organizationId, email, passwordHash, fullName, role);
        user.AddDomainEvent(new UserRegisteredDomainEvent(user.Id, user.OrganizationId, user.Email, user.Role, user.CreatedAt));
        return user;
    }

    public void UpdateProfile(string fullName)
    {
        FullName = fullName.Trim();
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void ChangePassword(string passwordHash)
    {
        PasswordHash = passwordHash;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void ChangeRole(UserRole role)
    {
        Role = role;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}

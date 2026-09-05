namespace CampaignSaaS.Modules.Identity.Domain.Entities;

using CampaignSaaS.SharedKernel.Domain;
using CampaignSaaS.SharedKernel.MultiTenancy;

public class RefreshToken : Entity<Guid>, ITenantEntity
{
    public Guid OrganizationId { get; private set; }
    public Guid UserId { get; private set; }
    public string Token { get; private set; } = string.Empty;
    public DateTimeOffset ExpiresAt { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? RevokedAt { get; private set; }
    public string? ReplacedByToken { get; private set; }

    public bool IsExpired => DateTimeOffset.UtcNow >= ExpiresAt;
    public bool IsRevoked => RevokedAt != null;
    public bool IsActive => !IsRevoked && !IsExpired;

    private RefreshToken() { }

    private RefreshToken(Guid id, Guid organizationId, Guid userId, string token, DateTimeOffset expiresAt)
        : base(id)
    {
        OrganizationId = organizationId;
        UserId = userId;
        Token = token;
        ExpiresAt = expiresAt;
        CreatedAt = DateTimeOffset.UtcNow;
    }

    public static RefreshToken Create(Guid organizationId, Guid userId, string token, DateTimeOffset expiresAt)
    {
        return new RefreshToken(Guid.NewGuid(), organizationId, userId, token, expiresAt);
    }

    public void Revoke(string? replacedByToken = null)
    {
        RevokedAt = DateTimeOffset.UtcNow;
        ReplacedByToken = replacedByToken;
    }
}

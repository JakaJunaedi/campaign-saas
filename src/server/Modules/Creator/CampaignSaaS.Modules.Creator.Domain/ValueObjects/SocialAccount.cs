namespace CampaignSaaS.Modules.Creator.Domain.ValueObjects;

using CampaignSaaS.Modules.Creator.Domain.Enums;
using CampaignSaaS.SharedKernel.Domain;

public class SocialAccount : ValueObject
{
    public PlatformType Platform { get; private set; }
    public string Handle { get; private set; } = string.Empty;
    public string ProfileUrl { get; private set; } = string.Empty;
    public long FollowerCount { get; private set; }

    private SocialAccount() { }

    public SocialAccount(PlatformType platform, string handle, string profileUrl, long followerCount)
    {
        Platform = platform;
        Handle = handle.Trim().TrimStart('@');
        ProfileUrl = profileUrl.Trim();
        FollowerCount = Math.Max(0, followerCount);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Platform;
        yield return Handle.ToLowerInvariant();
        yield return ProfileUrl.ToLowerInvariant();
        yield return FollowerCount;
    }
}

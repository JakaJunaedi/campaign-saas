namespace CampaignSaaS.Modules.Identity.Infrastructure.Services;

public class JwtOptions
{
    public const string SectionName = "JwtOptions";

    public string SecretKey { get; set; } = "SuperSecretKeyForCampaignSaaSApp2026_Minimum32Chars!";
    public string Issuer { get; set; } = "CampaignSaaS";
    public string Audience { get; set; } = "CampaignSaaSClient";
    public int ExpirationMinutes { get; set; } = 15;
    public int RefreshTokenExpirationDays { get; set; } = 7;
}

namespace CampaignSaaS.Modules.Identity.Application.Abstractions;

using CampaignSaaS.Modules.Identity.Domain.Entities;

public interface IJwtTokenService
{
    string GenerateAccessToken(User user, Organization organization);
    (string Token, DateTimeOffset ExpiresAt) GenerateRefreshToken();
    int AccessTokenExpirationMinutes { get; }
}

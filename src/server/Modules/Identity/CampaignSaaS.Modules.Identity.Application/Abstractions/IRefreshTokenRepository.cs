namespace CampaignSaaS.Modules.Identity.Application.Abstractions;

using CampaignSaaS.Modules.Identity.Domain.Entities;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default);
    Task AddAsync(RefreshToken token, CancellationToken cancellationToken = default);
    void Update(RefreshToken token);
    Task RevokeAllUserTokensAsync(Guid userId, CancellationToken cancellationToken = default);
}

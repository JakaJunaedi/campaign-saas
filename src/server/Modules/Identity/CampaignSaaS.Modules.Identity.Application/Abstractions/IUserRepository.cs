namespace CampaignSaaS.Modules.Identity.Application.Abstractions;

using CampaignSaaS.Modules.Identity.Domain.Entities;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<User?> GetByEmailAsync(Guid organizationId, string email, CancellationToken cancellationToken = default);
    Task<User?> GetByEmailGlobalAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> ExistsByEmailAsync(Guid organizationId, string email, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<User>> GetByOrganizationIdAsync(Guid organizationId, CancellationToken cancellationToken = default);
    Task AddAsync(User user, CancellationToken cancellationToken = default);
    void Update(User user);
}

namespace CampaignSaaS.Modules.Identity.Infrastructure.DataSeeding;

using CampaignSaaS.Modules.Identity.Application.Abstractions;
using CampaignSaaS.Modules.Identity.Domain.Entities;
using CampaignSaaS.Modules.Identity.Domain.Enums;
using Microsoft.Extensions.Logging;

/// <summary>
/// Seeds the documented development accounts (see docs/06-implementation/LOCAL_DEVELOPMENT_GUIDE.md §5).
/// Idempotent: skips any organization (by slug) or user (by email) that already exists.
/// Intended to run only in Development environment.
/// </summary>
public sealed class IdentityDataSeeder
{
    // NOTE: not Guid.Empty — EF Core's default Guid key generation treats Guid.Empty
    // as "unset" and auto-generates a new value, which would break the fixed reference.
    public static readonly Guid SystemOrganizationId = Guid.Parse("22222222-2222-2222-2222-222222222222");
    public static readonly Guid AgencyAlphaOrganizationId = Guid.Parse("11111111-1111-1111-1111-111111111111");

    private readonly IOrganizationRepository _organizationRepository;
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IIdentityUnitOfWork _unitOfWork;
    private readonly ILogger<IdentityDataSeeder> _logger;

    public IdentityDataSeeder(
        IOrganizationRepository organizationRepository,
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IIdentityUnitOfWork unitOfWork,
        ILogger<IdentityDataSeeder> logger)
    {
        _organizationRepository = organizationRepository;
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        await EnsureOrganizationAsync(SystemOrganizationId, "Campaign SaaS System", "campaign-saas-system", cancellationToken);
        await EnsureOrganizationAsync(AgencyAlphaOrganizationId, "Agency Alpha", "agency-alpha", cancellationToken);

        await EnsureUserAsync(SystemOrganizationId, "superadmin@campaignsaas.local", "SuperAdminDev123!", "Alex (SuperAdmin)", UserRole.SuperAdmin, cancellationToken);
        await EnsureUserAsync(AgencyAlphaOrganizationId, "owner@agency-alpha.local", "OwnerDev123!", "Ryan (Agency Owner)", UserRole.AgencyOwner, cancellationToken);
        await EnsureUserAsync(AgencyAlphaOrganizationId, "cm@agency-alpha.local", "CmDev123!", "Sarah (Campaign Manager)", UserRole.CampaignManager, cancellationToken);
        await EnsureUserAsync(AgencyAlphaOrganizationId, "reviewer@agency-alpha.local", "ReviewerDev123!", "Dimas (Content Reviewer)", UserRole.ContentReviewer, cancellationToken);
        await EnsureUserAsync(AgencyAlphaOrganizationId, "creator@beauty.local", "CreatorDev123!", "Amanda (Creator)", UserRole.Creator, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Development identity seeding completed.");
    }

    private async Task EnsureOrganizationAsync(Guid id, string name, string slug, CancellationToken cancellationToken)
    {
        var existing = await _organizationRepository.GetBySlugAsync(slug, cancellationToken);
        if (existing is not null)
        {
            return;
        }

        var organization = Organization.Create(id, name, slug, OrganizationStatus.Active);
        await _organizationRepository.AddAsync(organization, cancellationToken);
        _logger.LogInformation("Seeded organization '{Slug}' ({Id}).", slug, id);
    }

    private async Task EnsureUserAsync(Guid organizationId, string email, string password, string fullName, UserRole role, CancellationToken cancellationToken)
    {
        var existing = await _userRepository.GetByEmailGlobalAsync(email, cancellationToken);
        if (existing is not null)
        {
            return;
        }

        var passwordHash = _passwordHasher.HashPassword(password);
        var user = User.Create(organizationId, email, passwordHash, fullName, role);
        await _userRepository.AddAsync(user, cancellationToken);
        _logger.LogInformation("Seeded user '{Email}' ({Role}).", email, role);
    }
}

namespace CampaignSaaS.Modules.Identity.Application.Commands.CreateOrganization;

using CampaignSaaS.Modules.Identity.Application.Abstractions;
using CampaignSaaS.Modules.Identity.Contracts.DTOs;
using CampaignSaaS.Modules.Identity.Domain.Entities;
using CampaignSaaS.Modules.Identity.Domain.Enums;
using ErrorOr;
using MediatR;

public class CreateOrganizationCommandHandler : IRequestHandler<CreateOrganizationCommand, ErrorOr<AdminOrganizationItemDto>>
{
    private readonly IOrganizationRepository _organizationRepository;
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IIdentityUnitOfWork _unitOfWork;

    public CreateOrganizationCommandHandler(
        IOrganizationRepository organizationRepository,
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IIdentityUnitOfWork unitOfWork)
    {
        _organizationRepository = organizationRepository;
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<AdminOrganizationItemDto>> Handle(CreateOrganizationCommand request, CancellationToken cancellationToken)
    {
        var slug = request.Slug.ToLowerInvariant().Trim();
        if (await _organizationRepository.ExistsBySlugAsync(slug, cancellationToken))
        {
            return Error.Conflict("Organization.DuplicateSlug", $"Organization slug '{slug}' is already taken.");
        }

        var normalizedEmail = request.AdminEmail.ToLowerInvariant().Trim();
        var existingUser = await _userRepository.GetByEmailGlobalAsync(normalizedEmail, cancellationToken);
        if (existingUser != null)
        {
            return Error.Conflict("User.DuplicateEmail", $"User with email '{normalizedEmail}' is already registered.");
        }

        var organization = Organization.Create(request.OrganizationName, slug, OrganizationStatus.Active);
        await _organizationRepository.AddAsync(organization, cancellationToken);

        var passwordHash = _passwordHasher.HashPassword(request.Password);
        var adminUser = User.Create(
            organization.Id,
            normalizedEmail,
            passwordHash,
            request.AdminFullName,
            UserRole.AgencyOwner);

        await _userRepository.AddAsync(adminUser, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new AdminOrganizationItemDto(
            organization.Id,
            organization.Name,
            organization.Slug,
            organization.Status.ToString(),
            1, // Only 1 user created initially
            organization.CreatedAt);
    }
}

namespace CampaignSaaS.Modules.Identity.Application.Commands.RegisterOrganization;

using CampaignSaaS.Modules.Identity.Application.Abstractions;
using CampaignSaaS.Modules.Identity.Contracts.DTOs;
using CampaignSaaS.Modules.Identity.Domain.Entities;
using CampaignSaaS.Modules.Identity.Domain.Enums;
using ErrorOr;
using MediatR;

public class RegisterOrganizationCommandHandler : IRequestHandler<RegisterOrganizationCommand, ErrorOr<AuthResultDto>>
{
    private readonly IOrganizationRepository _organizationRepository;
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IIdentityUnitOfWork _unitOfWork;

    public RegisterOrganizationCommandHandler(
        IOrganizationRepository organizationRepository,
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenService jwtTokenService,
        IIdentityUnitOfWork unitOfWork)
    {
        _organizationRepository = organizationRepository;
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<AuthResultDto>> Handle(RegisterOrganizationCommand request, CancellationToken cancellationToken)
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

        var accessToken = _jwtTokenService.GenerateAccessToken(adminUser, organization);
        var (refreshTokenString, refreshTokenExpiry) = _jwtTokenService.GenerateRefreshToken();

        var refreshToken = RefreshToken.Create(organization.Id, adminUser.Id, refreshTokenString, refreshTokenExpiry);
        await _refreshTokenRepository.AddAsync(refreshToken, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var userDto = new UserDto(
            adminUser.Id,
            adminUser.OrganizationId,
            adminUser.Email,
            adminUser.FullName,
            adminUser.Role.ToString(),
            adminUser.IsActive,
            adminUser.CreatedAt);

        var orgDto = new OrganizationDto(
            organization.Id,
            organization.Name,
            organization.Slug,
            organization.Status.ToString(),
            organization.CreatedAt);

        return new AuthResultDto(
            accessToken,
            refreshTokenString,
            _jwtTokenService.AccessTokenExpirationMinutes * 60,
            userDto,
            orgDto);
    }
}

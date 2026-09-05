namespace CampaignSaaS.Modules.Identity.Application.Commands.Login;

using CampaignSaaS.Modules.Identity.Application.Abstractions;
using CampaignSaaS.Modules.Identity.Contracts.DTOs;
using CampaignSaaS.Modules.Identity.Domain.Entities;
using CampaignSaaS.Modules.Identity.Domain.Enums;
using ErrorOr;
using MediatR;

public class LoginCommandHandler : IRequestHandler<LoginCommand, ErrorOr<AuthResultDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IOrganizationRepository _organizationRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IIdentityUnitOfWork _unitOfWork;

    public LoginCommandHandler(
        IUserRepository userRepository,
        IOrganizationRepository organizationRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenService jwtTokenService,
        IIdentityUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _organizationRepository = organizationRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<AuthResultDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var normalizedEmail = request.Email.ToLowerInvariant().Trim();
        var user = await _userRepository.GetByEmailGlobalAsync(normalizedEmail, cancellationToken);

        if (user == null || !user.IsActive)
        {
            return Error.Unauthorized("Auth.InvalidCredentials", "Invalid email or password.");
        }

        if (!_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            return Error.Unauthorized("Auth.InvalidCredentials", "Invalid email or password.");
        }

        var organization = await _organizationRepository.GetByIdAsync(user.OrganizationId, cancellationToken);
        if (organization == null || organization.IsDeleted || organization.Status == OrganizationStatus.Suspended)
        {
            return Error.Unauthorized("Auth.OrganizationSuspended", "Organization is suspended or does not exist.");
        }

        var accessToken = _jwtTokenService.GenerateAccessToken(user, organization);
        var (refreshTokenString, refreshTokenExpiry) = _jwtTokenService.GenerateRefreshToken();

        var refreshToken = RefreshToken.Create(organization.Id, user.Id, refreshTokenString, refreshTokenExpiry);
        await _refreshTokenRepository.AddAsync(refreshToken, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var userDto = new UserDto(
            user.Id,
            user.OrganizationId,
            user.Email,
            user.FullName,
            user.Role.ToString(),
            user.IsActive,
            user.CreatedAt);

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

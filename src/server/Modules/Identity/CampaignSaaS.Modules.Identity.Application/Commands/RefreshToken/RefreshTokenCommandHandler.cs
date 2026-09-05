namespace CampaignSaaS.Modules.Identity.Application.Commands.RefreshToken;

using CampaignSaaS.Modules.Identity.Application.Abstractions;
using CampaignSaaS.Modules.Identity.Contracts.DTOs;
using CampaignSaaS.Modules.Identity.Domain.Entities;
using CampaignSaaS.Modules.Identity.Domain.Enums;
using ErrorOr;
using MediatR;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, ErrorOr<AuthResultDto>>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUserRepository _userRepository;
    private readonly IOrganizationRepository _organizationRepository;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IIdentityUnitOfWork _unitOfWork;

    public RefreshTokenCommandHandler(
        IRefreshTokenRepository refreshTokenRepository,
        IUserRepository userRepository,
        IOrganizationRepository organizationRepository,
        IJwtTokenService jwtTokenService,
        IIdentityUnitOfWork unitOfWork)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _userRepository = userRepository;
        _organizationRepository = organizationRepository;
        _jwtTokenService = jwtTokenService;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<AuthResultDto>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var existingToken = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken, cancellationToken);
        if (existingToken == null || !existingToken.IsActive)
        {
            return Error.Unauthorized("Auth.InvalidRefreshToken", "Refresh token is invalid, expired, or revoked.");
        }

        var user = await _userRepository.GetByIdAsync(existingToken.UserId, cancellationToken);
        if (user == null || !user.IsActive)
        {
            return Error.Unauthorized("Auth.UserInactive", "User account is inactive or not found.");
        }

        var organization = await _organizationRepository.GetByIdAsync(existingToken.OrganizationId, cancellationToken);
        if (organization == null || organization.IsDeleted || organization.Status == OrganizationStatus.Suspended)
        {
            return Error.Unauthorized("Auth.OrganizationSuspended", "Organization is suspended or does not exist.");
        }

        // Generate new tokens
        var accessToken = _jwtTokenService.GenerateAccessToken(user, organization);
        var (newRefreshTokenString, newRefreshTokenExpiry) = _jwtTokenService.GenerateRefreshToken();

        // Rotate token
        existingToken.Revoke(newRefreshTokenString);
        _refreshTokenRepository.Update(existingToken);

        var newRefreshToken = RefreshToken.Create(organization.Id, user.Id, newRefreshTokenString, newRefreshTokenExpiry);
        await _refreshTokenRepository.AddAsync(newRefreshToken, cancellationToken);

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
            newRefreshTokenString,
            _jwtTokenService.AccessTokenExpirationMinutes * 60,
            userDto,
            orgDto);
    }
}

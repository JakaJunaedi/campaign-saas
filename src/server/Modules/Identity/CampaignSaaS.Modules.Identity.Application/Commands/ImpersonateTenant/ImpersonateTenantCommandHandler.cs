namespace CampaignSaaS.Modules.Identity.Application.Commands.ImpersonateTenant;

using CampaignSaaS.Modules.Identity.Application.Abstractions;
using CampaignSaaS.Modules.Identity.Contracts.DTOs;
using CampaignSaaS.Modules.Identity.Domain.Enums;
using ErrorOr;
using MediatR;

public class ImpersonateTenantCommandHandler : IRequestHandler<ImpersonateTenantCommand, ErrorOr<AuthResultDto>>
{
    private readonly IOrganizationRepository _organizationRepository;
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenService _jwtTokenService;

    public ImpersonateTenantCommandHandler(
        IOrganizationRepository organizationRepository,
        IUserRepository userRepository,
        IJwtTokenService jwtTokenService)
    {
        _organizationRepository = organizationRepository;
        _userRepository = userRepository;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<ErrorOr<AuthResultDto>> Handle(ImpersonateTenantCommand request, CancellationToken cancellationToken)
    {
        var organization = await _organizationRepository.GetByIdAsync(request.OrganizationId, cancellationToken);
        if (organization == null)
        {
            return Error.NotFound("Organization.NotFound", $"Organization with ID '{request.OrganizationId}' was not found.");
        }

        if (organization.Status != OrganizationStatus.Active)
        {
            return Error.Validation("Organization.NotActive", "Cannot impersonate an inactive organization.");
        }

        var users = await _userRepository.GetByOrganizationIdAsync(request.OrganizationId, cancellationToken);
        var adminUser = users.FirstOrDefault(u => u.Role == UserRole.AgencyOwner && u.IsActive) 
                     ?? users.FirstOrDefault(u => u.IsActive);

        if (adminUser == null)
        {
            return Error.NotFound("Organization.NoActiveUsers", "Organization has no active users to impersonate.");
        }

        var accessToken = _jwtTokenService.GenerateAccessToken(adminUser, organization);
        // We do not generate a refresh token for impersonation, just access token
        var refreshTokenString = string.Empty;

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

namespace CampaignSaaS.Modules.Identity.Application.Queries.GetOrganizationUsers;

using CampaignSaaS.Modules.Identity.Application.Abstractions;
using CampaignSaaS.Modules.Identity.Contracts.DTOs;
using CampaignSaaS.SharedKernel.MultiTenancy;
using ErrorOr;
using MediatR;

public class GetOrganizationUsersQueryHandler : IRequestHandler<GetOrganizationUsersQuery, ErrorOr<IReadOnlyList<UserDto>>>
{
    private readonly IUserRepository _userRepository;
    private readonly ICurrentTenantContext _tenantContext;

    public GetOrganizationUsersQueryHandler(
        IUserRepository userRepository,
        ICurrentTenantContext tenantContext)
    {
        _userRepository = userRepository;
        _tenantContext = tenantContext;
    }

    public async Task<ErrorOr<IReadOnlyList<UserDto>>> Handle(GetOrganizationUsersQuery request, CancellationToken cancellationToken)
    {
        if (!_tenantContext.OrganizationId.HasValue)
        {
            return Error.Forbidden("Tenant.Required", "Active organization context is missing.");
        }

        var users = await _userRepository.GetByOrganizationIdAsync(_tenantContext.OrganizationId.Value, cancellationToken);

        var dtos = users.Select(u => new UserDto(
            u.Id,
            u.OrganizationId,
            u.Email,
            u.FullName,
            u.Role.ToString(),
            u.IsActive,
            u.CreatedAt)).ToList();

        return dtos;
    }
}

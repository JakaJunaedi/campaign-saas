namespace CampaignSaaS.Modules.Identity.Application.Queries.GetAdminOverview;

using CampaignSaaS.Modules.Identity.Application.Abstractions;
using CampaignSaaS.Modules.Identity.Contracts.DTOs;
using ErrorOr;
using MediatR;

public class GetAdminOverviewQueryHandler : IRequestHandler<GetAdminOverviewQuery, ErrorOr<AdminOverviewDto>>
{
    private readonly IOrganizationRepository _organizationRepository;
    private readonly IUserRepository _userRepository;

    public GetAdminOverviewQueryHandler(
        IOrganizationRepository organizationRepository,
        IUserRepository userRepository)
    {
        _organizationRepository = organizationRepository;
        _userRepository = userRepository;
    }

    public async Task<ErrorOr<AdminOverviewDto>> Handle(GetAdminOverviewQuery request, CancellationToken cancellationToken)
    {
        var (allOrgs, totalTenants) = await _organizationRepository.GetPagedAsync(null, null, 1, 1000, cancellationToken);
        var activeTenants = allOrgs.Count(o => o.Status == Domain.Enums.OrganizationStatus.Active);
        var totalUsers = await _userRepository.GetTotalUsersCountAsync(cancellationToken);

        return new AdminOverviewDto(
            totalTenants,
            totalUsers,
            activeTenants,
            "Healthy",
            DateTimeOffset.UtcNow);
    }
}


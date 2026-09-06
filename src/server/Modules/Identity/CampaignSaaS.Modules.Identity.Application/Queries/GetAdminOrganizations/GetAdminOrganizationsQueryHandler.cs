namespace CampaignSaaS.Modules.Identity.Application.Queries.GetAdminOrganizations;

using CampaignSaaS.Modules.Identity.Application.Abstractions;
using CampaignSaaS.Modules.Identity.Contracts.DTOs;
using CampaignSaaS.Modules.Identity.Domain.Enums;
using ErrorOr;
using MediatR;

public class GetAdminOrganizationsQueryHandler : IRequestHandler<GetAdminOrganizationsQuery, ErrorOr<PagedResult<AdminOrganizationItemDto>>>
{
    private readonly IOrganizationRepository _organizationRepository;
    private readonly IUserRepository _userRepository;

    public GetAdminOrganizationsQueryHandler(
        IOrganizationRepository organizationRepository,
        IUserRepository userRepository)
    {
        _organizationRepository = organizationRepository;
        _userRepository = userRepository;
    }

    public async Task<ErrorOr<PagedResult<AdminOrganizationItemDto>>> Handle(GetAdminOrganizationsQuery request, CancellationToken cancellationToken)
    {
        OrganizationStatus? statusFilter = null;
        if (!string.IsNullOrWhiteSpace(request.Status) && Enum.TryParse<OrganizationStatus>(request.Status, true, out var parsedStatus))
        {
            statusFilter = parsedStatus;
        }

        var (items, totalCount) = await _organizationRepository.GetPagedAsync(
            request.Search,
            statusFilter,
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        var dtoList = new List<AdminOrganizationItemDto>(items.Count);
        foreach (var org in items)
        {
            var userCount = await _userRepository.GetUsersCountByOrganizationIdAsync(org.Id, cancellationToken);
            dtoList.Add(new AdminOrganizationItemDto(
                org.Id,
                org.Name,
                org.Slug,
                org.Status.ToString(),
                userCount,
                org.CreatedAt));
        }

        var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);
        return new PagedResult<AdminOrganizationItemDto>(
            dtoList,
            totalCount,
            request.PageNumber,
            request.PageSize,
            totalPages);
    }
}

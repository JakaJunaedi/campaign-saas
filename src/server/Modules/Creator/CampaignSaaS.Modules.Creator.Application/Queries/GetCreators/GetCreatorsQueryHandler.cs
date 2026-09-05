namespace CampaignSaaS.Modules.Creator.Application.Queries.GetCreators;

using CampaignSaaS.Modules.Creator.Application.Abstractions;
using CampaignSaaS.Modules.Creator.Contracts.DTOs;
using CampaignSaaS.Modules.Creator.Domain.Enums;
using CampaignSaaS.SharedKernel.MultiTenancy;
using ErrorOr;
using MediatR;

public class GetCreatorsQueryHandler : IRequestHandler<GetCreatorsQuery, ErrorOr<PagedResult<CreatorSummaryDto>>>
{
    private readonly ICreatorRepository _creatorRepository;
    private readonly ICurrentTenantContext _tenantContext;

    public GetCreatorsQueryHandler(
        ICreatorRepository creatorRepository,
        ICurrentTenantContext tenantContext)
    {
        _creatorRepository = creatorRepository;
        _tenantContext = tenantContext;
    }

    public async Task<ErrorOr<PagedResult<CreatorSummaryDto>>> Handle(GetCreatorsQuery request, CancellationToken cancellationToken)
    {
        if (!_tenantContext.OrganizationId.HasValue)
        {
            return Error.Forbidden("Tenant.Required", "Active organization context is required.");
        }

        PlatformType? platformFilter = null;
        if (!string.IsNullOrWhiteSpace(request.Platform) && Enum.TryParse<PlatformType>(request.Platform, true, out var parsedPlatform))
        {
            platformFilter = parsedPlatform;
        }

        CreatorStatus? statusFilter = null;
        if (!string.IsNullOrWhiteSpace(request.Status) && Enum.TryParse<CreatorStatus>(request.Status, true, out var parsedStatus))
        {
            statusFilter = parsedStatus;
        }

        var pageNumber = Math.Max(1, request.PageNumber);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);

        var (items, totalCount) = await _creatorRepository.GetPagedAsync(
            _tenantContext.OrganizationId.Value,
            request.SearchTerm,
            request.Niche,
            platformFilter,
            statusFilter,
            pageNumber,
            pageSize,
            cancellationToken);

        var dtos = items.Select(c => new CreatorSummaryDto(
            c.Id,
            c.OrganizationId,
            c.FullName,
            c.Email,
            c.PhoneNumber,
            c.Niche,
            c.Status.ToString(),
            c.SocialAccounts.Count,
            c.SocialAccounts.Sum(s => s.FollowerCount),
            c.CreatedAt)).ToList();

        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        return new PagedResult<CreatorSummaryDto>(
            dtos,
            totalCount,
            pageNumber,
            pageSize,
            totalPages);
    }
}

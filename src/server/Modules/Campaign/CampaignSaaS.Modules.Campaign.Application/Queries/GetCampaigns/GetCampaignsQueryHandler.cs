namespace CampaignSaaS.Modules.Campaign.Application.Queries.GetCampaigns;

using CampaignSaaS.Modules.Campaign.Application.Abstractions;
using CampaignSaaS.Modules.Campaign.Contracts.DTOs;
using CampaignSaaS.Modules.Campaign.Domain.Enums;
using CampaignSaaS.SharedKernel.MultiTenancy;
using ErrorOr;
using MediatR;

public class GetCampaignsQueryHandler : IRequestHandler<GetCampaignsQuery, ErrorOr<PagedResult<CampaignSummaryDto>>>
{
    private readonly ICampaignRepository _campaignRepository;
    private readonly ICurrentTenantContext _tenantContext;

    public GetCampaignsQueryHandler(
        ICampaignRepository campaignRepository,
        ICurrentTenantContext tenantContext)
    {
        _campaignRepository = campaignRepository;
        _tenantContext = tenantContext;
    }

    public async Task<ErrorOr<PagedResult<CampaignSummaryDto>>> Handle(GetCampaignsQuery request, CancellationToken cancellationToken)
    {
        if (!_tenantContext.OrganizationId.HasValue)
        {
            return Error.Forbidden("Tenant.Required", "Active organization context is required.");
        }

        CampaignStatus? statusFilter = null;
        if (!string.IsNullOrWhiteSpace(request.Status) && Enum.TryParse<CampaignStatus>(request.Status, true, out var parsedStatus))
        {
            statusFilter = parsedStatus;
        }

        var pageNumber = Math.Max(1, request.PageNumber);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);

        var (items, totalCount) = await _campaignRepository.GetPagedAsync(
            _tenantContext.OrganizationId.Value,
            request.SearchTerm,
            request.ClientId,
            statusFilter,
            pageNumber,
            pageSize,
            cancellationToken);

        var summaryDtos = new List<CampaignSummaryDto>();
        foreach (var c in items)
        {
            var creatorsCount = await _campaignRepository.GetCreatorsCountAsync(c.Id, cancellationToken);
            summaryDtos.Add(new CampaignSummaryDto(
                c.Id,
                c.OrganizationId,
                c.ClientId,
                c.Title,
                c.Budget,
                c.StartDate,
                c.EndDate,
                c.Status.ToString(),
                creatorsCount,
                c.CreatedAt));
        }

        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        return new PagedResult<CampaignSummaryDto>(
            summaryDtos,
            totalCount,
            pageNumber,
            pageSize,
            totalPages);
    }
}

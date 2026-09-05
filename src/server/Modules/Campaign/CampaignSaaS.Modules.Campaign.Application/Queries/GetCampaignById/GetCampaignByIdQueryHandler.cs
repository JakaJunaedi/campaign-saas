namespace CampaignSaaS.Modules.Campaign.Application.Queries.GetCampaignById;

using CampaignSaaS.Modules.Campaign.Application.Abstractions;
using CampaignSaaS.Modules.Campaign.Contracts.DTOs;
using CampaignSaaS.SharedKernel.MultiTenancy;
using ErrorOr;
using MediatR;

public class GetCampaignByIdQueryHandler : IRequestHandler<GetCampaignByIdQuery, ErrorOr<CampaignDto>>
{
    private readonly ICampaignRepository _campaignRepository;
    private readonly ICurrentTenantContext _tenantContext;

    public GetCampaignByIdQueryHandler(
        ICampaignRepository campaignRepository,
        ICurrentTenantContext tenantContext)
    {
        _campaignRepository = campaignRepository;
        _tenantContext = tenantContext;
    }

    public async Task<ErrorOr<CampaignDto>> Handle(GetCampaignByIdQuery request, CancellationToken cancellationToken)
    {
        if (!_tenantContext.OrganizationId.HasValue)
        {
            return Error.Forbidden("Tenant.Required", "Active organization context is required.");
        }

        var campaign = await _campaignRepository.GetByIdAsync(request.Id, cancellationToken);
        if (campaign == null || campaign.IsDeleted)
        {
            return Error.NotFound("Campaign.NotFound", "Campaign not found.");
        }

        return new CampaignDto(
            campaign.Id,
            campaign.OrganizationId,
            campaign.ClientId,
            campaign.Title,
            campaign.Description,
            campaign.Budget,
            campaign.StartDate,
            campaign.EndDate,
            campaign.Status.ToString(),
            campaign.CreatedAt,
            campaign.UpdatedAt);
    }
}

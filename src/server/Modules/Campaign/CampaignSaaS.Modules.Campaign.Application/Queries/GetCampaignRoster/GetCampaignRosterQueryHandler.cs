namespace CampaignSaaS.Modules.Campaign.Application.Queries.GetCampaignRoster;

using CampaignSaaS.Modules.Campaign.Application.Abstractions;
using CampaignSaaS.Modules.Campaign.Contracts.DTOs;
using CampaignSaaS.SharedKernel.MultiTenancy;
using ErrorOr;
using MediatR;

public class GetCampaignRosterQueryHandler : IRequestHandler<GetCampaignRosterQuery, ErrorOr<IReadOnlyList<CampaignCreatorDto>>>
{
    private readonly ICampaignCreatorRepository _rosterRepository;
    private readonly ICurrentTenantContext _tenantContext;

    public GetCampaignRosterQueryHandler(
        ICampaignCreatorRepository rosterRepository,
        ICurrentTenantContext tenantContext)
    {
        _rosterRepository = rosterRepository;
        _tenantContext = tenantContext;
    }

    public async Task<ErrorOr<IReadOnlyList<CampaignCreatorDto>>> Handle(GetCampaignRosterQuery request, CancellationToken cancellationToken)
    {
        if (!_tenantContext.OrganizationId.HasValue)
        {
            return Error.Forbidden("Tenant.Required", "Active organization context is required.");
        }

        var roster = await _rosterRepository.GetRosterByCampaignIdAsync(request.CampaignId, cancellationToken);

        var dtos = roster.Select(r => new CampaignCreatorDto(
            r.Id,
            r.OrganizationId,
            r.CampaignId,
            r.CreatorId,
            r.Status.ToString(),
            r.AgreedRate,
            r.CreatedAt,
            r.UpdatedAt)).ToList();

        return dtos;
    }
}

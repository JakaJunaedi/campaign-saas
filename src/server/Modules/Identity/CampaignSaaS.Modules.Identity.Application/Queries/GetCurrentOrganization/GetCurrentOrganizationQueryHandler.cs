namespace CampaignSaaS.Modules.Identity.Application.Queries.GetCurrentOrganization;

using CampaignSaaS.Modules.Identity.Application.Abstractions;
using CampaignSaaS.Modules.Identity.Contracts.DTOs;
using CampaignSaaS.SharedKernel.MultiTenancy;
using ErrorOr;
using MediatR;

public class GetCurrentOrganizationQueryHandler : IRequestHandler<GetCurrentOrganizationQuery, ErrorOr<OrganizationDto>>
{
    private readonly IOrganizationRepository _organizationRepository;
    private readonly ICurrentTenantContext _tenantContext;

    public GetCurrentOrganizationQueryHandler(
        IOrganizationRepository organizationRepository,
        ICurrentTenantContext tenantContext)
    {
        _organizationRepository = organizationRepository;
        _tenantContext = tenantContext;
    }

    public async Task<ErrorOr<OrganizationDto>> Handle(GetCurrentOrganizationQuery request, CancellationToken cancellationToken)
    {
        if (!_tenantContext.OrganizationId.HasValue)
        {
            return Error.Forbidden("Tenant.Required", "Active organization context is missing.");
        }

        var organization = await _organizationRepository.GetByIdAsync(_tenantContext.OrganizationId.Value, cancellationToken);
        if (organization == null || organization.IsDeleted)
        {
            return Error.NotFound("Organization.NotFound", "Organization not found.");
        }

        return new OrganizationDto(
            organization.Id,
            organization.Name,
            organization.Slug,
            organization.Status.ToString(),
            organization.CreatedAt);
    }
}

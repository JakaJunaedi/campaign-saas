namespace CampaignSaaS.Modules.Client.Application.Queries.GetClients;

using CampaignSaaS.Modules.Client.Application.Abstractions;
using CampaignSaaS.Modules.Client.Contracts.DTOs;
using CampaignSaaS.SharedKernel.MultiTenancy;
using ErrorOr;
using MediatR;

public class GetClientsQueryHandler : IRequestHandler<GetClientsQuery, ErrorOr<PagedResult<ClientSummaryDto>>>
{
    private readonly IClientRepository _clientRepository;
    private readonly ICurrentTenantContext _tenantContext;

    public GetClientsQueryHandler(
        IClientRepository clientRepository,
        ICurrentTenantContext tenantContext)
    {
        _clientRepository = clientRepository;
        _tenantContext = tenantContext;
    }

    public async Task<ErrorOr<PagedResult<ClientSummaryDto>>> Handle(GetClientsQuery request, CancellationToken cancellationToken)
    {
        if (!_tenantContext.OrganizationId.HasValue)
        {
            return Error.Forbidden("Tenant.Required", "Active organization context is required.");
        }

        var pageNumber = Math.Max(1, request.PageNumber);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);

        var (items, totalCount) = await _clientRepository.GetPagedAsync(
            _tenantContext.OrganizationId.Value,
            request.SearchTerm,
            pageNumber,
            pageSize,
            cancellationToken);

        var dtos = items.Select(c => new ClientSummaryDto(
            c.Id,
            c.OrganizationId,
            c.Name,
            c.CompanyName,
            c.Contacts.Count,
            c.CreatedAt)).ToList();

        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        return new PagedResult<ClientSummaryDto>(
            dtos,
            totalCount,
            pageNumber,
            pageSize,
            totalPages);
    }
}

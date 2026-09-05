namespace CampaignSaaS.Modules.Client.Application.Queries.GetClientById;

using CampaignSaaS.Modules.Client.Application.Abstractions;
using CampaignSaaS.Modules.Client.Contracts.DTOs;
using CampaignSaaS.SharedKernel.MultiTenancy;
using ErrorOr;
using MediatR;

public class GetClientByIdQueryHandler : IRequestHandler<GetClientByIdQuery, ErrorOr<ClientDto>>
{
    private readonly IClientRepository _clientRepository;
    private readonly ICurrentTenantContext _tenantContext;

    public GetClientByIdQueryHandler(
        IClientRepository clientRepository,
        ICurrentTenantContext tenantContext)
    {
        _clientRepository = clientRepository;
        _tenantContext = tenantContext;
    }

    public async Task<ErrorOr<ClientDto>> Handle(GetClientByIdQuery request, CancellationToken cancellationToken)
    {
        if (!_tenantContext.OrganizationId.HasValue)
        {
            return Error.Forbidden("Tenant.Required", "Active organization context is required.");
        }

        var client = await _clientRepository.GetByIdAsync(request.Id, cancellationToken);
        if (client == null || client.IsDeleted)
        {
            return Error.NotFound("Client.NotFound", "Client not found.");
        }

        var contactDtos = client.Contacts
            .Select(c => new ClientContactDto(c.Name, c.Email, c.PhoneNumber, c.Position))
            .ToList();

        return new ClientDto(
            client.Id,
            client.OrganizationId,
            client.Name,
            client.CompanyName,
            contactDtos,
            client.CreatedAt,
            client.UpdatedAt);
    }
}

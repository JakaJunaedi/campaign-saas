namespace CampaignSaaS.Modules.Client.Application.Commands.UpdateClient;

using CampaignSaaS.Modules.Client.Application.Abstractions;
using CampaignSaaS.Modules.Client.Contracts.DTOs;
using CampaignSaaS.Modules.Client.Domain.ValueObjects;
using CampaignSaaS.SharedKernel.MultiTenancy;
using ErrorOr;
using MediatR;

public class UpdateClientCommandHandler : IRequestHandler<UpdateClientCommand, ErrorOr<ClientDto>>
{
    private readonly IClientRepository _clientRepository;
    private readonly IClientUnitOfWork _unitOfWork;
    private readonly ICurrentTenantContext _tenantContext;

    public UpdateClientCommandHandler(
        IClientRepository clientRepository,
        IClientUnitOfWork unitOfWork,
        ICurrentTenantContext tenantContext)
    {
        _clientRepository = clientRepository;
        _unitOfWork = unitOfWork;
        _tenantContext = tenantContext;
    }

    public async Task<ErrorOr<ClientDto>> Handle(UpdateClientCommand request, CancellationToken cancellationToken)
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

        var orgId = _tenantContext.OrganizationId.Value;
        var trimmedName = request.Name.Trim();

        if (await _clientRepository.ExistsByNameAsync(orgId, trimmedName, client.Id, cancellationToken))
        {
            return Error.Conflict("Client.DuplicateName", $"Client with name '{trimmedName}' already exists in this organization.");
        }

        client.Update(trimmedName, request.CompanyName);

        if (request.Contacts != null)
        {
            var contacts = request.Contacts
                .Select(c => new ClientContact(c.Name, c.Email, c.PhoneNumber, c.Position))
                .ToList();

            client.SetContacts(contacts);
        }

        _clientRepository.Update(client);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

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

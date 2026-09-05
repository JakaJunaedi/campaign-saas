namespace CampaignSaaS.Modules.Client.Application.Commands.DeleteClient;

using CampaignSaaS.Modules.Client.Application.Abstractions;
using CampaignSaaS.SharedKernel.MultiTenancy;
using ErrorOr;
using MediatR;

public class DeleteClientCommandHandler : IRequestHandler<DeleteClientCommand, ErrorOr<Success>>
{
    private readonly IClientRepository _clientRepository;
    private readonly IClientUnitOfWork _unitOfWork;
    private readonly ICurrentTenantContext _tenantContext;

    public DeleteClientCommandHandler(
        IClientRepository clientRepository,
        IClientUnitOfWork unitOfWork,
        ICurrentTenantContext tenantContext)
    {
        _clientRepository = clientRepository;
        _unitOfWork = unitOfWork;
        _tenantContext = tenantContext;
    }

    public async Task<ErrorOr<Success>> Handle(DeleteClientCommand request, CancellationToken cancellationToken)
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

        client.Delete();
        _clientRepository.Update(client);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }
}

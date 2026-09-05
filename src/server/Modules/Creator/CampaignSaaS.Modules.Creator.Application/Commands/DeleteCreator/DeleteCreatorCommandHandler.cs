namespace CampaignSaaS.Modules.Creator.Application.Commands.DeleteCreator;

using CampaignSaaS.Modules.Creator.Application.Abstractions;
using CampaignSaaS.SharedKernel.MultiTenancy;
using ErrorOr;
using MediatR;

public class DeleteCreatorCommandHandler : IRequestHandler<DeleteCreatorCommand, ErrorOr<Success>>
{
    private readonly ICreatorRepository _creatorRepository;
    private readonly ICreatorUnitOfWork _unitOfWork;
    private readonly ICurrentTenantContext _tenantContext;

    public DeleteCreatorCommandHandler(
        ICreatorRepository creatorRepository,
        ICreatorUnitOfWork unitOfWork,
        ICurrentTenantContext tenantContext)
    {
        _creatorRepository = creatorRepository;
        _unitOfWork = unitOfWork;
        _tenantContext = tenantContext;
    }

    public async Task<ErrorOr<Success>> Handle(DeleteCreatorCommand request, CancellationToken cancellationToken)
    {
        if (!_tenantContext.OrganizationId.HasValue)
        {
            return Error.Forbidden("Tenant.Required", "Active organization context is required.");
        }

        var creator = await _creatorRepository.GetByIdAsync(request.Id, cancellationToken);
        if (creator == null || creator.IsDeleted)
        {
            return Error.NotFound("Creator.NotFound", "Creator not found.");
        }

        creator.Delete();
        _creatorRepository.Update(creator);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }
}

namespace CampaignSaaS.Modules.Campaign.Application.Commands.RemoveCreatorFromRoster;

using CampaignSaaS.Modules.Campaign.Application.Abstractions;
using CampaignSaaS.SharedKernel.MultiTenancy;
using ErrorOr;
using MediatR;

public class RemoveCreatorFromRosterCommandHandler : IRequestHandler<RemoveCreatorFromRosterCommand, ErrorOr<Success>>
{
    private readonly ICampaignCreatorRepository _rosterRepository;
    private readonly ICampaignUnitOfWork _unitOfWork;
    private readonly ICurrentTenantContext _tenantContext;

    public RemoveCreatorFromRosterCommandHandler(
        ICampaignCreatorRepository rosterRepository,
        ICampaignUnitOfWork unitOfWork,
        ICurrentTenantContext tenantContext)
    {
        _rosterRepository = rosterRepository;
        _unitOfWork = unitOfWork;
        _tenantContext = tenantContext;
    }

    public async Task<ErrorOr<Success>> Handle(RemoveCreatorFromRosterCommand request, CancellationToken cancellationToken)
    {
        if (!_tenantContext.OrganizationId.HasValue)
        {
            return Error.Forbidden("Tenant.Required", "Active organization context is required.");
        }

        var rosterItem = await _rosterRepository.GetByCampaignAndCreatorAsync(request.CampaignId, request.CreatorId, cancellationToken);
        if (rosterItem == null)
        {
            return Error.NotFound("CampaignRoster.NotFound", "Creator is not assigned to this campaign roster.");
        }

        _rosterRepository.Remove(rosterItem);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }
}

namespace CampaignSaaS.Modules.Campaign.Application.Commands.UpdateRosterStatus;

using CampaignSaaS.Modules.Campaign.Application.Abstractions;
using CampaignSaaS.Modules.Campaign.Contracts.DTOs;
using CampaignSaaS.Modules.Campaign.Domain.Enums;
using CampaignSaaS.SharedKernel.MultiTenancy;
using ErrorOr;
using MediatR;

public class UpdateRosterStatusCommandHandler : IRequestHandler<UpdateRosterStatusCommand, ErrorOr<CampaignCreatorDto>>
{
    private readonly ICampaignCreatorRepository _rosterRepository;
    private readonly ICampaignUnitOfWork _unitOfWork;
    private readonly ICurrentTenantContext _tenantContext;

    public UpdateRosterStatusCommandHandler(
        ICampaignCreatorRepository rosterRepository,
        ICampaignUnitOfWork unitOfWork,
        ICurrentTenantContext tenantContext)
    {
        _rosterRepository = rosterRepository;
        _unitOfWork = unitOfWork;
        _tenantContext = tenantContext;
    }

    public async Task<ErrorOr<CampaignCreatorDto>> Handle(UpdateRosterStatusCommand request, CancellationToken cancellationToken)
    {
        if (!_tenantContext.OrganizationId.HasValue)
        {
            return Error.Forbidden("Tenant.Required", "Active organization context is required.");
        }

        if (!Enum.TryParse<CampaignCreatorStatus>(request.Status, true, out var status))
        {
            return Error.Validation("CampaignRoster.InvalidStatus", $"Status '{request.Status}' is invalid.");
        }

        var rosterItem = await _rosterRepository.GetByCampaignAndCreatorAsync(request.CampaignId, request.CreatorId, cancellationToken);
        if (rosterItem == null)
        {
            return Error.NotFound("CampaignRoster.NotFound", "Creator is not assigned to this campaign roster.");
        }

        rosterItem.UpdateStatus(status);

        if (request.AgreedRate.HasValue)
        {
            rosterItem.UpdateRate(request.AgreedRate.Value);
        }

        _rosterRepository.Update(rosterItem);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new CampaignCreatorDto(
            rosterItem.Id,
            rosterItem.OrganizationId,
            rosterItem.CampaignId,
            rosterItem.CreatorId,
            rosterItem.Status.ToString(),
            rosterItem.AgreedRate,
            rosterItem.CreatedAt,
            rosterItem.UpdatedAt);
    }
}

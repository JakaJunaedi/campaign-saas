namespace CampaignSaaS.Modules.Campaign.Application.Commands.AddCreatorToRoster;

using CampaignSaaS.Modules.Campaign.Application.Abstractions;
using CampaignSaaS.Modules.Campaign.Contracts.DTOs;
using CampaignSaaS.Modules.Campaign.Domain.Entities;
using CampaignSaaS.Modules.Campaign.Domain.Enums;
using CampaignSaaS.SharedKernel.MultiTenancy;
using ErrorOr;
using MediatR;

public class AddCreatorToRosterCommandHandler : IRequestHandler<AddCreatorToRosterCommand, ErrorOr<CampaignCreatorDto>>
{
    private readonly ICampaignRepository _campaignRepository;
    private readonly ICampaignCreatorRepository _rosterRepository;
    private readonly ICampaignUnitOfWork _unitOfWork;
    private readonly ICurrentTenantContext _tenantContext;

    public AddCreatorToRosterCommandHandler(
        ICampaignRepository campaignRepository,
        ICampaignCreatorRepository rosterRepository,
        ICampaignUnitOfWork unitOfWork,
        ICurrentTenantContext tenantContext)
    {
        _campaignRepository = campaignRepository;
        _rosterRepository = rosterRepository;
        _unitOfWork = unitOfWork;
        _tenantContext = tenantContext;
    }

    public async Task<ErrorOr<CampaignCreatorDto>> Handle(AddCreatorToRosterCommand request, CancellationToken cancellationToken)
    {
        if (!_tenantContext.OrganizationId.HasValue)
        {
            return Error.Forbidden("Tenant.Required", "Active organization context is required.");
        }

        var orgId = _tenantContext.OrganizationId.Value;

        var campaign = await _campaignRepository.GetByIdAsync(request.CampaignId, cancellationToken);
        if (campaign == null || campaign.IsDeleted)
        {
            return Error.NotFound("Campaign.NotFound", "Campaign not found.");
        }

        if (await _rosterRepository.ExistsInCampaignAsync(request.CampaignId, request.CreatorId, cancellationToken))
        {
            return Error.Conflict("CampaignRoster.DuplicateCreator", "Creator is already assigned to this campaign roster.");
        }

        var rosterItem = CampaignCreator.Create(
            orgId,
            request.CampaignId,
            request.CreatorId,
            CampaignCreatorStatus.Shortlisted,
            request.AgreedRate);

        await _rosterRepository.AddAsync(rosterItem, cancellationToken);
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

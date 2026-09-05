namespace CampaignSaaS.Modules.Campaign.Application.Commands.UpdateCampaignStatus;

using CampaignSaaS.Modules.Campaign.Application.Abstractions;
using CampaignSaaS.Modules.Campaign.Contracts.DTOs;
using CampaignSaaS.Modules.Campaign.Domain.Enums;
using CampaignSaaS.SharedKernel.MultiTenancy;
using ErrorOr;
using MediatR;

public class UpdateCampaignStatusCommandHandler : IRequestHandler<UpdateCampaignStatusCommand, ErrorOr<CampaignDto>>
{
    private readonly ICampaignRepository _campaignRepository;
    private readonly ICampaignUnitOfWork _unitOfWork;
    private readonly ICurrentTenantContext _tenantContext;

    public UpdateCampaignStatusCommandHandler(
        ICampaignRepository campaignRepository,
        ICampaignUnitOfWork unitOfWork,
        ICurrentTenantContext tenantContext)
    {
        _campaignRepository = campaignRepository;
        _unitOfWork = unitOfWork;
        _tenantContext = tenantContext;
    }

    public async Task<ErrorOr<CampaignDto>> Handle(UpdateCampaignStatusCommand request, CancellationToken cancellationToken)
    {
        if (!_tenantContext.OrganizationId.HasValue)
        {
            return Error.Forbidden("Tenant.Required", "Active organization context is required.");
        }

        if (!Enum.TryParse<CampaignStatus>(request.Status, true, out var status))
        {
            return Error.Validation("Campaign.InvalidStatus", $"Status '{request.Status}' is invalid.");
        }

        var campaign = await _campaignRepository.GetByIdAsync(request.Id, cancellationToken);
        if (campaign == null || campaign.IsDeleted)
        {
            return Error.NotFound("Campaign.NotFound", "Campaign not found.");
        }

        campaign.SetStatus(status);
        _campaignRepository.Update(campaign);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new CampaignDto(
            campaign.Id,
            campaign.OrganizationId,
            campaign.ClientId,
            campaign.Title,
            campaign.Description,
            campaign.Budget,
            campaign.StartDate,
            campaign.EndDate,
            campaign.Status.ToString(),
            campaign.CreatedAt,
            campaign.UpdatedAt);
    }
}

namespace CampaignSaaS.Modules.Campaign.Application.Commands.UpdateCampaign;

using CampaignSaaS.Modules.Campaign.Application.Abstractions;
using CampaignSaaS.Modules.Campaign.Contracts.DTOs;
using CampaignSaaS.SharedKernel.MultiTenancy;
using ErrorOr;
using MediatR;

public class UpdateCampaignCommandHandler : IRequestHandler<UpdateCampaignCommand, ErrorOr<CampaignDto>>
{
    private readonly ICampaignRepository _campaignRepository;
    private readonly ICampaignUnitOfWork _unitOfWork;
    private readonly ICurrentTenantContext _tenantContext;

    public UpdateCampaignCommandHandler(
        ICampaignRepository campaignRepository,
        ICampaignUnitOfWork unitOfWork,
        ICurrentTenantContext tenantContext)
    {
        _campaignRepository = campaignRepository;
        _unitOfWork = unitOfWork;
        _tenantContext = tenantContext;
    }

    public async Task<ErrorOr<CampaignDto>> Handle(UpdateCampaignCommand request, CancellationToken cancellationToken)
    {
        if (!_tenantContext.OrganizationId.HasValue)
        {
            return Error.Forbidden("Tenant.Required", "Active organization context is required.");
        }

        var campaign = await _campaignRepository.GetByIdAsync(request.Id, cancellationToken);
        if (campaign == null || campaign.IsDeleted)
        {
            return Error.NotFound("Campaign.NotFound", "Campaign not found.");
        }

        campaign.UpdateDetails(request.Title, request.Description, request.Budget, request.StartDate, request.EndDate);

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

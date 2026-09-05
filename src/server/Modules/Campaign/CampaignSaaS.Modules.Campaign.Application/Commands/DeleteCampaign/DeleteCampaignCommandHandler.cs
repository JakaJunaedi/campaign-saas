namespace CampaignSaaS.Modules.Campaign.Application.Commands.DeleteCampaign;

using CampaignSaaS.Modules.Campaign.Application.Abstractions;
using CampaignSaaS.SharedKernel.MultiTenancy;
using ErrorOr;
using MediatR;

public class DeleteCampaignCommandHandler : IRequestHandler<DeleteCampaignCommand, ErrorOr<Success>>
{
    private readonly ICampaignRepository _campaignRepository;
    private readonly ICampaignUnitOfWork _unitOfWork;
    private readonly ICurrentTenantContext _tenantContext;

    public DeleteCampaignCommandHandler(
        ICampaignRepository campaignRepository,
        ICampaignUnitOfWork unitOfWork,
        ICurrentTenantContext tenantContext)
    {
        _campaignRepository = campaignRepository;
        _unitOfWork = unitOfWork;
        _tenantContext = tenantContext;
    }

    public async Task<ErrorOr<Success>> Handle(DeleteCampaignCommand request, CancellationToken cancellationToken)
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

        campaign.Delete();
        _campaignRepository.Update(campaign);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }
}

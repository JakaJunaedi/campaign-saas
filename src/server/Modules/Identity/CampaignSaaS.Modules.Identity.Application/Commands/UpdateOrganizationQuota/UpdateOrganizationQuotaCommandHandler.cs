namespace CampaignSaaS.Modules.Identity.Application.Commands.UpdateOrganizationQuota;

using CampaignSaaS.Modules.Identity.Application.Abstractions;
using CampaignSaaS.SharedKernel.Application;
using ErrorOr;
using MediatR;

public class UpdateOrganizationQuotaCommandHandler : IRequestHandler<UpdateOrganizationQuotaCommand, ErrorOr<Success>>
{
    private readonly IOrganizationRepository _organizationRepository;
    private readonly IIdentityUnitOfWork _unitOfWork;

    public UpdateOrganizationQuotaCommandHandler(
        IOrganizationRepository organizationRepository,
        IIdentityUnitOfWork unitOfWork)
    {
        _organizationRepository = organizationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<Success>> Handle(UpdateOrganizationQuotaCommand request, CancellationToken cancellationToken)
    {
        var organization = await _organizationRepository.GetByIdAsync(request.OrganizationId, cancellationToken);
        if (organization == null)
        {
            return Error.NotFound("Organization.NotFound", $"Organization with ID '{request.OrganizationId}' was not found.");
        }

        organization.SetQuota(request.StorageQuotaBytes, request.MaxActiveCampaigns);

        _organizationRepository.Update(organization);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }
}

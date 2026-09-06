namespace CampaignSaaS.Modules.Identity.Application.Commands.UpdateOrganizationStatus;

using CampaignSaaS.Modules.Identity.Application.Abstractions;
using CampaignSaaS.Modules.Identity.Contracts.DTOs;
using CampaignSaaS.Modules.Identity.Domain.Enums;
using ErrorOr;
using MediatR;

public class UpdateOrganizationStatusCommandHandler : IRequestHandler<UpdateOrganizationStatusCommand, ErrorOr<AdminOrganizationItemDto>>
{
    private readonly IOrganizationRepository _organizationRepository;
    private readonly IUserRepository _userRepository;
    private readonly IIdentityUnitOfWork _unitOfWork;

    public UpdateOrganizationStatusCommandHandler(
        IOrganizationRepository organizationRepository,
        IUserRepository userRepository,
        IIdentityUnitOfWork unitOfWork)
    {
        _organizationRepository = organizationRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<AdminOrganizationItemDto>> Handle(UpdateOrganizationStatusCommand request, CancellationToken cancellationToken)
    {
        var org = await _organizationRepository.GetByIdAsync(request.OrganizationId, cancellationToken);
        if (org == null)
        {
            return Error.NotFound("Organization.NotFound", $"Organization with ID '{request.OrganizationId}' not found.");
        }

        if (!Enum.TryParse<OrganizationStatus>(request.Status, true, out var newStatus))
        {
            return Error.Validation("Organization.InvalidStatus", $"Status '{request.Status}' is invalid. Valid values: Active, Suspended, Trial.");
        }

        org.SetStatus(newStatus);
        _organizationRepository.Update(org);
        await _unitOfWork.SaveChangesAsync(cancellationToken);


        var userCount = await _userRepository.GetUsersCountByOrganizationIdAsync(org.Id, cancellationToken);
        return new AdminOrganizationItemDto(
            org.Id,
            org.Name,
            org.Slug,
            org.Status.ToString(),
            userCount,
            org.CreatedAt);
    }
}

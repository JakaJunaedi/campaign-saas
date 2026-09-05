namespace CampaignSaaS.Modules.Creator.Application.Commands.UpdateCreatorStatus;

using CampaignSaaS.Modules.Creator.Application.Abstractions;
using CampaignSaaS.Modules.Creator.Contracts.DTOs;
using CampaignSaaS.Modules.Creator.Domain.Enums;
using CampaignSaaS.SharedKernel.MultiTenancy;
using ErrorOr;
using MediatR;

public class UpdateCreatorStatusCommandHandler : IRequestHandler<UpdateCreatorStatusCommand, ErrorOr<CreatorDto>>
{
    private readonly ICreatorRepository _creatorRepository;
    private readonly ICreatorUnitOfWork _unitOfWork;
    private readonly ICurrentTenantContext _tenantContext;

    public UpdateCreatorStatusCommandHandler(
        ICreatorRepository creatorRepository,
        ICreatorUnitOfWork unitOfWork,
        ICurrentTenantContext tenantContext)
    {
        _creatorRepository = creatorRepository;
        _unitOfWork = unitOfWork;
        _tenantContext = tenantContext;
    }

    public async Task<ErrorOr<CreatorDto>> Handle(UpdateCreatorStatusCommand request, CancellationToken cancellationToken)
    {
        if (!_tenantContext.OrganizationId.HasValue)
        {
            return Error.Forbidden("Tenant.Required", "Active organization context is required.");
        }

        if (!Enum.TryParse<CreatorStatus>(request.Status, true, out var status))
        {
            return Error.Validation("Creator.InvalidStatus", $"Status '{request.Status}' is invalid.");
        }

        var creator = await _creatorRepository.GetByIdAsync(request.Id, cancellationToken);
        if (creator == null || creator.IsDeleted)
        {
            return Error.NotFound("Creator.NotFound", "Creator not found.");
        }

        creator.SetStatus(status);
        _creatorRepository.Update(creator);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var socialDtos = creator.SocialAccounts.Select(s => new SocialAccountDto(
            s.Platform.ToString(),
            s.Handle,
            s.ProfileUrl,
            s.FollowerCount)).ToList();

        return new CreatorDto(
            creator.Id,
            creator.OrganizationId,
            creator.FullName,
            creator.Email,
            creator.PhoneNumber,
            creator.Niche,
            creator.Status.ToString(),
            socialDtos,
            creator.CreatedAt,
            creator.UpdatedAt);
    }
}

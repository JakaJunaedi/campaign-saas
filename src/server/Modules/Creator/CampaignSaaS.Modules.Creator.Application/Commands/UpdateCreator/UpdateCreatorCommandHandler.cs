namespace CampaignSaaS.Modules.Creator.Application.Commands.UpdateCreator;

using CampaignSaaS.Modules.Creator.Application.Abstractions;
using CampaignSaaS.Modules.Creator.Contracts.DTOs;
using CampaignSaaS.Modules.Creator.Domain.Enums;
using CampaignSaaS.Modules.Creator.Domain.ValueObjects;
using CampaignSaaS.SharedKernel.MultiTenancy;
using ErrorOr;
using MediatR;

public class UpdateCreatorCommandHandler : IRequestHandler<UpdateCreatorCommand, ErrorOr<CreatorDto>>
{
    private readonly ICreatorRepository _creatorRepository;
    private readonly ICreatorUnitOfWork _unitOfWork;
    private readonly ICurrentTenantContext _tenantContext;

    public UpdateCreatorCommandHandler(
        ICreatorRepository creatorRepository,
        ICreatorUnitOfWork unitOfWork,
        ICurrentTenantContext tenantContext)
    {
        _creatorRepository = creatorRepository;
        _unitOfWork = unitOfWork;
        _tenantContext = tenantContext;
    }

    public async Task<ErrorOr<CreatorDto>> Handle(UpdateCreatorCommand request, CancellationToken cancellationToken)
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

        creator.UpdateDetails(request.FullName, request.Niche, request.Email, request.PhoneNumber);

        if (request.SocialAccounts != null)
        {
            var socialAccounts = new List<SocialAccount>();
            foreach (var acc in request.SocialAccounts)
            {
                if (Enum.TryParse<PlatformType>(acc.Platform, true, out var platform))
                {
                    socialAccounts.Add(new SocialAccount(platform, acc.Handle, acc.ProfileUrl, acc.FollowerCount));
                }
            }

            creator.SetSocialAccounts(socialAccounts);
        }

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

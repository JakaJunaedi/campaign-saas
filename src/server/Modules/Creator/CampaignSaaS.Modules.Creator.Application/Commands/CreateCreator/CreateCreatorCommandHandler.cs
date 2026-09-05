namespace CampaignSaaS.Modules.Creator.Application.Commands.CreateCreator;

using CampaignSaaS.Modules.Creator.Application.Abstractions;
using CampaignSaaS.Modules.Creator.Contracts.DTOs;
using CampaignSaaS.Modules.Creator.Domain.Entities;
using CampaignSaaS.Modules.Creator.Domain.Enums;
using CampaignSaaS.Modules.Creator.Domain.ValueObjects;
using CampaignSaaS.SharedKernel.MultiTenancy;
using ErrorOr;
using MediatR;

public class CreateCreatorCommandHandler : IRequestHandler<CreateCreatorCommand, ErrorOr<CreatorDto>>
{
    private readonly ICreatorRepository _creatorRepository;
    private readonly ICreatorUnitOfWork _unitOfWork;
    private readonly ICurrentTenantContext _tenantContext;

    public CreateCreatorCommandHandler(
        ICreatorRepository creatorRepository,
        ICreatorUnitOfWork unitOfWork,
        ICurrentTenantContext tenantContext)
    {
        _creatorRepository = creatorRepository;
        _unitOfWork = unitOfWork;
        _tenantContext = tenantContext;
    }

    public async Task<ErrorOr<CreatorDto>> Handle(CreateCreatorCommand request, CancellationToken cancellationToken)
    {
        if (!_tenantContext.OrganizationId.HasValue)
        {
            return Error.Forbidden("Tenant.Required", "Active organization context is required.");
        }

        var orgId = _tenantContext.OrganizationId.Value;

        var socialAccounts = new List<SocialAccount>();
        if (request.SocialAccounts != null)
        {
            foreach (var acc in request.SocialAccounts)
            {
                if (Enum.TryParse<PlatformType>(acc.Platform, true, out var platform))
                {
                    socialAccounts.Add(new SocialAccount(platform, acc.Handle, acc.ProfileUrl, acc.FollowerCount));
                }
            }
        }

        var creator = Creator.Create(
            orgId,
            request.FullName,
            request.Niche,
            request.Email,
            request.PhoneNumber,
            socialAccounts);

        await _creatorRepository.AddAsync(creator, cancellationToken);
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

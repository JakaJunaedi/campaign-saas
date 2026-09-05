namespace CampaignSaaS.Modules.Creator.Application.Queries.GetCreatorById;

using CampaignSaaS.Modules.Creator.Application.Abstractions;
using CampaignSaaS.Modules.Creator.Contracts.DTOs;
using CampaignSaaS.SharedKernel.MultiTenancy;
using ErrorOr;
using MediatR;

public class GetCreatorByIdQueryHandler : IRequestHandler<GetCreatorByIdQuery, ErrorOr<CreatorDto>>
{
    private readonly ICreatorRepository _creatorRepository;
    private readonly ICurrentTenantContext _tenantContext;

    public GetCreatorByIdQueryHandler(
        ICreatorRepository creatorRepository,
        ICurrentTenantContext tenantContext)
    {
        _creatorRepository = creatorRepository;
        _tenantContext = tenantContext;
    }

    public async Task<ErrorOr<CreatorDto>> Handle(GetCreatorByIdQuery request, CancellationToken cancellationToken)
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

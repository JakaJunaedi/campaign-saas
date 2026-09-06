namespace CampaignSaaS.Modules.Creator.Application.Services;

using CampaignSaaS.Modules.Creator.Application.Abstractions;
using CampaignSaaS.Modules.Creator.Contracts;

public class CreatorSummaryQueryService : ICreatorSummaryQueryService
{
    private readonly ICreatorRepository _creatorRepository;

    public CreatorSummaryQueryService(ICreatorRepository creatorRepository)
    {
        _creatorRepository = creatorRepository;
    }

    public async Task<int> GetCreatorsCountAsync(Guid organizationId, CancellationToken cancellationToken = default)
    {
        var (_, totalCount) = await _creatorRepository.GetPagedAsync(organizationId, null, null, null, null, 1, 1, cancellationToken);
        return totalCount;
    }
}

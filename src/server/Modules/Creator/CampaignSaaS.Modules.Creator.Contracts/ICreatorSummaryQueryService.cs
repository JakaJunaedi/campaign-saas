namespace CampaignSaaS.Modules.Creator.Contracts;

public interface ICreatorSummaryQueryService
{
    Task<int> GetCreatorsCountAsync(Guid organizationId, CancellationToken cancellationToken = default);
}

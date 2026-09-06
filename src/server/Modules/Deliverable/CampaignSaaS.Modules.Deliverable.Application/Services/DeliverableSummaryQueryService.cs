namespace CampaignSaaS.Modules.Deliverable.Application.Services;

using CampaignSaaS.Modules.Deliverable.Application.Abstractions;
using CampaignSaaS.Modules.Deliverable.Contracts;
using CampaignSaaS.Modules.Deliverable.Contracts.DTOs;

public class DeliverableSummaryQueryService : IDeliverableSummaryQueryService
{
    private readonly IDeliverableRepository _deliverableRepository;
    private readonly IContentSubmissionRepository _submissionRepository;

    public DeliverableSummaryQueryService(
        IDeliverableRepository deliverableRepository,
        IContentSubmissionRepository submissionRepository)
    {
        _deliverableRepository = deliverableRepository;
        _submissionRepository = submissionRepository;
    }

    public async Task<IReadOnlyList<DeliverableDto>> GetDeliverablesByCampaignIdAsync(Guid organizationId, Guid campaignId, CancellationToken cancellationToken = default)
    {
        var deliverables = await _deliverableRepository.GetByCampaignIdAsync(campaignId, cancellationToken);
        var filtered = deliverables.Where(d => d.OrganizationId == organizationId).ToList();

        var result = new List<DeliverableDto>(filtered.Count);
        foreach (var d in filtered)
        {
            var latestVersion = await _submissionRepository.GetLatestVersionNumberAsync(d.Id, cancellationToken);
            result.Add(new DeliverableDto(
                d.Id,
                d.OrganizationId,
                d.CampaignId,
                d.CampaignCreatorId,
                d.Title,
                d.Platform.ToString(),
                d.ContentType.ToString(),
                d.BriefNotes,
                d.DueDate,
                d.PostingDate,
                d.Status.ToString(),
                d.LiveUrl,
                d.ProofMediaKey,
                latestVersion,
                d.CreatedAt,
                d.UpdatedAt));
        }

        return result;
    }

    public async Task<DeliverablesOverviewStatsDto> GetDeliverablesOverviewStatsAsync(Guid organizationId, CancellationToken cancellationToken = default)
    {
        var deliverables = await _deliverableRepository.GetByOrganizationIdAsync(organizationId, cancellationToken);
        var pendingReviews = deliverables.Count(d => d.Status == Domain.Enums.DeliverableStatus.Submitted || d.Status == Domain.Enums.DeliverableStatus.Revision);
        var completedCount = deliverables.Count(d => d.Status == Domain.Enums.DeliverableStatus.Approved || d.Status == Domain.Enums.DeliverableStatus.Published);

        var actionItems = deliverables
            .Where(d => d.Status == Domain.Enums.DeliverableStatus.Submitted || d.Status == Domain.Enums.DeliverableStatus.Revision || d.Status == Domain.Enums.DeliverableStatus.Pending)
            .OrderBy(d => d.DueDate)
            .Take(5)
            .Select(d => new DashboardDeliverableActionItemDto(
                d.Id,
                d.CampaignId,
                d.Title,
                d.Platform.ToString(),
                d.ContentType.ToString(),
                d.Status.ToString(),
                d.DueDate))
            .ToList();

        return new DeliverablesOverviewStatsDto(
            pendingReviews,
            completedCount,
            actionItems);
    }
}


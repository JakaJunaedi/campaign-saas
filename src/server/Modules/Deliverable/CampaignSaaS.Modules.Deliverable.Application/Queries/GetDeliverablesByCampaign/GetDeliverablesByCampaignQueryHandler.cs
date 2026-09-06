namespace CampaignSaaS.Modules.Deliverable.Application.Queries.GetDeliverablesByCampaign;

using CampaignSaaS.Modules.Deliverable.Application.Abstractions;
using CampaignSaaS.Modules.Deliverable.Contracts.DTOs;
using CampaignSaaS.SharedKernel.MultiTenancy;
using ErrorOr;
using MediatR;

public class GetDeliverablesByCampaignQueryHandler : IRequestHandler<GetDeliverablesByCampaignQuery, ErrorOr<IReadOnlyList<DeliverableDto>>>
{
    private readonly IDeliverableRepository _deliverableRepository;
    private readonly IContentSubmissionRepository _submissionRepository;
    private readonly ICurrentTenantContext _tenantContext;

    public GetDeliverablesByCampaignQueryHandler(
        IDeliverableRepository deliverableRepository,
        IContentSubmissionRepository submissionRepository,
        ICurrentTenantContext tenantContext)
    {
        _deliverableRepository = deliverableRepository;
        _submissionRepository = submissionRepository;
        _tenantContext = tenantContext;
    }

    public async Task<ErrorOr<IReadOnlyList<DeliverableDto>>> Handle(GetDeliverablesByCampaignQuery request, CancellationToken cancellationToken)
    {
        if (!_tenantContext.OrganizationId.HasValue)
        {
            return Error.Forbidden("Tenant.Required", "Active organization context is required.");
        }

        var deliverables = await _deliverableRepository.GetByCampaignIdAsync(request.CampaignId, cancellationToken);
        var dtos = new List<DeliverableDto>();

        foreach (var d in deliverables)
        {
            var latestVersion = await _submissionRepository.GetLatestVersionNumberAsync(d.Id, cancellationToken);
            dtos.Add(new DeliverableDto(
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

        return dtos;
    }
}


namespace CampaignSaaS.Modules.Deliverable.Application.Commands.SubmitPublishProof;

using CampaignSaaS.Modules.Deliverable.Application.Abstractions;
using CampaignSaaS.Modules.Deliverable.Contracts.DTOs;
using CampaignSaaS.SharedKernel.MultiTenancy;
using ErrorOr;
using MediatR;

public class SubmitPublishProofCommandHandler : IRequestHandler<SubmitPublishProofCommand, ErrorOr<DeliverableDto>>
{
    private readonly IDeliverableRepository _deliverableRepository;
    private readonly IContentSubmissionRepository _submissionRepository;
    private readonly IDeliverableUnitOfWork _unitOfWork;
    private readonly ICurrentTenantContext _tenantContext;

    public SubmitPublishProofCommandHandler(
        IDeliverableRepository deliverableRepository,
        IContentSubmissionRepository submissionRepository,
        IDeliverableUnitOfWork unitOfWork,
        ICurrentTenantContext tenantContext)
    {
        _deliverableRepository = deliverableRepository;
        _submissionRepository = submissionRepository;
        _unitOfWork = unitOfWork;
        _tenantContext = tenantContext;
    }

    public async Task<ErrorOr<DeliverableDto>> Handle(SubmitPublishProofCommand request, CancellationToken cancellationToken)
    {
        if (!_tenantContext.OrganizationId.HasValue)
        {
            return Error.Forbidden("Tenant.Required", "Active organization context is required.");
        }

        var deliverable = await _deliverableRepository.GetByIdAsync(request.DeliverableId, cancellationToken);
        if (deliverable == null)
        {
            return Error.NotFound("Deliverable.NotFound", "Deliverable not found.");
        }

        deliverable.MarkAsPublished(request.LiveUrl, request.ProofMediaKey, request.PostingDate);

        _deliverableRepository.Update(deliverable);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var latestVersion = await _submissionRepository.GetLatestVersionNumberAsync(deliverable.Id, cancellationToken);

        return new DeliverableDto(
            deliverable.Id,
            deliverable.OrganizationId,
            deliverable.CampaignId,
            deliverable.CampaignCreatorId,
            deliverable.Title,
            deliverable.Platform.ToString(),
            deliverable.ContentType.ToString(),
            deliverable.BriefNotes,
            deliverable.DueDate,
            deliverable.PostingDate,
            deliverable.Status.ToString(),
            deliverable.LiveUrl,
            deliverable.ProofMediaKey,
            latestVersion,
            deliverable.CreatedAt,
            deliverable.UpdatedAt);
    }
}


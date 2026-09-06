namespace CampaignSaaS.Modules.Deliverable.Application.Commands.UpdateDeliverable;

using CampaignSaaS.Modules.Deliverable.Application.Abstractions;
using CampaignSaaS.Modules.Deliverable.Contracts.DTOs;
using CampaignSaaS.Modules.Deliverable.Domain.Enums;
using CampaignSaaS.SharedKernel.MultiTenancy;
using ErrorOr;
using MediatR;

public class UpdateDeliverableCommandHandler : IRequestHandler<UpdateDeliverableCommand, ErrorOr<DeliverableDto>>
{
    private readonly IDeliverableRepository _deliverableRepository;
    private readonly IContentSubmissionRepository _submissionRepository;
    private readonly IDeliverableUnitOfWork _unitOfWork;
    private readonly ICurrentTenantContext _tenantContext;

    public UpdateDeliverableCommandHandler(
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

    public async Task<ErrorOr<DeliverableDto>> Handle(UpdateDeliverableCommand request, CancellationToken cancellationToken)
    {
        if (!_tenantContext.OrganizationId.HasValue)
        {
            return Error.Forbidden("Tenant.Required", "Active organization context is required.");
        }

        if (!Enum.TryParse<PlatformType>(request.Platform, true, out var platform))
        {
            return Error.Validation("Deliverable.InvalidPlatform", $"Platform '{request.Platform}' is invalid.");
        }

        if (!Enum.TryParse<ContentType>(request.ContentType, true, out var contentType))
        {
            return Error.Validation("Deliverable.InvalidContentType", $"ContentType '{request.ContentType}' is invalid.");
        }

        var deliverable = await _deliverableRepository.GetByIdAsync(request.Id, cancellationToken);
        if (deliverable == null)
        {
            return Error.NotFound("Deliverable.NotFound", "Deliverable not found.");
        }

        deliverable.UpdateDetails(
            request.Title,
            platform,
            contentType,
            request.BriefNotes,
            request.DueDate);

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


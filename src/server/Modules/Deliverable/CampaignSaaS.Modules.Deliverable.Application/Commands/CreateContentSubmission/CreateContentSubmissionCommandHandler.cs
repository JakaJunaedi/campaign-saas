namespace CampaignSaaS.Modules.Deliverable.Application.Commands.CreateContentSubmission;

using CampaignSaaS.Modules.Deliverable.Application.Abstractions;
using CampaignSaaS.Modules.Deliverable.Contracts.DTOs;
using CampaignSaaS.Modules.Deliverable.Domain.Entities;
using CampaignSaaS.SharedKernel.MultiTenancy;
using ErrorOr;
using MediatR;

public class CreateContentSubmissionCommandHandler : IRequestHandler<CreateContentSubmissionCommand, ErrorOr<ContentSubmissionDto>>
{
    private readonly IDeliverableRepository _deliverableRepository;
    private readonly IContentSubmissionRepository _submissionRepository;
    private readonly IStorageService _storageService;
    private readonly IDeliverableUnitOfWork _unitOfWork;
    private readonly ICurrentTenantContext _tenantContext;

    public CreateContentSubmissionCommandHandler(
        IDeliverableRepository deliverableRepository,
        IContentSubmissionRepository submissionRepository,
        IStorageService storageService,
        IDeliverableUnitOfWork unitOfWork,
        ICurrentTenantContext tenantContext)
    {
        _deliverableRepository = deliverableRepository;
        _submissionRepository = submissionRepository;
        _storageService = storageService;
        _unitOfWork = unitOfWork;
        _tenantContext = tenantContext;
    }

    public async Task<ErrorOr<ContentSubmissionDto>> Handle(CreateContentSubmissionCommand request, CancellationToken cancellationToken)
    {
        if (!_tenantContext.OrganizationId.HasValue)
        {
            return Error.Forbidden("Tenant.Required", "Active organization context is required.");
        }

        var orgId = _tenantContext.OrganizationId.Value;

        var deliverable = await _deliverableRepository.GetByIdAsync(request.DeliverableId, cancellationToken);
        if (deliverable == null)
        {
            return Error.NotFound("Deliverable.NotFound", "Deliverable not found.");
        }

        var currentLatestVersion = await _submissionRepository.GetLatestVersionNumberAsync(deliverable.Id, cancellationToken);
        var newVersion = currentLatestVersion + 1;

        var submission = ContentSubmission.Create(
            orgId,
            deliverable.Id,
            newVersion,
            request.MediaObjectKey,
            request.MediaFileName,
            request.MediaFileSize,
            request.Caption);

        deliverable.MarkAsSubmitted();

        await _submissionRepository.AddAsync(submission, cancellationToken);
        _deliverableRepository.Update(deliverable);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        string? downloadUrl = null;
        try
        {
            downloadUrl = await _storageService.GeneratePresignedDownloadUrlAsync(submission.MediaObjectKey, 60, cancellationToken);
        }
        catch
        {
            // Non-blocking fallback if storage endpoint is in local offline test mode
            downloadUrl = $"/api/v1/files/download?key={Uri.EscapeDataString(submission.MediaObjectKey)}";
        }

        return new ContentSubmissionDto(
            submission.Id,
            submission.OrganizationId,
            submission.DeliverableId,
            submission.VersionNumber,
            submission.MediaObjectKey,
            submission.MediaFileName,
            submission.MediaFileSize,
            submission.Caption,
            downloadUrl,
            submission.SubmittedAt);
    }
}


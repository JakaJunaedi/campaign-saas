namespace CampaignSaaS.Modules.Deliverable.Application.Queries.GetContentSubmissions;

using CampaignSaaS.Modules.Deliverable.Application.Abstractions;
using CampaignSaaS.Modules.Deliverable.Contracts.DTOs;
using CampaignSaaS.SharedKernel.MultiTenancy;
using ErrorOr;
using MediatR;

public class GetContentSubmissionsQueryHandler : IRequestHandler<GetContentSubmissionsQuery, ErrorOr<IReadOnlyList<ContentSubmissionDto>>>
{
    private readonly IContentSubmissionRepository _submissionRepository;
    private readonly IStorageService _storageService;
    private readonly ICurrentTenantContext _tenantContext;

    public GetContentSubmissionsQueryHandler(
        IContentSubmissionRepository submissionRepository,
        IStorageService storageService,
        ICurrentTenantContext tenantContext)
    {
        _submissionRepository = submissionRepository;
        _storageService = storageService;
        _tenantContext = tenantContext;
    }

    public async Task<ErrorOr<IReadOnlyList<ContentSubmissionDto>>> Handle(GetContentSubmissionsQuery request, CancellationToken cancellationToken)
    {
        if (!_tenantContext.OrganizationId.HasValue)
        {
            return Error.Forbidden("Tenant.Required", "Active organization context is required.");
        }

        var submissions = await _submissionRepository.GetByDeliverableIdAsync(request.DeliverableId, cancellationToken);
        var dtos = new List<ContentSubmissionDto>();

        foreach (var s in submissions)
        {
            string? downloadUrl = null;
            try
            {
                downloadUrl = await _storageService.GeneratePresignedDownloadUrlAsync(s.MediaObjectKey, 60, cancellationToken);
            }
            catch
            {
                downloadUrl = $"/api/v1/files/download?key={Uri.EscapeDataString(s.MediaObjectKey)}";
            }

            dtos.Add(new ContentSubmissionDto(
                s.Id,
                s.OrganizationId,
                s.DeliverableId,
                s.VersionNumber,
                s.MediaObjectKey,
                s.MediaFileName,
                s.MediaFileSize,
                s.Caption,
                downloadUrl,
                s.SubmittedAt));
        }

        return dtos;
    }
}


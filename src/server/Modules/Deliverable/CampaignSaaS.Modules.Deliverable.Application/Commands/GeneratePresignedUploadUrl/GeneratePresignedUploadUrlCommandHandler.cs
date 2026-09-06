namespace CampaignSaaS.Modules.Deliverable.Application.Commands.GeneratePresignedUploadUrl;

using CampaignSaaS.Modules.Deliverable.Application.Abstractions;
using CampaignSaaS.Modules.Deliverable.Contracts.DTOs;
using CampaignSaaS.SharedKernel.MultiTenancy;
using ErrorOr;
using MediatR;

public class GeneratePresignedUploadUrlCommandHandler : IRequestHandler<GeneratePresignedUploadUrlCommand, ErrorOr<PresignedUploadUrlDto>>
{
    private readonly IStorageService _storageService;
    private readonly ICurrentTenantContext _tenantContext;

    public GeneratePresignedUploadUrlCommandHandler(
        IStorageService storageService,
        ICurrentTenantContext tenantContext)
    {
        _storageService = storageService;
        _tenantContext = tenantContext;
    }

    public async Task<ErrorOr<PresignedUploadUrlDto>> Handle(GeneratePresignedUploadUrlCommand request, CancellationToken cancellationToken)
    {
        if (!_tenantContext.OrganizationId.HasValue)
        {
            return Error.Forbidden("Tenant.Required", "Active organization context is required.");
        }

        var orgId = _tenantContext.OrganizationId.Value;

        var result = await _storageService.GeneratePresignedUploadUrlAsync(
            orgId,
            request.FileName,
            request.ContentType,
            request.FileSize,
            request.CampaignId,
            request.DeliverableId,
            null,
            cancellationToken);

        return result;
    }
}


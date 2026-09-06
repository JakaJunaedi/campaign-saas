namespace CampaignSaaS.Modules.Reporting.Application.Queries.GetReportDownloadUrl;

using CampaignSaaS.Modules.Reporting.Application.Abstractions;
using CampaignSaaS.SharedKernel.MultiTenancy;
using ErrorOr;
using MediatR;

public class GetReportDownloadUrlQueryHandler : IRequestHandler<GetReportDownloadUrlQuery, ErrorOr<string>>
{
    private readonly ICampaignReportRepository _reportRepository;
    private readonly IReportStorageService _storageService;
    private readonly ICurrentTenantContext _tenantContext;

    public GetReportDownloadUrlQueryHandler(
        ICampaignReportRepository reportRepository,
        IReportStorageService storageService,
        ICurrentTenantContext tenantContext)
    {
        _reportRepository = reportRepository;
        _storageService = storageService;
        _tenantContext = tenantContext;
    }

    public async Task<ErrorOr<string>> Handle(GetReportDownloadUrlQuery request, CancellationToken cancellationToken)
    {
        if (!_tenantContext.OrganizationId.HasValue)
        {
            return Error.Forbidden("Tenant.Required", "Active organization context is required.");
        }

        var report = await _reportRepository.GetByIdAsync(request.Id, cancellationToken);
        if (report == null)
        {
            return Error.NotFound("CampaignReport.NotFound", "Campaign report not found.");
        }

        if (string.IsNullOrEmpty(report.FileObjectKey))
        {
            return Error.Validation("CampaignReport.NotReady", "Report has not completed generation yet.");
        }

        var downloadUrl = await _storageService.GeneratePresignedDownloadUrlAsync(report.FileObjectKey, 60, cancellationToken);
        return downloadUrl;
    }
}

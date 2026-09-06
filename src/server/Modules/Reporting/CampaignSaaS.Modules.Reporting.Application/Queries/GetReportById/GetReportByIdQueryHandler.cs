namespace CampaignSaaS.Modules.Reporting.Application.Queries.GetReportById;

using CampaignSaaS.Modules.Reporting.Application.Abstractions;
using CampaignSaaS.Modules.Reporting.Contracts.DTOs;
using CampaignSaaS.SharedKernel.MultiTenancy;
using ErrorOr;
using MediatR;

public class GetReportByIdQueryHandler : IRequestHandler<GetReportByIdQuery, ErrorOr<CampaignReportDto>>
{
    private readonly ICampaignReportRepository _reportRepository;
    private readonly IReportStorageService _storageService;
    private readonly ICurrentTenantContext _tenantContext;

    public GetReportByIdQueryHandler(
        ICampaignReportRepository reportRepository,
        IReportStorageService storageService,
        ICurrentTenantContext tenantContext)
    {
        _reportRepository = reportRepository;
        _storageService = storageService;
        _tenantContext = tenantContext;
    }

    public async Task<ErrorOr<CampaignReportDto>> Handle(GetReportByIdQuery request, CancellationToken cancellationToken)
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

        string? downloadUrl = null;
        if (!string.IsNullOrEmpty(report.FileObjectKey))
        {
            try
            {
                downloadUrl = await _storageService.GeneratePresignedDownloadUrlAsync(report.FileObjectKey, 60, cancellationToken);
            }
            catch
            {
                downloadUrl = $"/api/v1/files/download?key={Uri.EscapeDataString(report.FileObjectKey)}";
            }
        }

        return new CampaignReportDto(
            report.Id,
            report.OrganizationId,
            report.CampaignId,
            report.Status.ToString(),
            report.FileObjectKey,
            downloadUrl,
            report.ErrorMessage,
            report.RequestedAt,
            report.CompletedAt);
    }
}

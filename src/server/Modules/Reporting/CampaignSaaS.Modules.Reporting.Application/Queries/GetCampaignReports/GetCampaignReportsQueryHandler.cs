namespace CampaignSaaS.Modules.Reporting.Application.Queries.GetCampaignReports;

using CampaignSaaS.Modules.Reporting.Application.Abstractions;
using CampaignSaaS.Modules.Reporting.Contracts.DTOs;
using CampaignSaaS.SharedKernel.MultiTenancy;
using ErrorOr;
using MediatR;

public class GetCampaignReportsQueryHandler : IRequestHandler<GetCampaignReportsQuery, ErrorOr<IReadOnlyList<CampaignReportDto>>>
{
    private readonly ICampaignReportRepository _reportRepository;
    private readonly IReportStorageService _storageService;
    private readonly ICurrentTenantContext _tenantContext;

    public GetCampaignReportsQueryHandler(
        ICampaignReportRepository reportRepository,
        IReportStorageService storageService,
        ICurrentTenantContext tenantContext)
    {
        _reportRepository = reportRepository;
        _storageService = storageService;
        _tenantContext = tenantContext;
    }

    public async Task<ErrorOr<IReadOnlyList<CampaignReportDto>>> Handle(GetCampaignReportsQuery request, CancellationToken cancellationToken)
    {
        if (!_tenantContext.OrganizationId.HasValue)
        {
            return Error.Forbidden("Tenant.Required", "Active organization context is required.");
        }

        var reports = await _reportRepository.GetByCampaignIdAsync(request.CampaignId, cancellationToken);
        var dtos = new List<CampaignReportDto>();

        foreach (var report in reports)
        {
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

            dtos.Add(new CampaignReportDto(
                report.Id,
                report.OrganizationId,
                report.CampaignId,
                report.Status.ToString(),
                report.FileObjectKey,
                downloadUrl,
                report.ErrorMessage,
                report.RequestedAt,
                report.CompletedAt));
        }

        return dtos;
    }
}

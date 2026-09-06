namespace CampaignSaaS.Modules.Reporting.Application.Commands.GenerateCampaignReport;

using CampaignSaaS.Modules.Reporting.Application.Abstractions;
using CampaignSaaS.Modules.Reporting.Contracts.DTOs;
using CampaignSaaS.Modules.Reporting.Domain.Entities;
using CampaignSaaS.SharedKernel.MultiTenancy;
using ErrorOr;
using MediatR;

public class GenerateCampaignReportCommandHandler : IRequestHandler<GenerateCampaignReportCommand, ErrorOr<CampaignReportDto>>
{
    private readonly ICampaignReportRepository _reportRepository;
    private readonly IReportingUnitOfWork _unitOfWork;
    private readonly IJsReportService _jsReportService;
    private readonly IReportStorageService _storageService;
    private readonly ICurrentTenantContext _tenantContext;

    public GenerateCampaignReportCommandHandler(
        ICampaignReportRepository reportRepository,
        IReportingUnitOfWork unitOfWork,
        IJsReportService jsReportService,
        IReportStorageService storageService,
        ICurrentTenantContext tenantContext)
    {
        _reportRepository = reportRepository;
        _unitOfWork = unitOfWork;
        _jsReportService = jsReportService;
        _storageService = storageService;
        _tenantContext = tenantContext;
    }

    public async Task<ErrorOr<CampaignReportDto>> Handle(GenerateCampaignReportCommand request, CancellationToken cancellationToken)
    {
        if (!_tenantContext.OrganizationId.HasValue)
        {
            return Error.Forbidden("Tenant.Required", "Active organization context is required.");
        }

        var orgId = _tenantContext.OrganizationId.Value;

        var report = CampaignReport.RequestReport(orgId, request.CampaignId);
        await _reportRepository.AddAsync(report, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        try
        {
            report.MarkAsGenerating();
            _reportRepository.Update(report);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var reportPayload = new CampaignReportPayloadDto(
                new OrganizationInfo("Agency Organization", null),
                new CampaignInfo("Campaign Report", "Client", DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(-1)), DateOnly.FromDateTime(DateTime.UtcNow), 0, "IDR"),
                new SummaryMetricsInfo(0, 0, 0, 0, 0, 0, 0, 0, 0),
                Array.Empty<DeliverableMetricItem>());

            var pdfBytes = await _jsReportService.RenderReportPdfAsync(reportPayload, cancellationToken);

            var objectKey = await _storageService.UploadReportPdfAsync(orgId, request.CampaignId, report.Id, pdfBytes, cancellationToken);

            report.MarkAsCompleted(objectKey);
            _reportRepository.Update(report);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            string? downloadUrl = null;
            try
            {
                downloadUrl = await _storageService.GeneratePresignedDownloadUrlAsync(objectKey, 60, cancellationToken);
            }
            catch
            {
                downloadUrl = $"/api/v1/files/download?key={Uri.EscapeDataString(objectKey)}";
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
        catch (Exception ex)
        {
            report.MarkAsFailed(ex.Message);
            _reportRepository.Update(report);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new CampaignReportDto(
                report.Id,
                report.OrganizationId,
                report.CampaignId,
                report.Status.ToString(),
                report.FileObjectKey,
                null,
                report.ErrorMessage,
                report.RequestedAt,
                report.CompletedAt);
        }
    }
}

namespace CampaignSaaS.Modules.Reporting.Application.Abstractions;

public interface IReportStorageService
{
    Task<string> UploadReportPdfAsync(Guid organizationId, Guid campaignId, Guid reportId, byte[] pdfBytes, CancellationToken cancellationToken = default);
    Task<string> GeneratePresignedDownloadUrlAsync(string objectKey, int expiryMinutes = 60, CancellationToken cancellationToken = default);
}

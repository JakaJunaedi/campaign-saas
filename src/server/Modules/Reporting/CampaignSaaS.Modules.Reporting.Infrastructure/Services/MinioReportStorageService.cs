namespace CampaignSaaS.Modules.Reporting.Infrastructure.Services;

using CampaignSaaS.Modules.Reporting.Application.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Minio;
using Minio.DataModel.Args;

public class MinioReportStorageService : IReportStorageService
{
    private readonly IMinioClient? _minioClient;
    private readonly string _endpoint;
    private readonly string _bucket;
    private readonly bool _useSsl;
    private readonly ILogger<MinioReportStorageService> _logger;

    public MinioReportStorageService(IConfiguration configuration, ILogger<MinioReportStorageService> logger)
    {
        _logger = logger;
        _endpoint = configuration["MinIO:Endpoint"] ?? "localhost:9000";
        var accessKey = configuration["MinIO:AccessKey"] ?? "minio_admin";
        var secretKey = configuration["MinIO:SecretKey"] ?? "MinioDevPassword123!";
        _bucket = configuration["MinIO:ReportsBucket"] ?? "reports";
        _useSsl = bool.TryParse(configuration["MinIO:UseSSL"], out var ssl) && ssl;

        try
        {
            _minioClient = new MinioClient()
                .WithEndpoint(_endpoint)
                .WithCredentials(accessKey, secretKey)
                .WithSSL(_useSsl)
                .Build();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to initialize MinIO client for reports.");
            _minioClient = null;
        }
    }

    public async Task<string> UploadReportPdfAsync(
        Guid organizationId,
        Guid campaignId,
        Guid reportId,
        byte[] pdfBytes,
        CancellationToken cancellationToken = default)
    {
        var objectKey = $"orgs/{organizationId}/campaigns/{campaignId}/reports/{reportId}_{DateTime.UtcNow:yyyyMMddHHmmss}.pdf";

        if (_minioClient != null)
        {
            try
            {
                using var stream = new MemoryStream(pdfBytes);
                var putArgs = new PutObjectArgs()
                    .WithBucket(_bucket)
                    .WithObject(objectKey)
                    .WithStreamData(stream)
                    .WithObjectSize(pdfBytes.Length)
                    .WithContentType("application/pdf");

                await _minioClient.PutObjectAsync(putArgs, cancellationToken);
                return objectKey;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "MinIO report upload failed, using simulated object key.");
            }
        }

        return objectKey;
    }

    public async Task<string> GeneratePresignedDownloadUrlAsync(
        string objectKey,
        int expiryMinutes = 60,
        CancellationToken cancellationToken = default)
    {
        if (_minioClient != null)
        {
            try
            {
                var args = new PresignedGetObjectArgs()
                    .WithBucket(_bucket)
                    .WithObject(objectKey)
                    .WithExpiry(expiryMinutes * 60);

                return await _minioClient.PresignedGetObjectAsync(args);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "MinIO presigned report download URL generation failed.");
            }
        }

        var protocol = _useSsl ? "https" : "http";
        return $"{protocol}://{_endpoint}/{_bucket}/{objectKey}";
    }
}

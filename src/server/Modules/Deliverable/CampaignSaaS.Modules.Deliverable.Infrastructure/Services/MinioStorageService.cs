namespace CampaignSaaS.Modules.Deliverable.Infrastructure.Services;

using CampaignSaaS.Modules.Deliverable.Application.Abstractions;
using CampaignSaaS.Modules.Deliverable.Contracts.DTOs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Minio;
using Minio.DataModel.Args;

public class MinioStorageService : IStorageService
{
    private readonly IMinioClient? _minioClient;
    private readonly string _endpoint;
    private readonly string _defaultBucket;
    private readonly bool _useSsl;
    private readonly ILogger<MinioStorageService> _logger;

    public MinioStorageService(IConfiguration configuration, ILogger<MinioStorageService> logger)
    {
        _logger = logger;
        _endpoint = configuration["MinIO:Endpoint"] ?? "localhost:9000";
        var accessKey = configuration["MinIO:AccessKey"] ?? "minio_admin";
        var secretKey = configuration["MinIO:SecretKey"] ?? "MinioDevPassword123!";
        _defaultBucket = configuration["MinIO:DefaultBucket"] ?? "creator-content";
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
            _logger.LogWarning(ex, "Failed to initialize MinIO client. Storage service will use simulated presigned URLs.");
            _minioClient = null;
        }
    }

    public async Task<PresignedUploadUrlDto> GeneratePresignedUploadUrlAsync(
        Guid organizationId,
        string fileName,
        string contentType,
        long fileSize,
        Guid? campaignId = null,
        Guid? deliverableId = null,
        int? version = null,
        CancellationToken cancellationToken = default)
    {
        var safeFileName = Path.GetFileName(fileName);
        var fileUuid = Guid.NewGuid().ToString("N");
        var versionSegment = version.HasValue ? $"v{version.Value}" : "drafts";
        var campaignSegment = campaignId.HasValue ? campaignId.Value.ToString() : "general";
        var deliverableSegment = deliverableId.HasValue ? deliverableId.Value.ToString() : "unassigned";

        // Object Key Convention: orgs/{orgId}/campaigns/{campaignId}/{deliverableId}/{version}/{uuid}_{fileName}
        var objectKey = $"orgs/{organizationId}/campaigns/{campaignSegment}/{deliverableSegment}/{versionSegment}/{fileUuid}_{safeFileName}";
        var expiryMinutes = 15;

        if (_minioClient != null)
        {
            try
            {
                var beArgs = new BucketExistsArgs().WithBucket(_defaultBucket);
                bool found = await _minioClient.BucketExistsAsync(beArgs, cancellationToken);
                if (!found)
                {
                    var mbArgs = new MakeBucketArgs().WithBucket(_defaultBucket);
                    await _minioClient.MakeBucketAsync(mbArgs, cancellationToken);
                }

                var args = new PresignedPutObjectArgs()
                    .WithBucket(_defaultBucket)
                    .WithObject(objectKey)
                    .WithExpiry(expiryMinutes * 60);

                var url = await _minioClient.PresignedPutObjectAsync(args);
                return new PresignedUploadUrlDto(url, objectKey, expiryMinutes);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "MinIO presigned PUT generation failed, falling back to direct URL.");
            }
        }

        var protocol = _useSsl ? "https" : "http";
        var fallbackUrl = $"{protocol}://{_endpoint}/{_defaultBucket}/{objectKey}";
        return new PresignedUploadUrlDto(fallbackUrl, objectKey, expiryMinutes);
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
                    .WithBucket(_defaultBucket)
                    .WithObject(objectKey)
                    .WithExpiry(expiryMinutes * 60);

                return await _minioClient.PresignedGetObjectAsync(args);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "MinIO presigned GET generation failed, falling back to direct URL.");
            }
        }

        var protocol = _useSsl ? "https" : "http";
        return $"{protocol}://{_endpoint}/{_defaultBucket}/{objectKey}";
    }
}


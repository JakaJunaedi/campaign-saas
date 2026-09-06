namespace CampaignSaaS.Modules.Reporting.Application.Abstractions;

using CampaignSaaS.Modules.Reporting.Contracts.DTOs;

public interface IJsReportService
{
    Task<byte[]> RenderReportPdfAsync(CampaignReportPayloadDto payload, CancellationToken cancellationToken = default);
}

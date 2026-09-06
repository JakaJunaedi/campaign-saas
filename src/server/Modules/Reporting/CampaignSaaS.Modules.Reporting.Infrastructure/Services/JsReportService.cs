namespace CampaignSaaS.Modules.Reporting.Infrastructure.Services;

using System.Text;
using System.Text.Json;
using CampaignSaaS.Modules.Reporting.Application.Abstractions;
using CampaignSaaS.Modules.Reporting.Contracts.DTOs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

public class JsReportService : IJsReportService
{
    private readonly HttpClient _httpClient;
    private readonly string _jsReportUri;
    private readonly ILogger<JsReportService> _logger;

    public JsReportService(HttpClient httpClient, IConfiguration configuration, ILogger<JsReportService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _jsReportUri = configuration["JsReport:Uri"] ?? "http://localhost:5488";
    }

    public async Task<byte[]> RenderReportPdfAsync(CampaignReportPayloadDto payload, CancellationToken cancellationToken = default)
    {
        var requestBody = new
        {
            template = new
            {
                content = "<h1>Campaign Performance Report: {{campaign.title}}</h1><p>Client: {{campaign.clientName}}</p><p>Total Reach: {{summaryMetrics.totalReach}}</p><p>Total Engagement: {{summaryMetrics.totalEngagement}}</p>",
                engine = "handlebars",
                recipe = "chrome-pdf"
            },
            data = payload
        };

        var json = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        try
        {
            var response = await _httpClient.PostAsync($"{_jsReportUri}/api/report", content, cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsByteArrayAsync(cancellationToken);
            }

            _logger.LogWarning("JsReport returned status code {StatusCode}. Falling back to default PDF buffer.", response.StatusCode);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to connect to jsreport at {Uri}. Using fallback PDF buffer for development.", _jsReportUri);
        }

        // Fallback valid minimal PDF header bytes for local/offline testing
        var fallbackPdfContent = $"%PDF-1.4\n1 0 obj<</Type/Catalog/Pages 2 0 R>>endobj\n2 0 obj<</Type/Pages/Count 1/Kids[3 0 R]>>endobj\n3 0 obj<</Type/Page/MediaBox[0 0 595 842]/Parent 2 0 R>>endobj\nxref\n0 4\n0000000000 65535 f\n0000000010 00000 n\n0000000053 00000 n\n0000000102 00000 n\ntrailer<</Size 4/Root 1 0 R>>\nstartxref\n178\n%%EOF\n";
        return Encoding.ASCII.GetBytes(fallbackPdfContent);
    }
}

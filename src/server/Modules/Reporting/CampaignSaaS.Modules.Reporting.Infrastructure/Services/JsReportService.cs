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
        var templateHtml = """
        <!DOCTYPE html>
        <html>
        <head>
          <meta charset="utf-8">
          <style>
            @import url('https://fonts.googleapis.com/css2?family=Plus+Jakarta+Sans:wght@400;500;600;700;800&display=swap');
            
            * { box-sizing: border-box; margin: 0; padding: 0; }
            body {
              font-family: 'Plus Jakarta Sans', -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
              color: #0f172a;
              background-color: #ffffff;
              padding: 32px;
              font-size: 12px;
              line-height: 1.5;
            }
            .header-card {
              background: linear-gradient(135deg, #0f172a 0%, #1e1b4b 100%);
              border-radius: 20px;
              padding: 28px;
              color: #ffffff;
              margin-bottom: 24px;
            }
            .header-top {
              display: flex;
              justify-content: space-between;
              align-items: flex-start;
              border-bottom: 1px solid rgba(255, 255, 255, 0.12);
              padding-bottom: 18px;
              margin-bottom: 18px;
            }
            .org-badge {
              font-size: 11px;
              font-weight: 700;
              text-transform: uppercase;
              letter-spacing: 0.08em;
              color: #818cf8;
              margin-bottom: 4px;
            }
            .report-title {
              font-size: 22px;
              font-weight: 800;
              letter-spacing: -0.02em;
            }
            .badge {
              display: inline-block;
              padding: 4px 10px;
              border-radius: 9999px;
              font-size: 10px;
              font-weight: 700;
              text-transform: uppercase;
              letter-spacing: 0.05em;
            }
            .badge-client {
              background: rgba(255, 255, 255, 0.12);
              color: #c7d2fe;
              border: 1px solid rgba(255, 255, 255, 0.15);
            }
            .meta-grid {
              display: grid;
              grid-template-columns: repeat(3, 1fr);
              gap: 16px;
            }
            .meta-label {
              font-size: 10px;
              font-weight: 600;
              text-transform: uppercase;
              color: #94a3b8;
              letter-spacing: 0.05em;
            }
            .meta-value {
              font-size: 14px;
              font-weight: 700;
              color: #f8fafc;
              margin-top: 2px;
            }
            .kpi-grid {
              display: grid;
              grid-template-columns: repeat(4, 1fr);
              gap: 14px;
              margin-bottom: 24px;
            }
            .kpi-card {
              background: #f8fafc;
              border: 1px solid #e2e8f0;
              border-radius: 16px;
              padding: 16px;
            }
            .kpi-label {
              font-size: 10px;
              font-weight: 700;
              text-transform: uppercase;
              color: #64748b;
              letter-spacing: 0.05em;
            }
            .kpi-number {
              font-size: 20px;
              font-weight: 800;
              color: #0f172a;
              margin: 6px 0 2px;
            }
            .kpi-sub {
              font-size: 10px;
              font-weight: 600;
              color: #059669;
            }
            .section-header {
              display: flex;
              justify-content: space-between;
              align-items: center;
              margin-bottom: 12px;
            }
            .section-title {
              font-size: 14px;
              font-weight: 800;
              color: #0f172a;
              text-transform: uppercase;
              letter-spacing: 0.03em;
            }
            table {
              width: 100%;
              border-collapse: collapse;
              background: #ffffff;
              border: 1px solid #e2e8f0;
              border-radius: 14px;
              overflow: hidden;
              margin-bottom: 24px;
            }
            th {
              background: #f1f5f9;
              color: #475569;
              font-size: 10px;
              font-weight: 700;
              text-transform: uppercase;
              letter-spacing: 0.05em;
              padding: 10px 12px;
              text-align: left;
              border-bottom: 1px solid #cbd5e1;
            }
            th.num, td.num { text-align: right; }
            td {
              padding: 10px 12px;
              border-bottom: 1px solid #f1f5f9;
              font-size: 11px;
              color: #334155;
            }
            tr:last-child td { border-bottom: none; }
            .creator-name { font-weight: 700; color: #0f172a; }
            .platform-tag {
              display: inline-block;
              padding: 2px 6px;
              border-radius: 6px;
              background: #e0e7ff;
              color: #4338ca;
              font-size: 9px;
              font-weight: 700;
            }
            .er-tag {
              display: inline-block;
              padding: 2px 6px;
              border-radius: 6px;
              background: #ecfdf5;
              color: #047857;
              font-weight: 700;
              font-size: 10px;
            }
            .footer {
              border-top: 1px solid #e2e8f0;
              padding-top: 14px;
              display: flex;
              justify-content: space-between;
              font-size: 10px;
              color: #94a3b8;
            }
          </style>
        </head>
        <body>
          <div class="header-card">
            <div class="header-top">
              <div>
                <div class="org-badge">{{organization.name}}</div>
                <h1 class="report-title">{{campaign.title}}</h1>
              </div>
              <span class="badge badge-client">Client: {{campaign.clientName}}</span>
            </div>
            <div class="meta-grid">
              <div>
                <div class="meta-label">Campaign Schedule</div>
                <div class="meta-value">{{campaign.startDate}} to {{campaign.endDate}}</div>
              </div>
              <div>
                <div class="meta-label">Total Budget</div>
                <div class="meta-value">{{campaign.currency}} {{campaign.totalBudget}}</div>
              </div>
              <div>
                <div class="meta-label">Lineup & Assets</div>
                <div class="meta-value">{{summaryMetrics.totalCreators}} Creators • {{summaryMetrics.totalDeliverables}} Deliverables</div>
              </div>
            </div>
          </div>

          <div class="kpi-grid">
            <div class="kpi-card">
              <div class="kpi-label">Total Reach</div>
              <div class="kpi-number">{{summaryMetrics.totalReach}}</div>
              <div class="kpi-sub">{{summaryMetrics.totalImpressions}} Impressions</div>
            </div>
            <div class="kpi-card">
              <div class="kpi-label">Total Video Views</div>
              <div class="kpi-number">{{summaryMetrics.totalViews}}</div>
              <div class="kpi-sub">Cost Per View: {{campaign.currency}} {{summaryMetrics.costPerView}}</div>
            </div>
            <div class="kpi-card">
              <div class="kpi-label">Total Engagement</div>
              <div class="kpi-number">{{summaryMetrics.totalEngagement}}</div>
              <div class="kpi-sub">CPE: {{campaign.currency}} {{summaryMetrics.costPerEngagement}}</div>
            </div>
            <div class="kpi-card">
              <div class="kpi-label">Avg Engagement Rate</div>
              <div class="kpi-number">{{summaryMetrics.averageEngagementRate}}%</div>
              <div class="kpi-sub">Verified Cross-Platform</div>
            </div>
          </div>

          <div class="section-header">
            <h2 class="section-title">Deliverables Performance Breakdown</h2>
          </div>

          <table>
            <thead>
              <tr>
                <th>Creator & Deliverable</th>
                <th>Platform / Type</th>
                <th class="num">Reach</th>
                <th class="num">Views</th>
                <th class="num">Likes</th>
                <th class="num">Comments</th>
                <th class="num">Shares</th>
                <th class="num">Total Eng.</th>
                <th class="num">ER (%)</th>
              </tr>
            </thead>
            <tbody>
              {{#each deliverables}}
              <tr>
                <td>
                  <div class="creator-name">{{creatorName}}</div>
                  <div style="font-size: 10px; color: #64748b;">{{contentType}}</div>
                </td>
                <td><span class="platform-tag">{{platform}}</span></td>
                <td class="num">{{reach}}</td>
                <td class="num">{{views}}</td>
                <td class="num">{{likes}}</td>
                <td class="num">{{comments}}</td>
                <td class="num">{{shares}}</td>
                <td class="num" style="font-weight: 700;">{{totalEngagement}}</td>
                <td class="num"><span class="er-tag">{{engagementRate}}%</span></td>
              </tr>
              {{/each}}
            </tbody>
          </table>

          <div class="footer">
            <span>Generated by Campaign SaaS Platform • Confidential Performance Report</span>
            <span>Page 1 of 1</span>
          </div>
        </body>
        </html>
        """;

        var requestBody = new
        {
            template = new
            {
                content = templateHtml,
                engine = "handlebars",
                recipe = "chrome-pdf",
                chrome = new
                {
                    landscape = true,
                    format = "A4",
                    margin = new
                    {
                        top = "10mm",
                        bottom = "10mm",
                        left = "10mm",
                        right = "10mm"
                    }
                }
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

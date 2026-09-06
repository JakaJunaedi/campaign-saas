namespace CampaignSaaS.Modules.Reporting.Infrastructure;

using CampaignSaaS.Modules.Reporting.Application.Abstractions;
using CampaignSaaS.Modules.Reporting.Infrastructure.Persistence;
using CampaignSaaS.Modules.Reporting.Infrastructure.Persistence.Repositories;
using CampaignSaaS.Modules.Reporting.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddReportingInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? configuration.GetConnectionString("PostgreSQL");

        services.AddDbContext<ReportingDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<IReportingMetricRepository, ReportingMetricRepository>();
        services.AddScoped<ICampaignReportRepository, CampaignReportRepository>();
        services.AddScoped<IReportingUnitOfWork, ReportingUnitOfWork>();

        services.AddHttpClient<IJsReportService, JsReportService>();
        services.AddScoped<IReportStorageService, MinioReportStorageService>();

        return services;
    }
}

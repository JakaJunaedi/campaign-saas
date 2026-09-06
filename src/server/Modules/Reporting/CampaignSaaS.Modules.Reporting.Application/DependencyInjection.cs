namespace CampaignSaaS.Modules.Reporting.Application;

using CampaignSaaS.Modules.Reporting.Application.Services;
using CampaignSaaS.Modules.Reporting.Contracts;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddReportingApplication(this IServiceCollection services)
    {
        var assembly = typeof(DependencyInjection).Assembly;

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));
        services.AddValidatorsFromAssembly(assembly);
        services.AddScoped<IReportingSummaryQueryService, ReportingSummaryQueryService>();

        return services;
    }
}


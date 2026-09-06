namespace CampaignSaaS.Modules.Deliverable.Application;

using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddDeliverableApplication(this IServiceCollection services)
    {
        var assembly = typeof(DependencyInjection).Assembly;
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));
        services.AddValidatorsFromAssembly(assembly);
        services.AddScoped<CampaignSaaS.Modules.Deliverable.Contracts.IDeliverableSummaryQueryService, CampaignSaaS.Modules.Deliverable.Application.Services.DeliverableSummaryQueryService>();
        return services;
    }
}


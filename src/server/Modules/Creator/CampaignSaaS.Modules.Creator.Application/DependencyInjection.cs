namespace CampaignSaaS.Modules.Creator.Application;

using CampaignSaaS.Modules.Creator.Application.Services;
using CampaignSaaS.Modules.Creator.Contracts;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddCreatorApplication(this IServiceCollection services)
    {
        var assembly = typeof(DependencyInjection).Assembly;

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));
        services.AddValidatorsFromAssembly(assembly);
        services.AddScoped<ICreatorSummaryQueryService, CreatorSummaryQueryService>();

        return services;
    }
}


namespace CampaignSaaS.Modules.Client.Application;

using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddClientApplication(this IServiceCollection services)
    {
        var assembly = typeof(DependencyInjection).Assembly;

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));
        services.AddValidatorsFromAssembly(assembly);
        services.AddScoped<CampaignSaaS.Modules.Client.Contracts.IClientSummaryQueryService, CampaignSaaS.Modules.Client.Application.Services.ClientSummaryQueryService>();

        return services;
    }
}

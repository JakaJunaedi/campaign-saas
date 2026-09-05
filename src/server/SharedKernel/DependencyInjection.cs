namespace CampaignSaaS.SharedKernel;

using CampaignSaaS.SharedKernel.Application.Behaviors;
using CampaignSaaS.SharedKernel.MultiTenancy;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddSharedKernel(this IServiceCollection services)
    {
        services.AddScoped<ICurrentTenantContext, CurrentTenantContext>();

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        return services;
    }
}

namespace CampaignSaaS.Modules.Creator.Infrastructure;

using CampaignSaaS.Modules.Creator.Application.Abstractions;
using CampaignSaaS.Modules.Creator.Infrastructure.Persistence;
using CampaignSaaS.Modules.Creator.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddCreatorInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("PostgreSQL")
            ?? "Host=localhost;Port=5432;Database=campaign_saas;Username=postgres;Password=postgres";

        services.AddDbContext<CreatorDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<ICreatorRepository, CreatorRepository>();
        services.AddScoped<ICreatorUnitOfWork, CreatorUnitOfWork>();

        return services;
    }
}

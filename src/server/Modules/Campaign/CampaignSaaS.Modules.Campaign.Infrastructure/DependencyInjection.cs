namespace CampaignSaaS.Modules.Campaign.Infrastructure;

using CampaignSaaS.Modules.Campaign.Application.Abstractions;
using CampaignSaaS.Modules.Campaign.Infrastructure.Persistence;
using CampaignSaaS.Modules.Campaign.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddCampaignInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("PostgreSQL")
            ?? "Host=localhost;Port=5432;Database=campaign_saas;Username=postgres;Password=postgres";

        services.AddDbContext<CampaignDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<ICampaignRepository, CampaignRepository>();
        services.AddScoped<ICampaignCreatorRepository, CampaignCreatorRepository>();
        services.AddScoped<ICampaignUnitOfWork, CampaignUnitOfWork>();

        return services;
    }
}

namespace CampaignSaaS.Modules.Client.Infrastructure;

using CampaignSaaS.Modules.Client.Application.Abstractions;
using CampaignSaaS.Modules.Client.Infrastructure.Persistence;
using CampaignSaaS.Modules.Client.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddClientInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("PostgreSQL")
            ?? "Host=localhost;Port=5432;Database=campaign_saas;Username=postgres;Password=postgres";

        services.AddDbContext<ClientDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<IClientRepository, ClientRepository>();
        services.AddScoped<IClientUnitOfWork, ClientUnitOfWork>();

        return services;
    }
}

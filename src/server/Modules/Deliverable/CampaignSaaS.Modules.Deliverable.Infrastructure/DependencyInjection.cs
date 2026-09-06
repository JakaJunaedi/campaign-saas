namespace CampaignSaaS.Modules.Deliverable.Infrastructure;

using CampaignSaaS.Modules.Deliverable.Application.Abstractions;
using CampaignSaaS.Modules.Deliverable.Infrastructure.Persistence;
using CampaignSaaS.Modules.Deliverable.Infrastructure.Persistence.Repositories;
using CampaignSaaS.Modules.Deliverable.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddDeliverableInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? configuration.GetConnectionString("PostgreSQL");

        services.AddDbContext<DeliverableDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<IDeliverableRepository, DeliverableRepository>();
        services.AddScoped<IContentSubmissionRepository, ContentSubmissionRepository>();
        services.AddScoped<IDeliverableUnitOfWork, DeliverableUnitOfWork>();
        services.AddScoped<IStorageService, MinioStorageService>();

        return services;
    }
}


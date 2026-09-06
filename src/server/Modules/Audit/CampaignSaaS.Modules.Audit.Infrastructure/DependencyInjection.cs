namespace CampaignSaaS.Modules.Audit.Infrastructure;

using CampaignSaaS.Modules.Audit.Application.Abstractions;
using CampaignSaaS.Modules.Audit.Infrastructure.Persistence;
using CampaignSaaS.Modules.Audit.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddAuditInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? configuration.GetConnectionString("PostgreSQL");

        services.AddDbContext<AuditDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<IAuditLogRepository, AuditLogRepository>();
        services.AddScoped<IAuditUnitOfWork, AuditUnitOfWork>();

        return services;
    }
}

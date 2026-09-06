namespace CampaignSaaS.Modules.Approval.Infrastructure;

using CampaignSaaS.Modules.Approval.Application.Abstractions;
using CampaignSaaS.Modules.Approval.Infrastructure.Persistence;
using CampaignSaaS.Modules.Approval.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddApprovalInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? configuration.GetConnectionString("PostgreSQL");

        services.AddDbContext<ApprovalDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<IApprovalReviewRepository, ApprovalReviewRepository>();
        services.AddScoped<IApprovalUnitOfWork, ApprovalUnitOfWork>();

        return services;
    }
}

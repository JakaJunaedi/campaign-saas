namespace CampaignSaaS.SharedKernel.Infrastructure;

using CampaignSaaS.SharedKernel.Infrastructure.Persistence;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddSharedKernelInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<IBusRegistrationConfigurator>? configureConsumers = null)
    {
        var connectionString = configuration.GetConnectionString("PostgreSQL")
            ?? configuration.GetConnectionString("DefaultConnection")
            ?? "Host=localhost;Port=5432;Database=campaign_saas_dev;Username=saas_admin;Password=DevPassword123!";

        services.AddDbContextFactory<IntegrationDbContext>(options =>
        {
            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.MigrationsAssembly(typeof(IntegrationDbContext).Assembly.FullName);
            });
        });

        // Scoped resolution must come from the singleton factory (avoid captive dependency
        // between the singleton IDbContextFactory and the scoped DbContextOptions).
        services.AddScoped<IntegrationDbContext>(sp =>
            sp.GetRequiredService<IDbContextFactory<IntegrationDbContext>>().CreateDbContext());

        services.AddMassTransit(busConfig =>
        {
            busConfig.SetKebabCaseEndpointNameFormatter();

            configureConsumers?.Invoke(busConfig);

            busConfig.AddEntityFrameworkOutbox<IntegrationDbContext>(o =>
            {
                o.UsePostgres();
                o.UseBusOutbox();
            });

            busConfig.UsingRabbitMq((context, cfg) =>
            {
                var rabbitHost = configuration["RabbitMQ:Host"] ?? "localhost";
                var rabbitPort = ushort.TryParse(configuration["RabbitMQ:Port"], out var port) ? port : (ushort)5672;
                var rabbitUser = configuration["RabbitMQ:Username"] ?? "rabbit_user";
                var rabbitPass = configuration["RabbitMQ:Password"] ?? "RabbitDev123!";

                cfg.Host(rabbitHost, rabbitPort, "/", h =>
                {
                    h.Username(rabbitUser);
                    h.Password(rabbitPass);
                });

                cfg.ConfigureEndpoints(context);
            });
        });

        return services;
    }
}

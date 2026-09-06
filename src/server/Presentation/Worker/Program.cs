using CampaignSaaS.Modules.Audit.Application;
using CampaignSaaS.Modules.Audit.Application.Consumers;
using CampaignSaaS.Modules.Audit.Infrastructure;
using CampaignSaaS.Modules.Campaign.Application;
using CampaignSaaS.Modules.Campaign.Infrastructure;
using CampaignSaaS.Modules.Client.Application;
using CampaignSaaS.Modules.Client.Infrastructure;
using CampaignSaaS.Modules.Deliverable.Application;
using CampaignSaaS.Modules.Deliverable.Infrastructure;
using CampaignSaaS.Modules.Notification.Application;
using CampaignSaaS.Modules.Notification.Application.Consumers;
using CampaignSaaS.Modules.Notification.Infrastructure;
using CampaignSaaS.Modules.Reporting.Application;
using CampaignSaaS.Modules.Reporting.Application.Consumers;
using CampaignSaaS.Modules.Reporting.Infrastructure;
using CampaignSaaS.SharedKernel;
using CampaignSaaS.SharedKernel.Infrastructure;
using MassTransit;
using Serilog;

var builder = Host.CreateApplicationBuilder(args);

// 1. Serilog Setup
builder.Services.AddSerilog(lc => lc.WriteTo.Console());

// 2. Shared Kernel & Multi-Tenancy
builder.Services.AddSharedKernel();

// 3. Modules Registration for Worker Aggregations
builder.Services.AddClientApplication();
builder.Services.AddClientInfrastructure(builder.Configuration);

builder.Services.AddCampaignApplication();
builder.Services.AddCampaignInfrastructure(builder.Configuration);

builder.Services.AddDeliverableApplication();
builder.Services.AddDeliverableInfrastructure(builder.Configuration);

builder.Services.AddReportingApplication();
builder.Services.AddReportingInfrastructure(builder.Configuration);

builder.Services.AddNotificationApplication();
builder.Services.AddNotificationInfrastructure(builder.Configuration);

builder.Services.AddAuditApplication();
builder.Services.AddAuditInfrastructure(builder.Configuration);

// 4. MassTransit Consumers Configuration
builder.Services.AddSharedKernelInfrastructure(builder.Configuration, busConfig =>
{
    busConfig.AddConsumer<GenerateCampaignReportConsumer>();
    busConfig.AddConsumer<ApprovalNotificationConsumer>();
    busConfig.AddConsumer<DeliverableNotificationConsumer>();
    busConfig.AddConsumer<ReportNotificationConsumer>();
    busConfig.AddConsumer<AuditIntegrationEventConsumer>();
});

var host = builder.Build();
host.Run();

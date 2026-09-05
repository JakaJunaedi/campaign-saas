using System.Text;
using CampaignSaaS.Api.Common;
using CampaignSaaS.Modules.Identity.Application;
using CampaignSaaS.Modules.Identity.Infrastructure;
using CampaignSaaS.Modules.Client.Application;
using CampaignSaaS.Modules.Client.Infrastructure;
using CampaignSaaS.Modules.Creator.Application;
using CampaignSaaS.Modules.Creator.Infrastructure;
using CampaignSaaS.SharedKernel;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// 1. Serilog Setup
builder.Host.UseSerilog((ctx, lc) => lc
    .WriteTo.Console()
    .ReadFrom.Configuration(ctx.Configuration));

// 2. Services Registration
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Campaign SaaS API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Format: Bearer {token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// 3. SharedKernel & MultiTenancy
builder.Services.AddSharedKernel();

// 4. Modules Registration
builder.Services.AddIdentityApplication();
builder.Services.AddIdentityInfrastructure(builder.Configuration);

builder.Services.AddClientApplication();
builder.Services.AddClientInfrastructure(builder.Configuration);

builder.Services.AddCreatorApplication();
builder.Services.AddCreatorInfrastructure(builder.Configuration);

// 5. Authentication & JWT Setup
var jwtSecretKey = builder.Configuration["JwtOptions:SecretKey"] ?? "SuperSecretKeyForCampaignSaaSApp2026_Minimum32Chars!";
var jwtIssuer = builder.Configuration["JwtOptions:Issuer"] ?? "CampaignSaaS";
var jwtAudience = builder.Configuration["JwtOptions:Audience"] ?? "CampaignSaaSClient";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecretKey)),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// 6. Health Checks
builder.Services.AddHealthChecks();

var app = builder.Build();

// 7. HTTP Request Pipeline
if (app.Environment.IsDevelopment() || true) // Always enable swagger in local dev mode
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Campaign SaaS API v1");
        c.RoutePrefix = "swagger";
    });
}

app.UseRouting();

app.UseAuthentication();
app.UseMiddleware<CurrentTenantMiddleware>();
app.UseAuthorization();

// Health check endpoint
app.MapHealthChecks("/healthz");

// Root Ping
app.MapGet("/", () => new { Status = "Online", Service = "Campaign SaaS API (.NET 10 Modular Monolith)", Timestamp = DateTime.UtcNow });

app.MapControllers();

app.Run();

// Required for WebApplicationFactory in IntegrationTests
public partial class Program { }

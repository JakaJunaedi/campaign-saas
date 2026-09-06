namespace CampaignSaaS.Modules.Identity.Application.Commands.ImpersonateTenant;

using CampaignSaaS.Modules.Identity.Contracts.DTOs;
using CampaignSaaS.SharedKernel.Application;

public record ImpersonateTenantCommand(Guid OrganizationId) : ICommand<AuthResultDto>;

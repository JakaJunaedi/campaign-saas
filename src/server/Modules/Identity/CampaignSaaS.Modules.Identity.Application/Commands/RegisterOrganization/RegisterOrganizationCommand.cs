namespace CampaignSaaS.Modules.Identity.Application.Commands.RegisterOrganization;

using CampaignSaaS.Modules.Identity.Contracts.DTOs;
using CampaignSaaS.SharedKernel.Application;

public record RegisterOrganizationCommand(
    string OrganizationName,
    string Slug,
    string AdminFullName,
    string AdminEmail,
    string Password) : ICommand<AuthResultDto>;

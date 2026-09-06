namespace CampaignSaaS.Modules.Identity.Application.Commands.CreateOrganization;

using CampaignSaaS.Modules.Identity.Contracts.DTOs;
using CampaignSaaS.SharedKernel.Application;

public record CreateOrganizationCommand(
    string OrganizationName,
    string Slug,
    string AdminFullName,
    string AdminEmail,
    string Password) : ICommand<AdminOrganizationItemDto>;

namespace CampaignSaaS.Modules.Identity.Application.Commands.UpdateOrganizationStatus;

using CampaignSaaS.Modules.Identity.Contracts.DTOs;
using CampaignSaaS.SharedKernel.Application;

public record UpdateOrganizationStatusCommand(
    Guid OrganizationId,
    string Status) : ICommand<AdminOrganizationItemDto>;

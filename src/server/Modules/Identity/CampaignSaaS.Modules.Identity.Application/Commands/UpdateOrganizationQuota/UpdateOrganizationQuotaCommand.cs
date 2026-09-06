namespace CampaignSaaS.Modules.Identity.Application.Commands.UpdateOrganizationQuota;

using CampaignSaaS.SharedKernel.Application;

public record UpdateOrganizationQuotaCommand(
    Guid OrganizationId,
    long StorageQuotaBytes,
    int MaxActiveCampaigns) : ICommand;

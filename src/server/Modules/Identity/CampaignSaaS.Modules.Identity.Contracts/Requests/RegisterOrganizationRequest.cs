namespace CampaignSaaS.Modules.Identity.Contracts.Requests;

public record RegisterOrganizationRequest(
    string OrganizationName,
    string Slug,
    string AdminFullName,
    string AdminEmail,
    string Password);

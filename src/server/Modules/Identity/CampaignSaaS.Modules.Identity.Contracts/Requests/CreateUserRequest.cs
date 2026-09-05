namespace CampaignSaaS.Modules.Identity.Contracts.Requests;

public record CreateUserRequest(
    string FullName,
    string Email,
    string Password,
    string Role);

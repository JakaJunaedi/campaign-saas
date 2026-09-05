namespace CampaignSaaS.Modules.Identity.Contracts.Requests;

public record LoginRequest(
    string Email,
    string Password);

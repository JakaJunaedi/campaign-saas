namespace CampaignSaaS.Modules.Client.Contracts.Requests;

using CampaignSaaS.Modules.Client.Contracts.DTOs;

public record CreateClientRequest(
    string Name,
    string? CompanyName,
    List<ClientContactDto>? Contacts);

public record UpdateClientRequest(
    string Name,
    string? CompanyName,
    List<ClientContactDto>? Contacts);

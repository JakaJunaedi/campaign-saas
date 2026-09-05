namespace CampaignSaaS.Modules.Client.Application.Commands.UpdateClient;

using CampaignSaaS.Modules.Client.Contracts.DTOs;
using CampaignSaaS.SharedKernel.Application;

public record UpdateClientCommand(
    Guid Id,
    string Name,
    string? CompanyName,
    List<ClientContactDto>? Contacts) : ICommand<ClientDto>;

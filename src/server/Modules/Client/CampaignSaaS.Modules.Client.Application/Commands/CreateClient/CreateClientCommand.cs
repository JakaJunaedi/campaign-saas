namespace CampaignSaaS.Modules.Client.Application.Commands.CreateClient;

using CampaignSaaS.Modules.Client.Contracts.DTOs;
using CampaignSaaS.SharedKernel.Application;

public record CreateClientCommand(
    string Name,
    string? CompanyName,
    List<ClientContactDto>? Contacts) : ICommand<ClientDto>;

namespace CampaignSaaS.Modules.Identity.Application.Commands.Login;

using CampaignSaaS.Modules.Identity.Contracts.DTOs;
using CampaignSaaS.SharedKernel.Application;

public record LoginCommand(string Email, string Password) : ICommand<AuthResultDto>;

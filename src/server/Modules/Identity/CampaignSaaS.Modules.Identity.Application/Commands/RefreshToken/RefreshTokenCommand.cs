namespace CampaignSaaS.Modules.Identity.Application.Commands.RefreshToken;

using CampaignSaaS.Modules.Identity.Contracts.DTOs;
using CampaignSaaS.SharedKernel.Application;

public record RefreshTokenCommand(string RefreshToken) : ICommand<AuthResultDto>;

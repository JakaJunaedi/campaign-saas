namespace CampaignSaaS.Modules.Identity.Application.Commands.CreateUser;

using CampaignSaaS.Modules.Identity.Contracts.DTOs;
using CampaignSaaS.SharedKernel.Application;

public record CreateUserCommand(
    string FullName,
    string Email,
    string Password,
    string Role) : ICommand<UserDto>;

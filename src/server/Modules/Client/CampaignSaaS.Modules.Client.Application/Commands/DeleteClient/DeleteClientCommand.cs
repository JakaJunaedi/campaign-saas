namespace CampaignSaaS.Modules.Client.Application.Commands.DeleteClient;

using CampaignSaaS.SharedKernel.Application;
using ErrorOr;
using MediatR;

public record DeleteClientCommand(Guid Id) : ICommand<Success>;

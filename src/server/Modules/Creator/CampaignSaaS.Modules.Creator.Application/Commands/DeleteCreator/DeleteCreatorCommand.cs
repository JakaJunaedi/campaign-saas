namespace CampaignSaaS.Modules.Creator.Application.Commands.DeleteCreator;

using CampaignSaaS.SharedKernel.Application;
using ErrorOr;
using MediatR;

public record DeleteCreatorCommand(Guid Id) : ICommand<Success>;

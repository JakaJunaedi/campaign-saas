namespace CampaignSaaS.Modules.Campaign.Application.Commands.DeleteCampaign;

using CampaignSaaS.SharedKernel.Application;
using ErrorOr;
using MediatR;

public record DeleteCampaignCommand(Guid Id) : ICommand<Success>;

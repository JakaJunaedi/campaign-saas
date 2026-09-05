namespace CampaignSaaS.Modules.Campaign.Application.Commands.RemoveCreatorFromRoster;

using CampaignSaaS.SharedKernel.Application;
using ErrorOr;
using MediatR;

public record RemoveCreatorFromRosterCommand(Guid CampaignId, Guid CreatorId) : ICommand<Success>;

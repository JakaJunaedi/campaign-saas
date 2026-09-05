namespace CampaignSaaS.SharedKernel.Application;

using ErrorOr;
using MediatR;

public interface ICommand<TResponse> : IRequest<ErrorOr<TResponse>>
{
}

public interface ICommand : IRequest<ErrorOr<Success>>
{
}

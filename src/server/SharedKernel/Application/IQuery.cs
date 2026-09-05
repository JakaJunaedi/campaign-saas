namespace CampaignSaaS.SharedKernel.Application;

using ErrorOr;
using MediatR;

public interface IQuery<TResponse> : IRequest<ErrorOr<TResponse>>
{
}

namespace CampaignSaaS.Modules.Identity.Application.Queries.GetOrganizationUsers;

using CampaignSaaS.Modules.Identity.Contracts.DTOs;
using CampaignSaaS.SharedKernel.Application;

public record GetOrganizationUsersQuery : IQuery<IReadOnlyList<UserDto>>;

namespace CampaignSaaS.Api.Controllers;

using CampaignSaaS.Api.Common;
using CampaignSaaS.Modules.Identity.Application.Commands.CreateUser;
using CampaignSaaS.Modules.Identity.Application.Queries.GetCurrentOrganization;
using CampaignSaaS.Modules.Identity.Application.Queries.GetOrganizationUsers;
using CampaignSaaS.Modules.Identity.Contracts.DTOs;
using CampaignSaaS.Modules.Identity.Contracts.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize]
[Route("api/v1/organizations")]
public class OrganizationController : ApiControllerBase
{
    [HttpGet("current")]
    [ProducesResponseType(typeof(ApiResponse<OrganizationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCurrentOrganization()
    {
        var query = new GetCurrentOrganizationQuery();
        var result = await Mediator.Send(query);
        return HandleResult(result);
    }

    [HttpGet("users")]
    [Authorize(Roles = "SuperAdmin,AgencyOwner,CampaignManager")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<UserDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetOrganizationUsers()
    {
        var query = new GetOrganizationUsersQuery();
        var result = await Mediator.Send(query);
        return HandleResult(result);
    }

    [HttpPost("users")]
    [Authorize(Roles = "SuperAdmin,AgencyOwner")]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
    {
        var command = new CreateUserCommand(
            request.FullName,
            request.Email,
            request.Password,
            request.Role);

        var result = await Mediator.Send(command);
        return HandleResult(result, StatusCodes.Status201Created);
    }
}

namespace CampaignSaaS.Api.Controllers;

using System.Security.Claims;
using CampaignSaaS.Api.Common;
using CampaignSaaS.Modules.Notification.Application.Commands.MarkAllNotificationsAsRead;
using CampaignSaaS.Modules.Notification.Application.Commands.MarkNotificationAsRead;
using CampaignSaaS.Modules.Notification.Application.Queries.GetNotifications;
using CampaignSaaS.Modules.Notification.Application.Queries.GetUnreadNotificationCount;
using CampaignSaaS.Modules.Notification.Contracts.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize]
[ApiController]
public class NotificationController : ApiControllerBase
{
    [HttpGet("api/v1/notifications")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<InAppNotificationDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetNotifications(
        [FromQuery] bool? unreadOnly,
        [FromQuery] int take = 50,
        CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();
        var query = new GetNotificationsQuery(userId, unreadOnly, take);
        var result = await Mediator.Send(query, cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("api/v1/notifications/unread-count")]
    [ProducesResponseType(typeof(ApiResponse<int>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetUnreadCount(CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();
        var query = new GetUnreadNotificationCountQuery(userId);
        var result = await Mediator.Send(query, cancellationToken);
        return HandleResult(result);
    }

    [HttpPatch("api/v1/notifications/{id:guid}/read")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MarkAsRead(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var command = new MarkNotificationAsReadCommand(id);
        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result, StatusCodes.Status204NoContent);
    }

    [HttpPost("api/v1/notifications/mark-all-read")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> MarkAllAsRead(CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();
        var command = new MarkAllNotificationsAsReadCommand(userId);
        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result, StatusCodes.Status204NoContent);
    }

    private Guid GetCurrentUserId()
    {
        var claimValue = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? User.FindFirst("sub")?.Value;

        if (Guid.TryParse(claimValue, out var parsedId))
        {
            return parsedId;
        }

        return Guid.Empty;
    }
}

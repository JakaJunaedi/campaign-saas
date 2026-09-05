namespace CampaignSaaS.Api.Common;

using System.Security.Claims;
using CampaignSaaS.SharedKernel.MultiTenancy;

public class CurrentTenantMiddleware
{
    private readonly RequestDelegate _next;

    public CurrentTenantMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ICurrentTenantContext tenantContext)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            // Extract organization ID claim
            var orgIdClaim = context.User.FindFirst("org_id")?.Value
                ?? context.User.FindFirst("organization_id")?.Value;

            // Extract user ID claim
            var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? context.User.FindFirst("sub")?.Value;

            // Extract role claim
            var roleClaim = context.User.FindFirst(ClaimTypes.Role)?.Value
                ?? context.User.FindFirst("role")?.Value;

            Guid? orgId = null;
            if (!string.IsNullOrEmpty(orgIdClaim) && Guid.TryParse(orgIdClaim, out var parsedOrgId))
            {
                orgId = parsedOrgId;
            }

            Guid? userId = null;
            if (!string.IsNullOrEmpty(userIdClaim) && Guid.TryParse(userIdClaim, out var parsedUserId))
            {
                userId = parsedUserId;
            }

            var isSuperAdmin = string.Equals(roleClaim, "SuperAdmin", StringComparison.OrdinalIgnoreCase);

            if (orgId.HasValue)
            {
                tenantContext.SetTenant(orgId.Value, userId, isSuperAdmin);
            }
        }

        await _next(context);
    }
}

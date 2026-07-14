using DavidGroup.Core.Identity.AuthorizationRequirements;
using DavidGroup.Core.Identity.Data;

using Microsoft.AspNetCore.Authorization;

namespace DavidGroup.Core.Identity.AuthorizationHandlers;

/// <summary>
/// Handles authorization for the <see cref="PermissionRequirement"/>.
/// </summary>
/// <remarks>
/// Authorization succeeds when the authenticated user possesses the required
/// permission in a <see cref="DavidGroupClaimTypes.Permission"/> claim.
/// </remarks>
public sealed class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    /// <inheritdoc />
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        HashSet<string> permissions = context.User.Claims
            .Where(r => r.Type == DavidGroupClaimTypes.Permission)
            .Select(r => r.Value)
            .ToHashSet();

        if (permissions.Any(permission => permission == requirement.Permission))
            context.Succeed(requirement);
        else
        {
            context.Fail(new AuthorizationFailureReason(this,
                $"You do not have permission {requirement.Permission} to access this resource"));
        }

        return Task.CompletedTask;
    }
}

using DavidGroup.Core.Identity.AuthorizationRequirements;
using DavidGroup.Core.Identity.Utilities;

using Microsoft.AspNetCore.Authorization;

namespace DavidGroup.Core.Identity.AuthorizationHandlers;

/// <summary>
/// Handles authorization for the <see cref="FreshSessionRequirement"/>.
/// </summary>
/// <remarks>
/// Authorization succeeds when the authenticated user's access token was issued
/// within the maximum age specified by the requirement. Otherwise, authorization
/// fails with a reason indicating that re-authentication is required.
/// </remarks>
public sealed class FreshSessionAuthorizationHandler : AuthorizationHandler<FreshSessionRequirement>
{
    /// <inheritdoc />
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
        FreshSessionRequirement requirement)
    {
        if (UserHelper.IsAuthenticatedWithin(context.User, TimeSpan.FromSeconds(requirement.TokenIssuanceAgeInSeconds)))
            context.Succeed(requirement);
        else
            context.Fail(new AuthorizationFailureReason(this, "Re-authentication is required."));

        return Task.CompletedTask;
    }
}

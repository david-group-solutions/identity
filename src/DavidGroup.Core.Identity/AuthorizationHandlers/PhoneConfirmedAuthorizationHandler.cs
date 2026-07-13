using DavidGroup.Core.Identity.AuthorizationRequirements;
using DavidGroup.Core.Identity.Data;
using DavidGroup.Core.Identity.Extensions;

using Microsoft.AspNetCore.Authorization;

namespace DavidGroup.Core.Identity.AuthorizationHandlers;

/// <summary>
/// Handles authorization for the <see cref="PhoneConfirmedRequirement"/>.
/// </summary>
/// <remarks>
/// Authorization succeeds when the authenticated user has the
/// <see cref="DavidGroupClaimTypes.PhoneConfirmed"/> claim and its value is
/// <see langword="true"/>.
/// </remarks>
public sealed class PhoneConfirmedAuthorizationHandler : AuthorizationHandler<PhoneConfirmedRequirement>
{
    /// <inheritdoc />
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
        PhoneConfirmedRequirement requirement)
    {
        if (context.User.TryGetClaim(DavidGroupClaimTypes.PhoneConfirmed, out bool confirmed) && confirmed)
            context.Succeed(requirement);

        return Task.CompletedTask;
    }
}

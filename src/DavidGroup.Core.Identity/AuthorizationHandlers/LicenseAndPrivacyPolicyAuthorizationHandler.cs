using System.Security.Claims;

using DavidGroup.Core.Identity.AuthorizationRequirements;
using DavidGroup.Core.Identity.Data;

using Microsoft.AspNetCore.Authorization;

namespace DavidGroup.Core.Identity.AuthorizationHandlers;

/// <summary>
/// Handles authorization checks for <see cref="LicenseAndPrivacyPolicyRequirement"/>.
/// </summary>
/// <remarks>
/// Validates that the current user has accepted the required license and privacy policy versions
/// by comparing the corresponding claims in the authenticated user's identity.
/// </remarks>
public sealed class LicenseAndPrivacyPolicyAuthorizationHandler : AuthorizationHandler<LicenseAndPrivacyPolicyRequirement>
{
    /// <inheritdoc />
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
        LicenseAndPrivacyPolicyRequirement requirement)
    {
        Claim? licenseClaim = context.User.FindFirst(DavidGroupClaimTypes.LicenceVersion);
        Claim? privacyPolicyClaim = context.User.FindFirst(DavidGroupClaimTypes.PrivacyPolicyVersion);

        if (licenseClaim is null || privacyPolicyClaim is null ||
            licenseClaim.Value != requirement.LicenceVersion ||
            privacyPolicyClaim.Value != requirement.PrivacyPolicyVersion)
        {
            context.Fail(new AuthorizationFailureReason(
                this,
                $"License {requirement.LicenceVersion} and Privacy Policy {requirement.PrivacyPolicyVersion} must be be accepted."
            ));
        }
        else
            context.Succeed(requirement);

        return Task.CompletedTask;
    }
}

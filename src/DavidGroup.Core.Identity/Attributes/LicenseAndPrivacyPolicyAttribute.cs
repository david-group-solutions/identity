using DavidGroup.Core.Identity.AuthorizationRequirements;

using Microsoft.AspNetCore.Authorization;

namespace DavidGroup.Core.Identity.Attributes;

/// <summary>
/// Specifies that the user must have accepted the required license and privacy policy versions
/// before accessing the decorated controller or action.
/// </summary>
/// <remarks>
/// This attribute creates an authorization policy based on the provided license and privacy policy
/// versions. The policy is handled by <see cref="LicenseAndPrivacyPolicyRequirement"/>.
/// </remarks>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public sealed class LicenseAndPrivacyPolicyAttribute : AuthorizeAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LicenseAndPrivacyPolicyAttribute"/> class.
    /// </summary>
    /// <param name="licenceVersion">
    /// The required license version that the user must have accepted.
    /// </param>
    /// <param name="privacyPolicyVersion">
    /// The required privacy policy version that the user must have accepted.
    /// </param>
    public LicenseAndPrivacyPolicyAttribute(string licenceVersion, string privacyPolicyVersion)
    {
        Policy = $"{LicenseAndPrivacyPolicyRequirement.Prefix}{licenceVersion}:{privacyPolicyVersion}";
    }
}

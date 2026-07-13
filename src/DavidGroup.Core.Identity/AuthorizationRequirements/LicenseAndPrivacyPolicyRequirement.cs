using Microsoft.AspNetCore.Authorization;

namespace DavidGroup.Core.Identity.AuthorizationRequirements;

/// <summary>
/// Represents an authorization requirement that ensures a user has accepted
/// the required license and privacy policy versions.
/// </summary>
/// <param name="licenceVersion">
/// The required license version that the user must have accepted.
/// </param>
/// <param name="privacyPolicyVersion">
/// The required privacy policy version that the user must have accepted.
/// </param>
public class LicenseAndPrivacyPolicyRequirement(string licenceVersion, string privacyPolicyVersion) : IAuthorizationRequirement
{
    /// <summary>
    /// The prefix used when generating dynamic authorization policy names.
    /// </summary>
    public const string Prefix = "LicenseAndPrivacyPolicy:";

    /// <summary>
    /// Gets the required license version.
    /// </summary>
    public string LicenceVersion { get; } = licenceVersion;

    /// <summary>
    /// Gets the required privacy policy version.
    /// </summary>
    public string PrivacyPolicyVersion { get; } = privacyPolicyVersion;
}

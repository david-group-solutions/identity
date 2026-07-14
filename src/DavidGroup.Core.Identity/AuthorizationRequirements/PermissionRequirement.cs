using Microsoft.AspNetCore.Authorization;

namespace DavidGroup.Core.Identity.AuthorizationRequirements;

/// <summary>
/// Represents an authorization requirement that ensures the authenticated user
/// has a specific permission.
/// </summary>
/// <param name="permission">The required permission.</param>
public class PermissionRequirement(string permission) : IAuthorizationRequirement
{
    /// <summary>
    /// The prefix used to identify permission-based authorization policies.
    /// </summary>
    public const string Prefix = "Permission:";

    /// <summary>
    /// Gets the permission required to satisfy this authorization requirement.
    /// </summary>
    public string Permission { get; } = permission;
}

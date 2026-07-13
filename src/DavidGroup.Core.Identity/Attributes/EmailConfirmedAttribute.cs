using DavidGroup.Core.Identity.AuthorizationRequirements;

using Microsoft.AspNetCore.Authorization;

namespace DavidGroup.Core.Identity.Attributes;

/// <summary>
/// Requires the authenticated user to have a confirmed email address.
/// </summary>
/// <remarks>
/// This attribute applies the <see cref="EmailConfirmedRequirement"/> authorization
/// policy to the decorated controller or action.
/// </remarks>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public sealed class EmailConfirmedAttribute() : AuthorizeAttribute(policy: EmailConfirmedRequirement.PolicyName);

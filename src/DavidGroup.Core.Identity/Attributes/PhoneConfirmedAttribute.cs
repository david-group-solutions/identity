using DavidGroup.Core.Identity.AuthorizationRequirements;

using Microsoft.AspNetCore.Authorization;

namespace DavidGroup.Core.Identity.Attributes;

/// <summary>
/// Requires the authenticated user to have a confirmed phone number.
/// </summary>
/// <remarks>
/// This attribute applies the <see cref="PhoneConfirmedRequirement"/> authorization
/// policy to the decorated controller or action.
/// </remarks>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public sealed class PhoneConfirmedAttribute() : AuthorizeAttribute(policy: PhoneConfirmedRequirement.PolicyName);

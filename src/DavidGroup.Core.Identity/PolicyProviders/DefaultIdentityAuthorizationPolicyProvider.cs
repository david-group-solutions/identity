using DavidGroup.Core.Identity.AuthorizationRequirements;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace DavidGroup.Core.Identity.PolicyProviders;

/// <summary>
/// Provides authorization policies for the Identity module.
/// </summary>
public class DefaultIdentityAuthorizationPolicyProvider(IOptions<AuthorizationOptions> options)
    : DefaultAuthorizationPolicyProvider(options)
{
    /// <inheritdoc />
    public override async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        AuthorizationPolicy? policy = await base.GetPolicyAsync(policyName);
        if (policy is not null)
            return policy;

        switch (policyName)
        {
            case EmailConfirmedRequirement.PolicyName:
                return Build(new EmailConfirmedRequirement());
            case PhoneConfirmedRequirement.PolicyName:
                return Build(new PhoneConfirmedRequirement());
        }

        if (policyName.StartsWith(FreshSessionRequirement.Prefix))
        {
            string secondsStr = policyName[FreshSessionRequirement.Prefix.Length..];
            uint seconds = uint.Parse(secondsStr);

            return Build(new FreshSessionRequirement(seconds));
        }

        if (policyName.StartsWith(PermissionRequirement.Prefix))
        {
            IEnumerable<PermissionRequirement> requirements = policyName[PermissionRequirement.Prefix.Length..]
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(p => p.Trim())
                .Distinct()
                .Select(p => new PermissionRequirement(p));

            return Build([..requirements]);
        }

        if (policyName.StartsWith(LicenseAndPrivacyPolicyRequirement.Prefix))
        {
            string[] parts = policyName[LicenseAndPrivacyPolicyRequirement.Prefix.Length..]
                .Split(':', StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length != 2)
                return null;

            return Build(new LicenseAndPrivacyPolicyRequirement(parts[0], parts[1]));
        }

        return null;

        static AuthorizationPolicy Build(params IAuthorizationRequirement[] requirements) =>
            new AuthorizationPolicyBuilder()
                .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                .AddRequirements(requirements)
                .Build();
    }
}

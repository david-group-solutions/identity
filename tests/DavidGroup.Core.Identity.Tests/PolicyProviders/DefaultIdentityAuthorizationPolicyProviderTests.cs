using DavidGroup.Core.Identity.AuthorizationRequirements;
using DavidGroup.Core.Identity.PolicyProviders;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.Extensions.Options;

namespace DavidGroup.Core.Identity.Tests.PolicyProviders;

/// <summary>
/// Unit tests for <see cref="DefaultIdentityAuthorizationPolicyProvider"/>.
/// </summary>
public static class DefaultIdentityAuthorizationPolicyProviderTests
{
    private static DefaultIdentityAuthorizationPolicyProvider CreateProvider(Action<AuthorizationOptions>? configure = null)
    {
        AuthorizationOptions authorizationOptions = new();
        configure?.Invoke(authorizationOptions);

        IOptions<AuthorizationOptions> options = Microsoft.Extensions.Options.Options.Create(authorizationOptions);

        return new DefaultIdentityAuthorizationPolicyProvider(options);
    }

    /// <summary>
    /// Tests for <see cref="DefaultIdentityAuthorizationPolicyProvider.GetPolicyAsync(string)"/>.
    /// </summary>
    public class GetPolicyAsyncTests
    {
        [Fact]
        public async Task GetPolicyAsync_PolicyRegisteredInOptions_ReturnsRegisteredPolicy()
        {
            // Arrange
            const string policyName = "CustomPolicy";
            AuthorizationPolicy registeredPolicy = new AuthorizationPolicyBuilder()
                .RequireAssertion(_ => true)
                .Build();
            DefaultIdentityAuthorizationPolicyProvider provider = CreateProvider(options =>
                options.AddPolicy(policyName, registeredPolicy));

            // Act
            AuthorizationPolicy? result = await provider.GetPolicyAsync(policyName);

            // Assert
            Assert.Same(registeredPolicy, result);
        }

        [Fact]
        public async Task GetPolicyAsync_PolicyNameMatchesSpecialCaseButAlreadyRegistered_ReturnsRegisteredPolicyInstead()
        {
            // Arrange
            DefaultIdentityAuthorizationPolicyProvider provider = CreateProvider(options =>
                options.AddPolicy(EmailConfirmedRequirement.PolicyName, policy => policy.RequireAssertion(_ => true)));

            // Act
            AuthorizationPolicy? result = await provider.GetPolicyAsync(EmailConfirmedRequirement.PolicyName);

            // Assert
            Assert.NotNull(result);
            IAuthorizationRequirement requirement = Assert.Single(result.Requirements);
            Assert.IsType<AssertionRequirement>(requirement);
        }

        [Fact]
        public async Task GetPolicyAsync_UnknownPolicyName_ReturnsNull()
        {
            // Arrange
            DefaultIdentityAuthorizationPolicyProvider provider = CreateProvider();

            // Act
            AuthorizationPolicy? result = await provider.GetPolicyAsync("NonExistentPolicy");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetPolicyAsync_EmailConfirmedPolicyName_ReturnsPolicyWithEmailConfirmedRequirement()
        {
            // Arrange
            DefaultIdentityAuthorizationPolicyProvider provider = CreateProvider();

            // Act
            AuthorizationPolicy? result = await provider.GetPolicyAsync(EmailConfirmedRequirement.PolicyName);

            // Assert
            Assert.NotNull(result);
            Assert.Contains(JwtBearerDefaults.AuthenticationScheme, result.AuthenticationSchemes);
            IAuthorizationRequirement requirement = Assert.Single(result.Requirements);
            Assert.IsType<EmailConfirmedRequirement>(requirement);
        }

        [Fact]
        public async Task GetPolicyAsync_PhoneConfirmedPolicyName_ReturnsPolicyWithPhoneConfirmedRequirement()
        {
            // Arrange
            DefaultIdentityAuthorizationPolicyProvider provider = CreateProvider();

            // Act
            AuthorizationPolicy? result = await provider.GetPolicyAsync(PhoneConfirmedRequirement.PolicyName);

            // Assert
            Assert.NotNull(result);
            Assert.Contains(JwtBearerDefaults.AuthenticationScheme, result.AuthenticationSchemes);
            IAuthorizationRequirement requirement = Assert.Single(result.Requirements);
            Assert.IsType<PhoneConfirmedRequirement>(requirement);
        }

        [Fact]
        public async Task GetPolicyAsync_FreshSessionPolicyNameWithValidSeconds_ReturnsPolicyWithFreshSessionRequirement()
        {
            // Arrange
            DefaultIdentityAuthorizationPolicyProvider provider = CreateProvider();
            const string policyName = $"{FreshSessionRequirement.Prefix}300";

            // Act
            AuthorizationPolicy? result = await provider.GetPolicyAsync(policyName);

            // Assert
            Assert.NotNull(result);
            Assert.Contains(JwtBearerDefaults.AuthenticationScheme, result.AuthenticationSchemes);
            IAuthorizationRequirement requirement = Assert.Single(result.Requirements);
            Assert.IsType<FreshSessionRequirement>(requirement);
        }

        [Fact]
        public async Task GetPolicyAsync_FreshSessionPolicyNameWithInvalidSeconds_ThrowsFormatException()
        {
            // Arrange
            DefaultIdentityAuthorizationPolicyProvider provider = CreateProvider();
            const string policyName = $"{FreshSessionRequirement.Prefix}not-a-number";

            // Act
            Func<Task> act = () => provider.GetPolicyAsync(policyName);

            // Assert
            await Assert.ThrowsAsync<FormatException>(act);
        }

        [Fact]
        public async Task GetPolicyAsync_PermissionPolicyNameWithSinglePermission_ReturnsPolicyWithOneRequirement()
        {
            // Arrange
            DefaultIdentityAuthorizationPolicyProvider provider = CreateProvider();
            const string policyName = $"{PermissionRequirement.Prefix}Users.Read";

            // Act
            AuthorizationPolicy? result = await provider.GetPolicyAsync(policyName);

            // Assert
            Assert.NotNull(result);
            Assert.Contains(JwtBearerDefaults.AuthenticationScheme, result.AuthenticationSchemes);
            IAuthorizationRequirement requirement = Assert.Single(result.Requirements);
            Assert.IsType<PermissionRequirement>(requirement);
        }

        [Fact]
        public async Task GetPolicyAsync_PermissionPolicyNameWithMultiplePermissions_ReturnsPolicyWithMultipleRequirements()
        {
            // Arrange
            DefaultIdentityAuthorizationPolicyProvider provider = CreateProvider();
            const string policyName = $"{PermissionRequirement.Prefix}Users.Read,Users.Write,Users.Update";

            // Act
            AuthorizationPolicy? result = await provider.GetPolicyAsync(policyName);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Requirements.Count);
            Assert.All(result.Requirements, requirement => Assert.IsType<PermissionRequirement>(requirement));
        }

        [Fact]
        public async Task GetPolicyAsync_PermissionPolicyNameWithEmptyEntries_IgnoresEmptyEntries()
        {
            // Arrange
            DefaultIdentityAuthorizationPolicyProvider provider = CreateProvider();
            string policyName = $"{PermissionRequirement.Prefix}Users.Read,,Users.Write,";

            // Act
            AuthorizationPolicy? result = await provider.GetPolicyAsync(policyName);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Requirements.Count);
            Assert.All(result.Requirements, requirement => Assert.IsType<PermissionRequirement>(requirement));
        }

        [Fact]
        public async Task GetPolicyAsync_PermissionPolicyNameWithWithDuplicateEntries_IgnoresDuplicateEntries()
        {
            // Arrange
            DefaultIdentityAuthorizationPolicyProvider provider = CreateProvider();
            const string policyName = $"{PermissionRequirement.Prefix}Users.Read,Users.Read,Users.Write";

            // Act
            AuthorizationPolicy? result = await provider.GetPolicyAsync(policyName);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Requirements.Count);
            Assert.All(result.Requirements, requirement => Assert.IsType<PermissionRequirement>(requirement));
        }

        [Fact]
        public async Task GetPolicyAsync_LicenseAndPrivacyPolicyNameWithTwoParts_ReturnsPolicyWithRequirement()
        {
            // Arrange
            DefaultIdentityAuthorizationPolicyProvider provider = CreateProvider();
            const string policyName = $"{LicenseAndPrivacyPolicyRequirement.Prefix}v1:v2";

            // Act
            AuthorizationPolicy? result = await provider.GetPolicyAsync(policyName);

            // Assert
            Assert.NotNull(result);
            Assert.Contains(JwtBearerDefaults.AuthenticationScheme, result.AuthenticationSchemes);
            IAuthorizationRequirement requirement = Assert.Single(result.Requirements);
            Assert.IsType<LicenseAndPrivacyPolicyRequirement>(requirement);
        }

        [Fact]
        public async Task GetPolicyAsync_LicenseAndPrivacyPolicyNameWithOnePart_ReturnsNull()
        {
            // Arrange
            DefaultIdentityAuthorizationPolicyProvider provider = CreateProvider();
            const string policyName = $"{LicenseAndPrivacyPolicyRequirement.Prefix}onlyonepart";

            // Act
            AuthorizationPolicy? result = await provider.GetPolicyAsync(policyName);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetPolicyAsync_LicenseAndPrivacyPolicyNameWithThreeParts_ReturnsNull()
        {
            // Arrange
            DefaultIdentityAuthorizationPolicyProvider provider = CreateProvider();
            const string policyName = $"{LicenseAndPrivacyPolicyRequirement.Prefix}a:b:c";

            // Act
            AuthorizationPolicy? result = await provider.GetPolicyAsync(policyName);

            // Assert
            Assert.Null(result);
        }
    }
}

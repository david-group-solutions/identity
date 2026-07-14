using System.Security.Claims;

using DavidGroup.Core.Identity.AuthorizationHandlers;
using DavidGroup.Core.Identity.AuthorizationRequirements;
using DavidGroup.Core.Identity.Data;

using Microsoft.AspNetCore.Authorization;

namespace DavidGroup.Core.Identity.Tests.AuthorizationHandlers;

/// <summary>
/// Unit tests for <see cref="LicenseAndPrivacyPolicyAuthorizationHandler"/>.
/// </summary>
public static class LicenseAndPrivacyPolicyAuthorizationHandlerTests
{
    private static ClaimsPrincipal CreateUser(string? licenceVersion, string? privacyPolicyVersion)
    {
        List<Claim> claims = [];

        if (licenceVersion is not null)
            claims.Add(new Claim(DavidGroupClaimTypes.LicenceVersion, licenceVersion));

        if (privacyPolicyVersion is not null)
            claims.Add(new Claim(DavidGroupClaimTypes.PrivacyPolicyVersion, privacyPolicyVersion));

        ClaimsIdentity identity = new(claims, authenticationType: "TestAuth");

        return new ClaimsPrincipal(identity);
    }

    private static AuthorizationHandlerContext CreateContext(ClaimsPrincipal user, LicenseAndPrivacyPolicyRequirement requirement)
    {
        return new AuthorizationHandlerContext([requirement], user, resource: null);
    }

    /// <summary>
    /// Tests for the protected <c>HandleRequirementAsync</c> override, exercised
    /// through the public <see cref="IAuthorizationHandler.HandleAsync"/> entry point
    /// inherited from <see cref="AuthorizationHandler{TRequirement}"/>.
    /// </summary>
    public class HandleRequirementAsyncTests
    {
        [Fact]
        public async Task HandleRequirementAsync_ClaimsMatchRequirement_Succeeds()
        {
            // Arrange
            LicenseAndPrivacyPolicyAuthorizationHandler handler = new();
            LicenseAndPrivacyPolicyRequirement requirement = new("v1", "v2");
            ClaimsPrincipal user = CreateUser(licenceVersion: "v1", privacyPolicyVersion: "v2");
            AuthorizationHandlerContext context = CreateContext(user, requirement);

            // Act
            await handler.HandleAsync(context);

            // Assert
            Assert.True(context.HasSucceeded);
            Assert.False(context.HasFailed);
        }

        [Fact]
        public async Task HandleRequirementAsync_LicenceClaimIsMissing_Fails()
        {
            // Arrange
            LicenseAndPrivacyPolicyAuthorizationHandler handler = new();
            LicenseAndPrivacyPolicyRequirement requirement = new("v1", "v2");
            ClaimsPrincipal user = CreateUser(licenceVersion: null, privacyPolicyVersion: "v2");
            AuthorizationHandlerContext context = CreateContext(user, requirement);

            // Act
            await handler.HandleAsync(context);

            // Assert
            Assert.False(context.HasSucceeded);
            Assert.True(context.HasFailed);
        }

        [Fact]
        public async Task HandleRequirementAsync_PrivacyPolicyClaimIsMissing_Fails()
        {
            // Arrange
            LicenseAndPrivacyPolicyAuthorizationHandler handler = new();
            LicenseAndPrivacyPolicyRequirement requirement = new("v1", "v2");
            ClaimsPrincipal user = CreateUser(licenceVersion: "v1", privacyPolicyVersion: null);
            AuthorizationHandlerContext context = CreateContext(user, requirement);

            // Act
            await handler.HandleAsync(context);

            // Assert
            Assert.False(context.HasSucceeded);
            Assert.True(context.HasFailed);
        }

        [Fact]
        public async Task HandleRequirementAsync_BothClaimsAreMissing_Fails()
        {
            // Arrange
            LicenseAndPrivacyPolicyAuthorizationHandler handler = new();
            LicenseAndPrivacyPolicyRequirement requirement = new("v1", "v2");
            ClaimsPrincipal user = CreateUser(licenceVersion: null, privacyPolicyVersion: null);
            AuthorizationHandlerContext context = CreateContext(user, requirement);

            // Act
            await handler.HandleAsync(context);

            // Assert
            Assert.False(context.HasSucceeded);
            Assert.True(context.HasFailed);
        }

        [Fact]
        public async Task HandleRequirementAsync_LicenceClaimValueDoesNotMatch_Fails()
        {
            // Arrange
            LicenseAndPrivacyPolicyAuthorizationHandler handler = new();
            LicenseAndPrivacyPolicyRequirement requirement = new("v1", "v2");
            ClaimsPrincipal user = CreateUser(licenceVersion: "v0", privacyPolicyVersion: "v2");
            AuthorizationHandlerContext context = CreateContext(user, requirement);

            // Act
            await handler.HandleAsync(context);

            // Assert
            Assert.False(context.HasSucceeded);
            Assert.True(context.HasFailed);
        }

        [Fact]
        public async Task HandleRequirementAsync_PrivacyPolicyClaimValueDoesNotMatch_Fails()
        {
            // Arrange
            LicenseAndPrivacyPolicyAuthorizationHandler handler = new();
            LicenseAndPrivacyPolicyRequirement requirement = new("v1", "v2");
            ClaimsPrincipal user = CreateUser(licenceVersion: "v1", privacyPolicyVersion: "v0");
            AuthorizationHandlerContext context = CreateContext(user, requirement);

            // Act
            await handler.HandleAsync(context);

            // Assert
            Assert.False(context.HasSucceeded);
            Assert.True(context.HasFailed);
        }

        [Fact]
        public async Task HandleRequirementAsync_ClaimsDoNotMatch_FailureReasonContainsHandlerAndMessage()
        {
            // Arrange
            LicenseAndPrivacyPolicyAuthorizationHandler handler = new();
            LicenseAndPrivacyPolicyRequirement requirement = new("v1", "v2");
            ClaimsPrincipal user = CreateUser(licenceVersion: null, privacyPolicyVersion: null);
            AuthorizationHandlerContext context = CreateContext(user, requirement);

            // Act
            await handler.HandleAsync(context);

            // Assert
            AuthorizationFailureReason failureReason = Assert.Single(context.FailureReasons);
            Assert.Same(handler, failureReason.Handler);
            Assert.Equal("License v1 and Privacy Policy v2 must be be accepted.", failureReason.Message);
        }
    }
}

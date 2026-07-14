using System.Security.Claims;

using DavidGroup.Core.Identity.AuthorizationHandlers;
using DavidGroup.Core.Identity.AuthorizationRequirements;
using DavidGroup.Core.Identity.Data;

using Microsoft.AspNetCore.Authorization;

namespace DavidGroup.Core.Identity.Tests.AuthorizationHandlers;

/// <summary>
/// Unit tests for <see cref="FreshSessionAuthorizationHandler"/>.
/// </summary>
public static class FreshSessionAuthorizationHandlerTests
{
    private static ClaimsPrincipal CreateUserWithAuthTime(DateTimeOffset authTime)
    {
        Claim claim = new(DavidGroupClaimTypes.AuthTime, authTime.ToUnixTimeSeconds().ToString());

        ClaimsIdentity identity = new([claim], authenticationType: "TestAuth");

        return new ClaimsPrincipal(identity);
    }

    private static AuthorizationHandlerContext CreateContext(ClaimsPrincipal user, FreshSessionRequirement requirement)
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
        public async Task HandleRequirementAsync_TokenIssuedWithinAllowedAge_Succeeds()
        {
            // Arrange
            FreshSessionAuthorizationHandler handler = new();
            FreshSessionRequirement requirement = new(300);
            DateTimeOffset authTime = DateTimeOffset.UtcNow.AddSeconds(-30);
            ClaimsPrincipal user = CreateUserWithAuthTime(authTime);
            AuthorizationHandlerContext context = CreateContext(user, requirement);

            // Act
            await handler.HandleAsync(context);

            // Assert
            Assert.True(context.HasSucceeded);
            Assert.False(context.HasFailed);
        }

        [Fact]
        public async Task HandleRequirementAsync_TokenIssuedOutsideAllowedAge_FailsWithReAuthenticationReason()
        {
            // Arrange
            FreshSessionAuthorizationHandler handler = new();
            FreshSessionRequirement requirement = new(60);
            DateTimeOffset authTime = DateTimeOffset.UtcNow.AddSeconds(-300);
            ClaimsPrincipal user = CreateUserWithAuthTime(authTime);
            AuthorizationHandlerContext context = CreateContext(user, requirement);

            // Act
            await handler.HandleAsync(context);

            // Assert
            Assert.False(context.HasSucceeded);
            Assert.True(context.HasFailed);
            AuthorizationFailureReason failureReason = Assert.Single(context.FailureReasons);
            Assert.Same(handler, failureReason.Handler);
            Assert.Equal("Re-authentication is required.", failureReason.Message);
        }
    }
}

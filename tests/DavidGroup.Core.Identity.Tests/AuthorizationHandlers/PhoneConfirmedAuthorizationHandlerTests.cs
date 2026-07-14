using System.Security.Claims;

using DavidGroup.Core.Identity.AuthorizationHandlers;
using DavidGroup.Core.Identity.AuthorizationRequirements;
using DavidGroup.Core.Identity.Data;

using Microsoft.AspNetCore.Authorization;

namespace DavidGroup.Core.Identity.Tests.AuthorizationHandlers;

/// <summary>
/// Unit tests for <see cref="PhoneConfirmedAuthorizationHandler"/>.
/// </summary>
public static class PhoneConfirmedAuthorizationHandlerTests
{
    private static ClaimsPrincipal CreateUser(string? phoneConfirmedClaimValue)
    {
        List<Claim> claims = [];

        if (phoneConfirmedClaimValue is not null)
            claims.Add(new Claim(DavidGroupClaimTypes.PhoneConfirmed, phoneConfirmedClaimValue));

        ClaimsIdentity identity = new(claims, authenticationType: "TestAuth");

        return new ClaimsPrincipal(identity);
    }

    private static AuthorizationHandlerContext CreateContext(ClaimsPrincipal user, PhoneConfirmedRequirement requirement)
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
        public async Task HandleRequirementAsync_PhoneConfirmedClaimIsTrue_Succeeds()
        {
            // Arrange
            PhoneConfirmedAuthorizationHandler handler = new();
            PhoneConfirmedRequirement requirement = new();
            ClaimsPrincipal user = CreateUser(bool.TrueString);
            AuthorizationHandlerContext context = CreateContext(user, requirement);

            // Act
            await handler.HandleAsync(context);

            // Assert
            Assert.True(context.HasSucceeded);
            Assert.False(context.HasFailed);
        }

        [Fact]
        public async Task HandleRequirementAsync_PhoneConfirmedClaimIsFalse_DoesNotSucceed()
        {
            // Arrange
            PhoneConfirmedAuthorizationHandler handler = new();
            PhoneConfirmedRequirement requirement = new();
            ClaimsPrincipal user = CreateUser(bool.FalseString);
            AuthorizationHandlerContext context = CreateContext(user, requirement);

            // Act
            await handler.HandleAsync(context);

            // Assert
            Assert.False(context.HasSucceeded);
            Assert.True(context.HasFailed);
        }

        [Fact]
        public async Task HandleRequirementAsync_PhoneConfirmedClaimIsMissing_DoesNotSucceed()
        {
            // Arrange
            PhoneConfirmedAuthorizationHandler handler = new();
            PhoneConfirmedRequirement requirement = new();
            ClaimsPrincipal user = CreateUser(phoneConfirmedClaimValue: null);
            AuthorizationHandlerContext context = CreateContext(user, requirement);

            // Act
            await handler.HandleAsync(context);

            // Assert
            Assert.False(context.HasSucceeded);
            Assert.True(context.HasFailed);
        }

        [Fact]
        public async Task HandleRequirementAsync_PhoneConfirmedClaimHasInvalidFormat_DoesNotSucceed()
        {
            // Arrange
            PhoneConfirmedAuthorizationHandler handler = new();
            PhoneConfirmedRequirement requirement = new();
            ClaimsPrincipal user = CreateUser("not-a-boolean");
            AuthorizationHandlerContext context = CreateContext(user, requirement);

            // Act
            await handler.HandleAsync(context);

            // Assert
            Assert.False(context.HasSucceeded);
            Assert.True(context.HasFailed);
        }
    }
}

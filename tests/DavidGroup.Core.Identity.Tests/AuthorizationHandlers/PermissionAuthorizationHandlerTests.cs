using System.Security.Claims;

using DavidGroup.Core.Identity.AuthorizationHandlers;
using DavidGroup.Core.Identity.AuthorizationRequirements;
using DavidGroup.Core.Identity.Data;

using Microsoft.AspNetCore.Authorization;

namespace DavidGroup.Core.Identity.Tests.AuthorizationHandlers;

/// <summary>
/// Unit tests for <see cref="PermissionAuthorizationHandler"/>.
/// </summary>
public static class PermissionAuthorizationHandlerTests
{
    private static ClaimsPrincipal CreateUserWithPermissions(params string[] permissions)
    {
        List<Claim> claims = permissions
            .Select(permission => new Claim(DavidGroupClaimTypes.Permission, permission))
            .ToList();

        ClaimsIdentity identity = new(claims, authenticationType: "TestAuth");

        return new ClaimsPrincipal(identity);
    }

    private static AuthorizationHandlerContext CreateContext(ClaimsPrincipal user, PermissionRequirement requirement)
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
        public async Task HandleRequirementAsync_UserHasRequiredPermission_Succeeds()
        {
            // Arrange
            PermissionAuthorizationHandler handler = new();
            PermissionRequirement requirement = new("Users.Read");
            ClaimsPrincipal user = CreateUserWithPermissions("Users.Read");
            AuthorizationHandlerContext context = CreateContext(user, requirement);

            // Act
            await handler.HandleAsync(context);

            // Assert
            Assert.True(context.HasSucceeded);
        }

        [Fact]
        public async Task HandleRequirementAsync_UserDoesNotHaveRequiredPermission_DoesNotSucceed()
        {
            // Arrange
            PermissionAuthorizationHandler handler = new();
            PermissionRequirement requirement = new("Users.Write");
            ClaimsPrincipal user = CreateUserWithPermissions("Users.Read");
            AuthorizationHandlerContext context = CreateContext(user, requirement);

            // Act
            await handler.HandleAsync(context);

            // Assert
            Assert.False(context.HasSucceeded);
        }

        [Fact]
        public async Task HandleRequirementAsync_UserHasNoPermissionClaims_DoesNotSucceed()
        {
            // Arrange
            PermissionAuthorizationHandler handler = new();
            PermissionRequirement requirement = new("Users.Read");
            ClaimsPrincipal user = CreateUserWithPermissions();
            AuthorizationHandlerContext context = CreateContext(user, requirement);

            // Act
            await handler.HandleAsync(context);

            // Assert
            Assert.False(context.HasSucceeded);
        }

        [Fact]
        public async Task HandleRequirementAsync_UserHasMultiplePermissionsIncludingRequired_Succeeds()
        {
            // Arrange
            PermissionAuthorizationHandler handler = new();
            PermissionRequirement requirement = new("Users.Read");
            ClaimsPrincipal user = CreateUserWithPermissions("Users.Write", "Users.Read", "Users.Delete");
            AuthorizationHandlerContext context = CreateContext(user, requirement);

            // Act
            await handler.HandleAsync(context);

            // Assert
            Assert.True(context.HasSucceeded);
        }

        [Fact]
        public async Task HandleRequirementAsync_UserHasPermissionWithDifferentCasing_DoesNotSucceed()
        {
            // Arrange
            PermissionAuthorizationHandler handler = new();
            PermissionRequirement requirement = new("Users.Read");
            ClaimsPrincipal user = CreateUserWithPermissions("USERS.READ");
            AuthorizationHandlerContext context = CreateContext(user, requirement);

            // Act
            await handler.HandleAsync(context);

            // Assert
            Assert.False(context.HasSucceeded);
        }
    }
}

using System.Security.Claims;

using DavidGroup.Core.Identity.Controllers;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DavidGroup.Core.Identity.Tests.Controllers;

/// <summary>
/// Unit tests for <see cref="IdentityController"/>.
/// </summary>
public static class IdentityControllerTests
{
    private const string ClaimType = "custom:claim";

    /// <summary>
    /// Test-only subclass that exposes the protected helper methods of
    /// <see cref="IdentityController"/> as public members so they can be
    /// invoked directly from tests.
    /// </summary>
    private class TestableIdentityController : IdentityController
    {
        public new string GetRequiredClaim(string claimType)
            => base.GetRequiredClaim(claimType);

        public new T GetRequiredClaim<T>(string claimType) where T : IParsable<T>
            => base.GetRequiredClaim<T>(claimType);

        public new bool TryGetClaim<T>(string claimType, out T? value) where T : IParsable<T>
            => base.TryGetClaim(claimType, out value);
    }

    private static TestableIdentityController CreateController(ClaimsPrincipal user)
    {
        DefaultHttpContext httpContext = new()
        {
            User = user
        };

        return new TestableIdentityController
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            }
        };
    }

    private static ClaimsPrincipal CreatePrincipalWithClaim(string claimType, string claimValue)
    {
        Claim claim = new(claimType, claimValue);
        ClaimsIdentity identity = new([claim]);
        return new ClaimsPrincipal(identity);
    }

    private static ClaimsPrincipal CreatePrincipalWithoutClaims()
    {
        ClaimsIdentity identity = new();
        return new ClaimsPrincipal(identity);
    }

    /// <summary>
    /// Tests for <see cref="IdentityController.GetRequiredClaim(string)"/>.
    /// </summary>
    public class GetRequiredClaimStringTests
    {
        [Fact]
        public void GetRequiredClaim_ClaimExists_ReturnsClaimValue()
        {
            // Arrange
            const string expectedValue = "user-123";
            ClaimsPrincipal user = CreatePrincipalWithClaim(ClaimType, expectedValue);
            TestableIdentityController controller = CreateController(user);

            // Act
            string actualValue = controller.GetRequiredClaim(ClaimType);

            // Assert
            Assert.Equal(expectedValue, actualValue);
        }

        [Fact]
        public void GetRequiredClaim_ClaimDoesNotExist_ThrowsInvalidOperationException()
        {
            // Arrange
            ClaimsPrincipal user = CreatePrincipalWithoutClaims();
            TestableIdentityController controller = CreateController(user);

            // Act
            Action act = () => controller.GetRequiredClaim(ClaimType);

            // Assert
            InvalidOperationException exception = Assert.Throws<InvalidOperationException>(act);
            Assert.Equal($"Claim '{ClaimType}' was not found.", exception.Message);
        }
    }

    /// <summary>
    /// Tests for <see cref="IdentityController.GetRequiredClaim{T}(string)"/>.
    /// </summary>
    public class GetRequiredClaimGenericTests
    {
        [Fact]
        public void GetRequiredClaim_ClaimExistsAndIsValidInt_ReturnsParsedValue()
        {
            // Arrange
            const int expectedValue = 42;
            ClaimsPrincipal user = CreatePrincipalWithClaim(ClaimType, expectedValue.ToString());
            TestableIdentityController controller = CreateController(user);

            // Act
            int actualValue = controller.GetRequiredClaim<int>(ClaimType);

            // Assert
            Assert.Equal(expectedValue, actualValue);
        }

        [Fact]
        public void GetRequiredClaim_ClaimDoesNotExist_ThrowsInvalidOperationException()
        {
            // Arrange
            ClaimsPrincipal user = CreatePrincipalWithoutClaims();
            TestableIdentityController controller = CreateController(user);

            // Act
            Action act = () => controller.GetRequiredClaim<int>(ClaimType);

            // Assert
            InvalidOperationException exception = Assert.Throws<InvalidOperationException>(act);
            Assert.Equal($"Claim '{ClaimType}' was not found.", exception.Message);
        }

        [Fact]
        public void GetRequiredClaim_ClaimValueIsInvalidFormat_ThrowsFormatException()
        {
            // Arrange
            ClaimsPrincipal user = CreatePrincipalWithClaim(ClaimType, "not-a-number");
            TestableIdentityController controller = CreateController(user);

            // Act
            Action act = () => controller.GetRequiredClaim<int>(ClaimType);

            // Assert
            Assert.Throws<FormatException>(act);
        }
    }

    /// <summary>
    /// Tests for <see cref="IdentityController.TryGetClaim{T}(string, out T?)"/>.
    /// </summary>
    public class TryGetClaimTests
    {
        [Fact]
        public void TryGetClaim_ClaimExistsAndIsValid_ReturnsTrueAndParsedValue()
        {
            // Arrange
            const int expectedValue = 7;
            ClaimsPrincipal user = CreatePrincipalWithClaim(ClaimType, expectedValue.ToString());
            TestableIdentityController controller = CreateController(user);

            // Act
            bool result = controller.TryGetClaim(ClaimType, out int actualValue);

            // Assert
            Assert.True(result);
            Assert.Equal(expectedValue, actualValue);
        }

        [Fact]
        public void TryGetClaim_ClaimDoesNotExist_ReturnsFalseAndDefaultValue()
        {
            // Arrange
            ClaimsPrincipal user = CreatePrincipalWithoutClaims();
            TestableIdentityController controller = CreateController(user);

            // Act
            bool result = controller.TryGetClaim(ClaimType, out int actualValue);

            // Assert
            Assert.False(result);
            Assert.Equal(0, actualValue);
        }

        [Fact]
        public void TryGetClaim_ClaimValueIsInvalidFormat_ReturnsFalseAndDefaultValue()
        {
            // Arrange
            ClaimsPrincipal user = CreatePrincipalWithClaim(ClaimType, "not-a-number");
            TestableIdentityController controller = CreateController(user);

            // Act
            bool result = controller.TryGetClaim(ClaimType, out int actualValue);

            // Assert
            Assert.False(result);
            Assert.Equal(0, actualValue);
        }
    }
}

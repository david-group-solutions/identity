using System.Security.Claims;

using DavidGroup.Core.Identity.Extensions;

namespace DavidGroup.Core.Identity.Tests.Extensions;

/// <summary>
/// Unit tests for <see cref="ClaimsPrincipalExtensions"/>.
/// </summary>
public class ClaimsPrincipalExtensionsTests
{
    private const string ClaimType = "custom_claim";

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
    /// Tests for <see cref="ClaimsPrincipalExtensions.GetRequiredClaim(ClaimsPrincipal, string)"/>.
    /// </summary>
    public class GetRequiredClaimStringTests
    {
        [Fact]
        public void GetRequiredClaim_ClaimExists_ReturnsClaimValue()
        {
            // Arrange
            const string expectedValue = "user-123";
            ClaimsPrincipal principal = CreatePrincipalWithClaim(ClaimType, expectedValue);

            // Act
            string actualValue = principal.GetRequiredClaim(ClaimType);

            // Assert
            Assert.Equal(expectedValue, actualValue);
        }

        [Fact]
        public void GetRequiredClaim_ClaimValueIsEmptyString_ReturnsEmptyString()
        {
            // Arrange
            string expectedValue = string.Empty;
            ClaimsPrincipal principal = CreatePrincipalWithClaim(ClaimType, expectedValue);

            // Act
            string actualValue = principal.GetRequiredClaim(ClaimType);

            // Assert
            Assert.Equal(expectedValue, actualValue);
        }

        [Fact]
        public void GetRequiredClaim_ClaimDoesNotExist_ThrowsInvalidOperationException()
        {
            // Arrange
            ClaimsPrincipal principal = CreatePrincipalWithoutClaims();

            // Act
            Action act = () => principal.GetRequiredClaim(ClaimType);

            // Assert
            InvalidOperationException exception = Assert.Throws<InvalidOperationException>(act);
            Assert.Equal($"Claim '{ClaimType}' was not found.", exception.Message);
        }
    }

    /// <summary>
    /// Tests for <see cref="ClaimsPrincipalExtensions.GetRequiredClaim{T}(ClaimsPrincipal, string)"/>.
    /// </summary>
    public class GetRequiredClaimGenericTests
    {
        [Fact]
        public void GetRequiredClaim_ClaimExistsAndIsValidInt_ReturnsParsedValue()
        {
            // Arrange
            const int expectedValue = 42;
            ClaimsPrincipal principal = CreatePrincipalWithClaim(ClaimType, expectedValue.ToString());

            // Act
            int actualValue = principal.GetRequiredClaim<int>(ClaimType);

            // Assert
            Assert.Equal(expectedValue, actualValue);
        }

        [Fact]
        public void GetRequiredClaim_ClaimExistsAndIsValidGuid_ReturnsParsedValue()
        {
            // Arrange
            Guid expectedValue = Guid.NewGuid();
            ClaimsPrincipal principal = CreatePrincipalWithClaim(ClaimType, expectedValue.ToString());

            // Act
            Guid actualValue = principal.GetRequiredClaim<Guid>(ClaimType);

            // Assert
            Assert.Equal(expectedValue, actualValue);
        }

        [Fact]
        public void GetRequiredClaim_ClaimDoesNotExist_ThrowsInvalidOperationException()
        {
            // Arrange
            ClaimsPrincipal principal = CreatePrincipalWithoutClaims();

            // Act
            Action act = () => principal.GetRequiredClaim<int>(ClaimType);

            // Assert
            InvalidOperationException exception = Assert.Throws<InvalidOperationException>(act);
            Assert.Equal($"Claim '{ClaimType}' was not found.", exception.Message);
        }

        [Fact]
        public void GetRequiredClaim_ClaimValueIsInvalidFormat_ThrowsFormatException()
        {
            // Arrange
            ClaimsPrincipal principal = CreatePrincipalWithClaim(ClaimType, "not-a-number");

            // Act
            Action act = () => principal.GetRequiredClaim<int>(ClaimType);

            // Assert
            Assert.Throws<FormatException>(act);
        }
    }

    /// <summary>
    /// Tests for <see cref="ClaimsPrincipalExtensions.TryGetClaim{T}(ClaimsPrincipal, string, out T)"/>.
    /// </summary>
    public class TryGetClaimTests
    {
        [Fact]
        public void TryGetClaim_ClaimExistsAndIsValid_ReturnsTrueAndParsedValue()
        {
            // Arrange
            const int expectedValue = 7;
            ClaimsPrincipal principal = CreatePrincipalWithClaim(ClaimType, expectedValue.ToString());

            // Act
            bool result = principal.TryGetClaim(ClaimType, out int actualValue);

            // Assert
            Assert.True(result);
            Assert.Equal(expectedValue, actualValue);
        }

        [Fact]
        public void TryGetClaim_ClaimExistsAndIsValidGuid_ReturnsTrueAndParsedValue()
        {
            // Arrange
            Guid expectedValue = Guid.NewGuid();
            ClaimsPrincipal principal = CreatePrincipalWithClaim(ClaimType, expectedValue.ToString());

            // Act
            bool result = principal.TryGetClaim(ClaimType, out Guid actualValue);

            // Assert
            Assert.True(result);
            Assert.Equal(expectedValue, actualValue);
        }

        [Fact]
        public void TryGetClaim_ClaimDoesNotExist_ReturnsFalseAndDefaultValue()
        {
            // Arrange
            ClaimsPrincipal principal = CreatePrincipalWithoutClaims();

            // Act
            bool result = principal.TryGetClaim(ClaimType, out int actualValue);

            // Assert
            Assert.False(result);
            Assert.Equal(0, actualValue);
        }

        [Fact]
        public void TryGetClaim_ClaimValueIsWhitespace_ReturnsFalseAndDefaultValue()
        {
            // Arrange
            ClaimsPrincipal principal = CreatePrincipalWithClaim(ClaimType, "   ");

            // Act
            bool result = principal.TryGetClaim(ClaimType, out int actualValue);

            // Assert
            Assert.False(result);
            Assert.Equal(0, actualValue);
        }

        [Fact]
        public void TryGetClaim_ClaimValueIsInvalidFormat_ReturnsFalseAndDefaultValue()
        {
            // Arrange
            ClaimsPrincipal principal = CreatePrincipalWithClaim(ClaimType, "not-a-number");

            // Act
            bool result = principal.TryGetClaim(ClaimType, out int actualValue);

            // Assert
            Assert.False(result);
            Assert.Equal(0, actualValue);
        }
    }
}

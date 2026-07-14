using System.Security.Claims;

using DavidGroup.Core.Identity.Data;
using DavidGroup.Core.Identity.Utilities;

namespace DavidGroup.Core.Identity.Tests.Utilities;

/// <summary>
/// Unit tests for <see cref="UserHelper"/>.
/// </summary>
public static class UserHelperTests
{
    private static ClaimsPrincipal CreatePrincipalWithAuthTime(DateTimeOffset authTime)
    {
        Claim claim = new(DavidGroupClaimTypes.AuthTime, authTime.ToUnixTimeSeconds().ToString());
        ClaimsIdentity identity = new([claim]);
        return new ClaimsPrincipal(identity);
    }

    private static ClaimsPrincipal CreatePrincipalWithInvalidAuthTimeClaim()
    {
        Claim claim = new(DavidGroupClaimTypes.AuthTime, "not-a-timestamp");
        ClaimsIdentity identity = new([claim]);
        return new ClaimsPrincipal(identity);
    }

    private static ClaimsPrincipal CreatePrincipalWithoutClaims()
    {
        ClaimsIdentity identity = new();
        return new ClaimsPrincipal(identity);
    }

    /// <summary>
    /// Tests for <see cref="UserHelper.IsAuthenticatedWithin(ClaimsPrincipal, TimeSpan, DateTimeOffset?)"/>.
    /// </summary>
    public class IsAuthenticatedWithinTests
    {
        [Fact]
        public void IsAuthenticatedWithin_AuthTimeClaimMissing_ReturnsFalse()
        {
            // Arrange
            ClaimsPrincipal user = CreatePrincipalWithoutClaims();
            TimeSpan maxAge = TimeSpan.FromMinutes(5);
            DateTimeOffset now = new(2026, 7, 14, 12, 0, 0, TimeSpan.Zero);

            // Act
            bool result = UserHelper.IsAuthenticatedWithin(user, maxAge, now);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsAuthenticatedWithin_AuthTimeClaimIsInvalidFormat_ReturnsFalse()
        {
            // Arrange
            ClaimsPrincipal user = CreatePrincipalWithInvalidAuthTimeClaim();
            TimeSpan maxAge = TimeSpan.FromMinutes(5);
            DateTimeOffset now = new(2026, 7, 14, 12, 0, 0, TimeSpan.Zero);

            // Act
            bool result = UserHelper.IsAuthenticatedWithin(user, maxAge, now);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsAuthenticatedWithin_AuthTimeWithinMaxAge_ReturnsTrue()
        {
            // Arrange
            DateTimeOffset now = new(2026, 7, 14, 12, 0, 0, TimeSpan.Zero);
            DateTimeOffset authTime = now.AddMinutes(-3);
            ClaimsPrincipal user = CreatePrincipalWithAuthTime(authTime);
            TimeSpan maxAge = TimeSpan.FromMinutes(5);

            // Act
            bool result = UserHelper.IsAuthenticatedWithin(user, maxAge, now);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsAuthenticatedWithin_AuthTimeOlderThanMaxAge_ReturnsFalse()
        {
            // Arrange
            DateTimeOffset now = new(2026, 7, 14, 12, 0, 0, TimeSpan.Zero);
            DateTimeOffset authTime = now.AddMinutes(-10);
            ClaimsPrincipal user = CreatePrincipalWithAuthTime(authTime);
            TimeSpan maxAge = TimeSpan.FromMinutes(5);

            // Act
            bool result = UserHelper.IsAuthenticatedWithin(user, maxAge, now);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsAuthenticatedWithin_AuthTimeEqualsMaxAge_ReturnsTrue()
        {
            // Arrange
            DateTimeOffset now = new(2026, 7, 14, 12, 0, 0, TimeSpan.Zero);
            TimeSpan maxAge = TimeSpan.FromMinutes(5);
            DateTimeOffset authTime = now.Subtract(maxAge);
            ClaimsPrincipal user = CreatePrincipalWithAuthTime(authTime);

            // Act
            bool result = UserHelper.IsAuthenticatedWithin(user, maxAge, now);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsAuthenticatedWithin_AuthTimeIsInTheFuture_ReturnsFalse()
        {
            // Arrange
            DateTimeOffset now = new(2026, 7, 14, 12, 0, 0, TimeSpan.Zero);
            DateTimeOffset authTime = now.AddMinutes(5);
            ClaimsPrincipal user = CreatePrincipalWithAuthTime(authTime);
            TimeSpan maxAge = TimeSpan.FromMinutes(10);

            // Act
            bool result = UserHelper.IsAuthenticatedWithin(user, maxAge, now);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsAuthenticatedWithin_NowNotProvided_UsesCurrentUtcTime()
        {
            // Arrange
            DateTimeOffset authTime = DateTimeOffset.UtcNow.AddMinutes(-1);
            ClaimsPrincipal user = CreatePrincipalWithAuthTime(authTime);
            TimeSpan maxAge = TimeSpan.FromMinutes(5);

            // Act
            bool result = UserHelper.IsAuthenticatedWithin(user, maxAge);

            // Assert
            Assert.True(result);
        }
    }
}

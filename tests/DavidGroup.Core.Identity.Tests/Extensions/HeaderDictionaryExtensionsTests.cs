using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;

using DavidGroup.Core.Identity.Extensions;

namespace DavidGroup.Core.Identity.Tests.Extensions;

/// <summary>
/// Unit tests for <see cref="Identity.Extensions.HeaderDictionaryExtensions"/>.
/// </summary>
public static class HeaderDictionaryExtensionsTests
{
    /// <summary>
    /// Tests for <see cref="Identity.Extensions.HeaderDictionaryExtensions.TryGetBearerToken(IHeaderDictionary, out string?)"/>.
    /// </summary>
    public class TryGetBearerTokenTests
    {
        [Fact]
        public void TryGetBearerToken_AuthorizationHeaderMissing_ReturnsFalseAndNullToken()
        {
            // Arrange
            IHeaderDictionary headers = new HeaderDictionary();

            // Act
            bool result = headers.TryGetBearerToken(out string? token);

            // Assert
            Assert.False(result);
            Assert.Null(token);
        }

        [Fact]
        public void TryGetBearerToken_ValidBearerToken_ReturnsTrueAndExtractedToken()
        {
            // Arrange
            const string expectedToken = "abc123";
            HeaderDictionary headers = new()
            {
                [HeaderNames.Authorization] = $"Bearer {expectedToken}"
            };

            // Act
            bool result = headers.TryGetBearerToken(out string? token);

            // Assert
            Assert.True(result);
            Assert.Equal(expectedToken, token);
        }

        [Fact]
        public void TryGetBearerToken_SchemeIsLowercase_ReturnsTrueAndExtractedToken()
        {
            // Arrange
            const string expectedToken = "abc123";
            HeaderDictionary headers = new()
            {
                [HeaderNames.Authorization] = $"bearer {expectedToken}"
            };

            // Act
            bool result = headers.TryGetBearerToken(out string? token);

            // Assert
            Assert.True(result);
            Assert.Equal(expectedToken, token);
        }

        [Fact]
        public void TryGetBearerToken_TokenHasSurroundingWhitespace_ReturnsTrimmedToken()
        {
            // Arrange
            const string expectedToken = "abc123";
            HeaderDictionary headers = new()
            {
                [HeaderNames.Authorization] = $"Bearer   {expectedToken}   "
            };

            // Act
            bool result = headers.TryGetBearerToken(out string? token);

            // Assert
            Assert.True(result);
            Assert.Equal(expectedToken, token);
        }

        [Fact]
        public void TryGetBearerToken_SchemeIsNotBearer_ReturnsFalseAndNullToken()
        {
            // Arrange
            HeaderDictionary headers = new()
            {
                [HeaderNames.Authorization] = "Basic dXNlcjpwYXNz"
            };

            // Act
            bool result = headers.TryGetBearerToken(out string? token);

            // Assert
            Assert.False(result);
            Assert.Null(token);
        }

        [Fact]
        public void TryGetBearerToken_AuthorizationValueIsEmpty_ReturnsFalseAndNullToken()
        {
            // Arrange
            HeaderDictionary headers = new()
            {
                [HeaderNames.Authorization] = string.Empty
            };

            // Act
            bool result = headers.TryGetBearerToken(out string? token);

            // Assert
            Assert.False(result);
            Assert.Null(token);
        }

        [Fact]
        public void TryGetBearerToken_AuthorizationValueIsWhitespace_ReturnsFalseAndNullToken()
        {
            // Arrange
            HeaderDictionary headers = new()
            {
                [HeaderNames.Authorization] = "   "
            };

            // Act
            bool result = headers.TryGetBearerToken(out string? token);

            // Assert
            Assert.False(result);
            Assert.Null(token);
        }

        [Fact]
        public void TryGetBearerToken_BearerPrefixWithNoToken_ReturnsFalseAndNullToken()
        {
            // Arrange
            HeaderDictionary headers = new()
            {
                [HeaderNames.Authorization] = "Bearer"
            };

            // Act
            bool result = headers.TryGetBearerToken(out string? token);

            // Assert
            Assert.False(result);
            Assert.Null(token);
        }

        [Fact]
        public void TryGetBearerToken_BearerPrefixWithOnlyWhitespaceToken_ReturnsFalseAndNullToken()
        {
            // Arrange
            HeaderDictionary headers = new()
            {
                [HeaderNames.Authorization] = "Bearer    "
            };

            // Act
            bool result = headers.TryGetBearerToken(out string? token);

            // Assert
            Assert.False(result);
            Assert.Null(token);
        }

        [Fact]
        public void TryGetBearerToken_MultipleAuthorizationValues_UsesFirstValue()
        {
            // Arrange
            const string expectedToken = "first-token";
            HeaderDictionary headers = new();
            StringValues multipleValues = new([$"Bearer {expectedToken}", "Bearer second-token"]);
            headers[HeaderNames.Authorization] = multipleValues;

            // Act
            bool result = headers.TryGetBearerToken(out string? token);

            // Assert
            Assert.True(result);
            Assert.Equal(expectedToken, token);
        }
    }
}

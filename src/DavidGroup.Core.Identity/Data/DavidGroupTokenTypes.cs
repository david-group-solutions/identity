namespace DavidGroup.Core.Identity.Data;

/// <summary>
/// Custom token types in DavidGroup namespace.
/// </summary>
public static class DavidGroupTokenTypes
{
    /// <summary>
    /// Temporary token used to complete 2FA challenge.
    /// </summary>
    public const string TwoFactorAuthentication = "2fa_challenge";
}

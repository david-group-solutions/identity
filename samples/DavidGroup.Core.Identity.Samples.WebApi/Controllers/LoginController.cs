using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using DavidGroup.Core.Identity.Controllers;
using DavidGroup.Core.Identity.Data;
using DavidGroup.Core.Identity.Options;
using DavidGroup.Core.Identity.Samples.WebApi.Data;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace DavidGroup.Core.Identity.Samples.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LoginController(IOptions<JwtOptions> jwtOptions) : IdentityController
{
    [AllowAnonymous]
    [HttpGet("login")]
    public IActionResult RetrieveToken()
    {
        SymmetricSecurityKey securityKey = new(Encoding.UTF8.GetBytes(jwtOptions.Value.SecretKey));
        SigningCredentials credentials = new(securityKey, SecurityAlgorithms.HmacSha256);

        string unixNowStr = DateTimeOffset.Now.ToUnixTimeSeconds().ToString();

        JwtSecurityToken token = new(
            jwtOptions.Value.Issuer,
            jwtOptions.Value.Audience,
            [
                new Claim(JwtRegisteredClaimNames.Sub, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Nbf, unixNowStr),
                new Claim(JwtRegisteredClaimNames.Iat, unixNowStr),
                new Claim(JwtRegisteredClaimNames.Typ, "JWT"),
                new Claim(DavidGroupClaimTypes.Permission, Permissions.Books.Read),
                new Claim(DavidGroupClaimTypes.AuthTime, unixNowStr),
                new Claim(DavidGroupClaimTypes.Email, "johndoe@example.com"),
                new Claim(DavidGroupClaimTypes.EmailConfirmed, bool.TrueString),
                new Claim(DavidGroupClaimTypes.Phone, "+15555550199"),
                new Claim(DavidGroupClaimTypes.PhoneConfirmed, bool.TrueString),
                new Claim(DavidGroupClaimTypes.LicenceVersion, "v1.2"),
                new Claim(DavidGroupClaimTypes.PrivacyPolicyVersion, "v1.5")
            ],
            expires: DateTime.UtcNow.AddSeconds(300),
            signingCredentials: credentials
        );

        string? accessToken = new JwtSecurityTokenHandler().WriteToken(token);

        return Ok(accessToken);
    }

    [Authorize]
    [HttpGet("who-am-i")]
    public IActionResult WhoAmI()
    {
        if (!TryGetClaim(DavidGroupClaimTypes.AuthTime, out long authTimeInUnixSeconds))
            return NotFound();

        return Ok(new
        {
            Id = GetRequiredClaim<Guid>(JwtRegisteredClaimNames.Sub),
            Email = GetRequiredClaim(DavidGroupClaimTypes.Email),
            AuthTime = DateTimeOffset.FromUnixTimeSeconds(authTimeInUnixSeconds).ToLocalTime()
        });
    }
}

# DavidGroup.Core.Identity

#### [![Release](https://github.com/david-group-solutions/identity/actions/workflows/release.yml/badge.svg)](https://github.com/david-group-solutions/identity/actions/workflows/release.yml) [![Nuget](https://img.shields.io/nuget/v/DavidGroup.Core.Identity)](https://www.nuget.org/packages/DavidGroup.Core.Identity/)

Simplifies authentication and authorization in .NET web APIs with ready-to-use features and helpers.

---

## 🚀 Getting Started

### Install NuGet Package

Using the .NET CLI:

```bash
dotnet add package DavidGroup.Core.Identity
```

Or via the Package Manager Console:

```bash
Install-Package DavidGroup.Core.Identity
```

### How to use it?

Feel free to explore the [samples](https://github.com/david-group-solutions/identity/tree/main/samples) to find
practical examples for each feature.
New samples are added continuously as more features are developed.

## 📦 Key Features

### How to add Identity Authentication & Authorization

#### Program.cs

```csharp
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddIdentityAuthentication(builder.Configuration);
builder.Services.AddAuthorization();

builder.Services.AddControllersWithIdentity();

WebApplication app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
```

#### appsettings.json

```json
{
    "JwtOptions": {
        "Issuer": "https://auth.example.com/",
        "Audience": "https://example.com/",
        "SecretKey": "a-string-secret-at-least-256-bits-long"
    }
}
```

### Attributes

```csharp
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class BookstoreController : ControllerBase
{
    private readonly string[] _books =
    [
        "The Shining",
        "Needful Things",
        "IT",
        "Misery",
        "Doctor Sleep",
        "Cell",
        "The Institute",
        "Mr. Mercedes",
        "The Green Mile"
    ];

    [LicenseAndPrivacyPolicy("v1.2", "v1.5")] // Requires license and privacy policy to be accepted
    [HasPermission(Permissions.Books.Read)]   // Requires defined permission
    [FreshSession(60)]                        // Session must be new and within specified seconds
    [EmailConfirmed]                          // Email address must be confirmed
    [PhoneConfirmed]                          // Phone number must be confirmed
    [HttpGet("all")]
    public IActionResult All()
    {
        return Ok(_books);
    }
}
```

### IdentityController

```csharp
[ApiController]
[Route("api/[controller]")]
public class LoginController(IOptions<JwtOptions> jwtOptions) : IdentityController
{
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
```

## 🤝 Contributing

Found a bug? Have an idea? Want to contribute?

* Submit an issue:
  https://github.com/david-group-solutions/identity/issues
* Create a pull request:
  https://github.com/david-group-solutions/identity/pulls

Contributions of any size are appreciated!

## 📝 License

Distributed under the **MIT license**.
See [License](https://github.com/david-group-solutions/identity/blob/main/LICENSE.txt) for more information.

Copyright © 2025-2026 David Khachatryan (David Group Solutions)

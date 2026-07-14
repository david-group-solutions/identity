using DavidGroup.Core.Identity.Attributes;
using DavidGroup.Core.Identity.Samples.WebApi.Data;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DavidGroup.Core.Identity.Samples.WebApi.Controllers;

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

    [LicenseAndPrivacyPolicy("v1.2", "v1.5")]
    [HasPermission(Permissions.Books.Read)]
    [FreshSession(60)]
    [EmailConfirmed]
    [PhoneConfirmed]
    [HttpGet("all")]
    public IActionResult All()
    {
        return Ok(_books);
    }

    [LicenseAndPrivacyPolicy("v1.2", "v1.5")]
    [HttpGet("license-and-privacy-policy")]
    public IActionResult LicenseAndPrivacyPolicy()
    {
        return Ok(_books);
    }

    [HasPermission(Permissions.Books.Read)]
    [HttpGet("has-permission")]
    public IActionResult HasPermission()
    {
        return Ok(_books);
    }

    [FreshSession(60)]
    [HttpGet("fresh-session")]
    public IActionResult FreshSession()
    {
        return Ok(_books);
    }

    [EmailConfirmed]
    [HttpGet("email-confirmed")]
    public IActionResult EmailConfirmed()
    {
        return Ok(_books);
    }

    [PhoneConfirmed]
    [HttpGet("phone-confirmed")]
    public IActionResult PhoneConfirmed()
    {
        return Ok(_books);
    }
}

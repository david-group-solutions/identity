using DavidGroup.Core.Identity.Attributes;
using DavidGroup.Core.Identity.Samples.WebApi.Data;
using DavidGroup.Core.Identity.Samples.WebApi.HttpClients;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DavidGroup.Core.Identity.Samples.WebApi.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class InventoryController(IInventoryClient inventoryClient) : ControllerBase
{
    [HasPermission(Permissions.Books.Read)]
    [HttpGet("{id:guid}/availability")]
    public async Task<IActionResult> GetAvailability(Guid id)
    {
        InventoryAvailabilityResponse result = await inventoryClient.IsBookAvailableAsync(id);

        return Ok(result);
    }
}

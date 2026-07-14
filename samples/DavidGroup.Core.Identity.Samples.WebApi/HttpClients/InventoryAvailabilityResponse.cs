namespace DavidGroup.Core.Identity.Samples.WebApi.HttpClients;

public sealed record InventoryAvailabilityResponse(Guid BookId, bool Available, string? AuthorizationHeader);

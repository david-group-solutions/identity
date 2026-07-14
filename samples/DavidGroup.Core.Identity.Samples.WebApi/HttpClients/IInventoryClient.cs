namespace DavidGroup.Core.Identity.Samples.WebApi.HttpClients;

public interface IInventoryClient
{
    Task<InventoryAvailabilityResponse> IsBookAvailableAsync(Guid bookId, CancellationToken cancellationToken = default);
}

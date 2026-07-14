namespace DavidGroup.Core.Identity.Samples.WebApi.HttpClients;

public sealed class InventoryClient(HttpClient httpClient) : IInventoryClient
{
    public async Task<InventoryAvailabilityResponse> IsBookAvailableAsync(Guid bookId,
        CancellationToken cancellationToken = default)
    {
        InventoryAvailabilityResponse? response = await httpClient.GetFromJsonAsync<InventoryAvailabilityResponse>(
            $"api/books/{bookId}/availability",
            cancellationToken);

        return response!;
    }
}

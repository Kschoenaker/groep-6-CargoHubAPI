using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using MyApi.Data;
using MyApi.Models;

namespace MyApi.Tests.Integration;

public class TransferItemsIntegrationTests
{
    private record TransferItemResponse(int item_id, int amount);

    private static async Task CreateTransfer(HttpClient client)
    {
        await client.PostAsJsonAsync("/api/v2/warehouses",
            new { code = "WH001", name = "Rotterdam", country = "NL" });
        await client.PostAsJsonAsync("/api/v2/locations",
            new { warehouse_id = 1, code = "A.1.0", name = "Row A Rack 1" });
        await client.PostAsJsonAsync("/api/v2/locations",
            new { warehouse_id = 1, code = "A.2.0", name = "Row A Rack 2" });
        await client.PostAsJsonAsync("/api/v2/transfers",
            new { reference = "TR00001", from_location_id = 1, to_location_id = 2, transfer_status = "Scheduled" });
    }

    private static async Task AddTransferItem(CargoHubApiFactory factory, int itemId, int amount)
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        db.TransferItems.Add(new TransferItem { transfer_id = 1, item_id = itemId, amount = amount });
        await db.SaveChangesAsync();
    }

    [Fact]
    public async Task Get_ReturnsItemsOfTransfer()
    {
        using var factory = new CargoHubApiFactory();
        var client = factory.CreateClient();
        await CreateTransfer(client);
        await AddTransferItem(factory, itemId: 10, amount: 5);
        await AddTransferItem(factory, itemId: 11, amount: 3);

        var items = await client.GetFromJsonAsync<List<TransferItemResponse>>("/api/v2/transfers/1/items");

        Assert.Equal(2, items!.Count);
        Assert.Contains(new TransferItemResponse(10, 5), items);
        Assert.Contains(new TransferItemResponse(11, 3), items);
    }

    [Fact]
    public async Task Get_TransferWithoutItems_ReturnsEmptyList()
    {
        using var factory = new CargoHubApiFactory();
        var client = factory.CreateClient();
        await CreateTransfer(client);

        var response = await client.GetAsync("/api/v2/transfers/1/items");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var items = await response.Content.ReadFromJsonAsync<List<TransferItemResponse>>();
        Assert.Empty(items!);
    }

    [Fact]
    public async Task DeleteTransfer_AlsoRemovesItsItems()
    {
        using var factory = new CargoHubApiFactory();
        var client = factory.CreateClient();
        await CreateTransfer(client);
        await AddTransferItem(factory, itemId: 10, amount: 5);
        await AddTransferItem(factory, itemId: 11, amount: 3);

        var response = await client.DeleteAsync("/api/v2/transfers/1");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        Assert.Empty(db.TransferItems);
    }
}

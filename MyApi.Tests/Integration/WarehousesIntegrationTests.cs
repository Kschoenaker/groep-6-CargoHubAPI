using System.Net;
using System.Net.Http.Json;
using MyApi.Models;

namespace MyApi.Tests.Integration;

public class WarehousesIntegrationTests
{
    [Fact]
    public async Task Post_CreatesWarehouse()
    {
        using var factory = new CargoHubApiFactory();
        var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/v2/warehouses",
            new { code = "WH001", name = "Rotterdam", country = "NL" });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task Post_MissingRequiredField_ReturnsBadRequest()
    {
        using var factory = new CargoHubApiFactory();
        var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/v2/warehouses",
            new { code = "WH001" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Get_ReturnsAllWarehouses()
    {
        using var factory = new CargoHubApiFactory();
        var client = factory.CreateClient();
        await client.PostAsJsonAsync("/api/v2/warehouses",
            new { code = "WH001", name = "Rotterdam", country = "NL" });
        await client.PostAsJsonAsync("/api/v2/warehouses",
            new { code = "WH002", name = "Amsterdam", country = "NL" });

        var warehouses = await client.GetFromJsonAsync<List<Warehouse>>("/api/v2/warehouses");

        Assert.Equal(2, warehouses!.Count);
    }

    [Fact]
    public async Task Get_ReturnsCreatedWarehouse()
    {
        using var factory = new CargoHubApiFactory();
        var client = factory.CreateClient();
        await client.PostAsJsonAsync("/api/v2/warehouses",
            new { code = "WH001", name = "Rotterdam", country = "NL" });

        var warehouse = await client.GetFromJsonAsync<Warehouse>("/api/v2/warehouses/1");

        Assert.Equal("WH001", warehouse!.code);
        Assert.Equal("Rotterdam", warehouse.name);
    }

    [Fact]
    public async Task Get_UnknownId_ReturnsNotFound()
    {
        using var factory = new CargoHubApiFactory();
        var client = factory.CreateClient();

        var response = await client.GetAsync("/api/v2/warehouses/999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetLocations_ReturnsOnlyLocationsOfWarehouse()
    {
        using var factory = new CargoHubApiFactory();
        var client = factory.CreateClient();
        await client.PostAsJsonAsync("/api/v2/warehouses",
            new { code = "WH001", name = "Rotterdam", country = "NL" });
        await client.PostAsJsonAsync("/api/v2/warehouses",
            new { code = "WH002", name = "Amsterdam", country = "NL" });
        await client.PostAsJsonAsync("/api/v2/locations",
            new { warehouse_id = 1, code = "A.1.0", name = "Row A Rack 1" });
        await client.PostAsJsonAsync("/api/v2/locations",
            new { warehouse_id = 1, code = "A.2.0", name = "Row A Rack 2" });
        await client.PostAsJsonAsync("/api/v2/locations",
            new { warehouse_id = 2, code = "B.1.0", name = "Row B Rack 1" });

        var locations = await client.GetFromJsonAsync<List<Location>>("/api/v2/warehouses/1/locations");

        Assert.Equal(2, locations!.Count);
        Assert.All(locations, location => Assert.Equal(1, location.warehouse_id));
    }

    [Fact]
    public async Task Put_UpdatesWarehouse()
    {
        using var factory = new CargoHubApiFactory();
        var client = factory.CreateClient();
        await client.PostAsJsonAsync("/api/v2/warehouses",
            new { code = "WH001", name = "Rotterdam", country = "NL" });

        var response = await client.PutAsJsonAsync("/api/v2/warehouses/1",
            new { code = "WH001", name = "Amsterdam", country = "NL" });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var warehouse = await client.GetFromJsonAsync<Warehouse>("/api/v2/warehouses/1");
        Assert.Equal("Amsterdam", warehouse!.name);
    }

    [Fact]
    public async Task Put_UnknownId_ReturnsNotFound()
    {
        using var factory = new CargoHubApiFactory();
        var client = factory.CreateClient();

        var response = await client.PutAsJsonAsync("/api/v2/warehouses/999",
            new { code = "WH001", name = "Rotterdam", country = "NL" });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_RemovesWarehouse()
    {
        using var factory = new CargoHubApiFactory();
        var client = factory.CreateClient();
        await client.PostAsJsonAsync("/api/v2/warehouses",
            new { code = "WH001", name = "Rotterdam", country = "NL" });

        var response = await client.DeleteAsync("/api/v2/warehouses/1");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        var getResponse = await client.GetAsync("/api/v2/warehouses/1");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task Delete_UnknownId_ReturnsNotFound()
    {
        using var factory = new CargoHubApiFactory();
        var client = factory.CreateClient();

        var response = await client.DeleteAsync("/api/v2/warehouses/999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}

using System.Net;
using System.Net.Http.Json;
using MyApi.Models;

namespace MyApi.Tests.Integration;

public class LocationsIntegrationTests
{
    private static async Task CreateWarehouse(HttpClient client)
    {
        await client.PostAsJsonAsync("/api/v2/warehouses",
            new { code = "WH001", name = "Rotterdam", country = "NL" });
    }

    [Fact]
    public async Task Post_CreatesLocation()
    {
        using var factory = new CargoHubApiFactory();
        var client = factory.CreateClient();
        await CreateWarehouse(client);

        var response = await client.PostAsJsonAsync("/api/v2/locations",
            new { warehouse_id = 1, code = "A.1.0", name = "Row A Rack 1" });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task Post_UnknownWarehouse_ReturnsBadRequest()
    {
        using var factory = new CargoHubApiFactory();
        var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/v2/locations",
            new { warehouse_id = 999, code = "A.1.0", name = "Row A Rack 1" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Get_ReturnsAllLocations()
    {
        using var factory = new CargoHubApiFactory();
        var client = factory.CreateClient();
        await CreateWarehouse(client);
        await client.PostAsJsonAsync("/api/v2/locations",
            new { warehouse_id = 1, code = "A.1.0", name = "Row A Rack 1" });
        await client.PostAsJsonAsync("/api/v2/locations",
            new { warehouse_id = 1, code = "A.2.0", name = "Row A Rack 2" });

        var locations = await client.GetFromJsonAsync<List<Location>>("/api/v2/locations");

        Assert.Equal(2, locations!.Count);
    }

    [Fact]
    public async Task Get_ReturnsCreatedLocation()
    {
        using var factory = new CargoHubApiFactory();
        var client = factory.CreateClient();
        await CreateWarehouse(client);
        await client.PostAsJsonAsync("/api/v2/locations",
            new { warehouse_id = 1, code = "A.1.0", name = "Row A Rack 1" });

        var location = await client.GetFromJsonAsync<Location>("/api/v2/locations/1");

        Assert.Equal("A.1.0", location!.code);
        Assert.Equal(1, location.warehouse_id);
    }

    [Fact]
    public async Task Get_UnknownId_ReturnsNotFound()
    {
        using var factory = new CargoHubApiFactory();
        var client = factory.CreateClient();

        var response = await client.GetAsync("/api/v2/locations/999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Put_UpdatesLocation()
    {
        using var factory = new CargoHubApiFactory();
        var client = factory.CreateClient();
        await CreateWarehouse(client);
        await client.PostAsJsonAsync("/api/v2/locations",
            new { warehouse_id = 1, code = "A.1.0", name = "Row A Rack 1" });

        var response = await client.PutAsJsonAsync("/api/v2/locations/1",
            new { warehouse_id = 1, code = "B.1.0", name = "Row B Rack 1" });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var location = await client.GetFromJsonAsync<Location>("/api/v2/locations/1");
        Assert.Equal("B.1.0", location!.code);
    }

    [Fact]
    public async Task Put_UnknownWarehouse_ReturnsBadRequest()
    {
        using var factory = new CargoHubApiFactory();
        var client = factory.CreateClient();
        await CreateWarehouse(client);
        await client.PostAsJsonAsync("/api/v2/locations",
            new { warehouse_id = 1, code = "A.1.0", name = "Row A Rack 1" });

        var response = await client.PutAsJsonAsync("/api/v2/locations/1",
            new { warehouse_id = 999, code = "A.1.0", name = "Row A Rack 1" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var location = await client.GetFromJsonAsync<Location>("/api/v2/locations/1");
        Assert.Equal(1, location!.warehouse_id);
    }

    [Fact]
    public async Task Put_UnknownId_ReturnsNotFound()
    {
        using var factory = new CargoHubApiFactory();
        var client = factory.CreateClient();
        await CreateWarehouse(client);

        var response = await client.PutAsJsonAsync("/api/v2/locations/999",
            new { warehouse_id = 1, code = "A.1.0", name = "Row A Rack 1" });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_RemovesLocation()
    {
        using var factory = new CargoHubApiFactory();
        var client = factory.CreateClient();
        await CreateWarehouse(client);
        await client.PostAsJsonAsync("/api/v2/locations",
            new { warehouse_id = 1, code = "A.1.0", name = "Row A Rack 1" });

        var response = await client.DeleteAsync("/api/v2/locations/1");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        var getResponse = await client.GetAsync("/api/v2/locations/1");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task Delete_UnknownId_ReturnsNotFound()
    {
        using var factory = new CargoHubApiFactory();
        var client = factory.CreateClient();

        var response = await client.DeleteAsync("/api/v2/locations/999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}

using System.Net;
using System.Net.Http.Json;
using MyApi.Models;

namespace MyApi.Tests.Integration;

public class TransfersIntegrationTests
{
    private static async Task CreateTwoLocations(HttpClient client)
    {
        await client.PostAsJsonAsync("/api/v2/warehouses",
            new { code = "WH001", name = "Rotterdam", country = "NL" });
        await client.PostAsJsonAsync("/api/v2/locations",
            new { warehouse_id = 1, code = "A.1.0", name = "Row A Rack 1" });
        await client.PostAsJsonAsync("/api/v2/locations",
            new { warehouse_id = 1, code = "A.2.0", name = "Row A Rack 2" });
    }

    [Fact]
    public async Task Post_CreatesTransfer()
    {
        using var factory = new CargoHubApiFactory();
        var client = factory.CreateClient();
        await CreateTwoLocations(client);

        var response = await client.PostAsJsonAsync("/api/v2/transfers",
            new { reference = "TR00001", from_location_id = 1, to_location_id = 2, transfer_status = "Scheduled" });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task Post_SameFromAndToLocation_ReturnsBadRequest()
    {
        using var factory = new CargoHubApiFactory();
        var client = factory.CreateClient();
        await CreateTwoLocations(client);

        var response = await client.PostAsJsonAsync("/api/v2/transfers",
            new { reference = "TR00001", from_location_id = 1, to_location_id = 1, transfer_status = "Scheduled" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Post_UnknownLocation_ReturnsBadRequest()
    {
        using var factory = new CargoHubApiFactory();
        var client = factory.CreateClient();
        await CreateTwoLocations(client);

        var response = await client.PostAsJsonAsync("/api/v2/transfers",
            new { reference = "TR00001", from_location_id = 1, to_location_id = 999, transfer_status = "Scheduled" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Get_ReturnsAllTransfers()
    {
        using var factory = new CargoHubApiFactory();
        var client = factory.CreateClient();
        await CreateTwoLocations(client);
        await client.PostAsJsonAsync("/api/v2/transfers",
            new { reference = "TR00001", from_location_id = 1, to_location_id = 2, transfer_status = "Scheduled" });
        await client.PostAsJsonAsync("/api/v2/transfers",
            new { reference = "TR00002", from_location_id = 2, to_location_id = 1, transfer_status = "Scheduled" });

        var transfers = await client.GetFromJsonAsync<List<Transfer>>("/api/v2/transfers");

        Assert.Equal(2, transfers!.Count);
    }

    [Fact]
    public async Task Get_ReturnsCreatedTransfer()
    {
        using var factory = new CargoHubApiFactory();
        var client = factory.CreateClient();
        await CreateTwoLocations(client);
        await client.PostAsJsonAsync("/api/v2/transfers",
            new { reference = "TR00001", from_location_id = 1, to_location_id = 2, transfer_status = "Scheduled" });

        var transfer = await client.GetFromJsonAsync<Transfer>("/api/v2/transfers/1");

        Assert.Equal("TR00001", transfer!.reference);
        Assert.Equal("Scheduled", transfer.transfer_status);
    }

    [Fact]
    public async Task Get_UnknownId_ReturnsNotFound()
    {
        using var factory = new CargoHubApiFactory();
        var client = factory.CreateClient();

        var response = await client.GetAsync("/api/v2/transfers/999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Put_UpdatesTransfer()
    {
        using var factory = new CargoHubApiFactory();
        var client = factory.CreateClient();
        await CreateTwoLocations(client);
        await client.PostAsJsonAsync("/api/v2/transfers",
            new { reference = "TR00001", from_location_id = 1, to_location_id = 2, transfer_status = "Scheduled" });

        var response = await client.PutAsJsonAsync("/api/v2/transfers/1",
            new { reference = "TR00001", from_location_id = 1, to_location_id = 2, transfer_status = "Completed" });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var transfer = await client.GetFromJsonAsync<Transfer>("/api/v2/transfers/1");
        Assert.Equal("Completed", transfer!.transfer_status);
    }

    [Fact]
    public async Task Put_SameFromAndToLocation_ReturnsBadRequest()
    {
        using var factory = new CargoHubApiFactory();
        var client = factory.CreateClient();
        await CreateTwoLocations(client);
        await client.PostAsJsonAsync("/api/v2/transfers",
            new { reference = "TR00001", from_location_id = 1, to_location_id = 2, transfer_status = "Scheduled" });

        var response = await client.PutAsJsonAsync("/api/v2/transfers/1",
            new { reference = "TR00001", from_location_id = 2, to_location_id = 2, transfer_status = "Scheduled" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Put_UnknownLocation_ReturnsBadRequest()
    {
        using var factory = new CargoHubApiFactory();
        var client = factory.CreateClient();
        await CreateTwoLocations(client);
        await client.PostAsJsonAsync("/api/v2/transfers",
            new { reference = "TR00001", from_location_id = 1, to_location_id = 2, transfer_status = "Scheduled" });

        var response = await client.PutAsJsonAsync("/api/v2/transfers/1",
            new { reference = "TR00001", from_location_id = 999, to_location_id = 2, transfer_status = "Scheduled" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var transfer = await client.GetFromJsonAsync<Transfer>("/api/v2/transfers/1");
        Assert.Equal(1, transfer!.from_location_id);
    }

    [Fact]
    public async Task Put_UnknownId_ReturnsNotFound()
    {
        using var factory = new CargoHubApiFactory();
        var client = factory.CreateClient();
        await CreateTwoLocations(client);

        var response = await client.PutAsJsonAsync("/api/v2/transfers/999",
            new { reference = "TR00001", from_location_id = 1, to_location_id = 2, transfer_status = "Scheduled" });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_RemovesTransfer()
    {
        using var factory = new CargoHubApiFactory();
        var client = factory.CreateClient();
        await CreateTwoLocations(client);
        await client.PostAsJsonAsync("/api/v2/transfers",
            new { reference = "TR00001", from_location_id = 1, to_location_id = 2, transfer_status = "Scheduled" });

        var response = await client.DeleteAsync("/api/v2/transfers/1");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        var getResponse = await client.GetAsync("/api/v2/transfers/1");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task Delete_UnknownId_ReturnsNotFound()
    {
        using var factory = new CargoHubApiFactory();
        var client = factory.CreateClient();

        var response = await client.DeleteAsync("/api/v2/transfers/999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}

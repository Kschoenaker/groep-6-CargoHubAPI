using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;

namespace MyApi.Tests;


public class ShipmentItemsEndpointTests
    : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ShipmentItemsEndpointTests(
        WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetShipmentItems_ReturnsOk()
    {
        // Arrange
        var request = "/api/v2/shipment-items";

        // Act
        var response = await _client.GetAsync(request); //stuur http request naar de endpoint

        // Assert
        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode
        );
    }

    [Fact]
    public async Task GetShipmentItemsById_ReturnsOk()
    {
        // Arrange
        var request = "/api/v2/shipment-items/1";

        // Act
        var response = await _client.GetAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetShipmentItemsById_ReturnsNotFound()
    {
        // Arrange
        var request = "/api/v2/ShipmentItems/9999999";

        // Act
        var response = await _client.GetAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
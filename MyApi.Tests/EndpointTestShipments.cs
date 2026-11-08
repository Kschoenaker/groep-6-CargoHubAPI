using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;

namespace MyApi.Tests;


public class ShipmentsEndpointTests
    : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ShipmentsEndpointTests(
        WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetShipments_ReturnsOk()
    {
        // Arrange
        var request = "/api/v2/shipments";

        // Act
        var response = await _client.GetAsync(request); //stuur http request naar de endpoint

        // Assert
        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode
        );
    }

    [Fact]
    public async Task GetShipmentById_ReturnsOk()
    {
        // Arrange
        var request = "/api/v2/shipments/1";

        // Act
        var response = await _client.GetAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetShipmentById_ReturnsNotFound()
    {
        // Arrange
        var request = "/api/v2/shipments/9999999";

        // Act
        var response = await _client.GetAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
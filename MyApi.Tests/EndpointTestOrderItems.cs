using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;

namespace MyApi.Tests;


public class OrderItemsEndpointTests
    : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public OrderItemsEndpointTests(
        WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetOrderItems_ReturnsOk()
    {
        // Arrange
        var request = "/api/v2/orderitems";

        // Act
        var response = await _client.GetAsync(request); //stuur http request naar de endpoint

        // Assert
        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode
        );
    }

    [Fact]
    public async Task GetOrderItemsById_ReturnsOk()
    {
        // Arrange
        var request = "/api/v2/orderitems/1";

        // Act
        var response = await _client.GetAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetOrderItemsById_ReturnsNotFound()
    {
        // Arrange
        var request = "/api/v2/orderitems/9999999";

        // Act
        var response = await _client.GetAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApi.Controllers;
using MyApi.Data;
using MyApi.Models;
using Xunit;

namespace MyApi.Tests;

public class ClientsControllerTests
{
    private static AppDbContext CreateContext() // temporary database
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    private static Client CreateClient() // mock data
    {
        return new Client
        {
            id = 1,
            name = "Test Client",
            address = "Test Street 1",
            city = "Brussels",
            zip_code = "1000",
            province = "Brussels",
            country = "Belgium",
            contact_name = "John Doe",
            contact_phone = "0123456789",
            contact_email = "test@example.com",
            created_at = DateTime.UtcNow,
            updated_at = DateTime.UtcNow
        };
    }

    // Integration test
    [Fact] 
    public async Task GetClients_ReturnsAllClients()
    {
        using var context = CreateContext();

        context.Clients.Add(CreateClient());
        await context.SaveChangesAsync();

        var controller = new ClientsController(context);

        var result = await controller.GetClients();

        var okResult = Assert.IsType<OkObjectResult>(result);
        var clients = Assert.IsType<List<Client>>(okResult.Value);

        Assert.Single(clients);
    }

    // test checks of  HTTP 200 OK is returned
    [Fact]
    public async Task GetClients_ReturnsOk()
    {
        using var context = CreateContext();

        var controller = new ClientsController(context);

        var result = await controller.GetClients();

        Assert.IsType<OkObjectResult>(result);
    }

[Fact] // test if a new client can be created
public async Task CreateClient_Test()
{
    using var context = CreateContext();

    var client = CreateClient();

    var controller = new ClientsController(context);

    var result = await controller.CreateClient(client);

    var okResult = Assert.IsType<OkObjectResult>(result);
    var createdClient = Assert.IsType<Client>(okResult.Value);

    Assert.Equal(1, createdClient.id);
    Assert.Equal("Test Client", createdClient.name);

    Assert.Single(context.Clients);
}

// PUT test
[Fact] // test if a client can be updated
public async Task UpdateClient_ReturnsOk()
{
    using var context = CreateContext();

    context.Clients.Add(CreateClient());
    await context.SaveChangesAsync();

    var client = CreateClient();
    client.name = "Updated Client";

    var controller = new ClientsController(context);

    var result = await controller.UpdateClient(1, client);

    var okResult = Assert.IsType<OkObjectResult>(result);
    var updatedClient = Assert.IsType<Client>(okResult.Value);

    Assert.Equal("Updated Client", updatedClient.name);
}

[Fact] // test if a client can be deleted
public async Task DeleteClient_ReturnsOk()
{
    using var context = CreateContext();

    context.Clients.Add(CreateClient());
    await context.SaveChangesAsync();

    var controller = new ClientsController(context);

    var result = await controller.DeleteClient(1);

    Assert.IsType<OkResult>(result);

    Assert.Empty(context.Clients);
}

}
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApi.Controllers;
using MyApi.Data;
using MyApi.Models;

namespace MyApi.Tests.Unit;

public class LocationsControllerTests
{
    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var context = new AppDbContext(options);

        context.Warehouses.Add(new Warehouse { id = 1, code = "WH001", name = "Rotterdam", country = "NL" });
        context.SaveChanges();

        return context;
    }

    private static Location CreateLocation(int id = 1, int warehouseId = 1)
    {
        return new Location { id = id, warehouse_id = warehouseId, code = $"A.{id}.0", name = $"Row A Rack {id}" };
    }

    [Fact]
    public async Task GetLocations_ReturnsAllLocations()
    {
        using var context = CreateContext();
        context.Locations.AddRange(CreateLocation(1), CreateLocation(2));
        await context.SaveChangesAsync();
        var controller = new LocationsController(context);

        var result = await controller.getLocations();

        var ok = Assert.IsType<OkObjectResult>(result);
        var locations = Assert.IsType<List<Location>>(ok.Value);
        Assert.Equal(2, locations.Count);
    }

    [Fact]
    public async Task GetLocationById_ReturnsLocation()
    {
        using var context = CreateContext();
        context.Locations.Add(CreateLocation(1));
        await context.SaveChangesAsync();
        var controller = new LocationsController(context);

        var result = await controller.GetLocationById(1);

        var ok = Assert.IsType<OkObjectResult>(result);
        var location = Assert.IsType<Location>(ok.Value);
        Assert.Equal("A.1.0", location.code);
    }

    [Fact]
    public async Task GetLocationById_UnknownId_ReturnsNotFound()
    {
        using var context = CreateContext();
        var controller = new LocationsController(context);

        var result = await controller.GetLocationById(999);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task CreateLocation_SavesLocation()
    {
        using var context = CreateContext();
        var controller = new LocationsController(context);

        var result = await controller.CreateLocation(CreateLocation());

        Assert.IsType<CreatedAtActionResult>(result);
        Assert.Single(context.Locations);
    }

    [Fact]
    public async Task CreateLocation_UnknownWarehouse_ReturnsBadRequest()
    {
        using var context = CreateContext();
        var controller = new LocationsController(context);

        var result = await controller.CreateLocation(CreateLocation(warehouseId: 999));

        Assert.IsType<BadRequestObjectResult>(result);
        Assert.Empty(context.Locations);
    }

    [Fact]
    public async Task UpdateLocation_ChangesLocation()
    {
        using var context = CreateContext();
        context.Locations.Add(CreateLocation(1));
        await context.SaveChangesAsync();
        var controller = new LocationsController(context);

        var updated = CreateLocation(1);
        updated.code = "B.1.0";
        var result = await controller.UpdateLocation(1, updated);

        Assert.IsType<OkObjectResult>(result);
        Assert.Equal("B.1.0", context.Locations.Find(1)!.code);
    }

    [Fact]
    public async Task UpdateLocation_UnknownWarehouse_ReturnsBadRequest()
    {
        using var context = CreateContext();
        context.Locations.Add(CreateLocation(1));
        await context.SaveChangesAsync();
        var controller = new LocationsController(context);

        var result = await controller.UpdateLocation(1, CreateLocation(1, warehouseId: 999));

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task UpdateLocation_UnknownId_ReturnsNotFound()
    {
        using var context = CreateContext();
        var controller = new LocationsController(context);

        var result = await controller.UpdateLocation(999, CreateLocation());

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task DeleteLocation_RemovesLocation()
    {
        using var context = CreateContext();
        context.Locations.Add(CreateLocation(1));
        await context.SaveChangesAsync();
        var controller = new LocationsController(context);

        var result = await controller.DeleteLocation(1);

        Assert.IsType<NoContentResult>(result);
        Assert.Empty(context.Locations);
    }

    [Fact]
    public async Task DeleteLocation_UnknownId_ReturnsNotFound()
    {
        using var context = CreateContext();
        var controller = new LocationsController(context);

        var result = await controller.DeleteLocation(999);

        Assert.IsType<NotFoundResult>(result);
    }
}

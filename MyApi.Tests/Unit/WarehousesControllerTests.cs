using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApi.Controllers;
using MyApi.Data;
using MyApi.Models;

namespace MyApi.Tests.Unit;

public class WarehousesControllerTests
{
    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    private static Warehouse CreateWarehouse(int id = 1)
    {
        return new Warehouse { id = id, code = $"WH00{id}", name = "Rotterdam", country = "NL" };
    }

    [Fact]
    public async Task GetWarehouses_ReturnsAllWarehouses()
    {
        using var context = CreateContext();
        context.Warehouses.AddRange(CreateWarehouse(1), CreateWarehouse(2));
        await context.SaveChangesAsync();
        var controller = new WarehousesController(context);

        var result = await controller.getWareHouses();

        var ok = Assert.IsType<OkObjectResult>(result);
        var warehouses = Assert.IsType<List<Warehouse>>(ok.Value);
        Assert.Equal(2, warehouses.Count);
    }

    [Fact]
    public async Task GetWarehouseById_ReturnsWarehouse()
    {
        using var context = CreateContext();
        context.Warehouses.Add(CreateWarehouse(1));
        await context.SaveChangesAsync();
        var controller = new WarehousesController(context);

        var result = await controller.GetWarehouseById(1);

        var ok = Assert.IsType<OkObjectResult>(result);
        var warehouse = Assert.IsType<Warehouse>(ok.Value);
        Assert.Equal("WH001", warehouse.code);
    }

    [Fact]
    public async Task GetWarehouseById_UnknownId_ReturnsNotFound()
    {
        using var context = CreateContext();
        var controller = new WarehousesController(context);

        var result = await controller.GetWarehouseById(999);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task GetLocationsByWarehouseId_ReturnsOnlyLocationsOfWarehouse()
    {
        using var context = CreateContext();
        context.Locations.AddRange(
            new Location { id = 1, warehouse_id = 1, code = "A.1.0", name = "A1" },
            new Location { id = 2, warehouse_id = 1, code = "A.2.0", name = "A2" },
            new Location { id = 3, warehouse_id = 2, code = "B.1.0", name = "B1" });
        await context.SaveChangesAsync();
        var controller = new WarehousesController(context);

        var result = await controller.GetLocationsByWarehouseId(1);

        var ok = Assert.IsType<OkObjectResult>(result);
        var locations = Assert.IsType<List<Location>>(ok.Value);
        Assert.Equal(2, locations.Count);
    }

    [Fact]
    public async Task CreateWarehouse_SavesWarehouse()
    {
        using var context = CreateContext();
        var controller = new WarehousesController(context);

        var result = await controller.CreateWarehouse(CreateWarehouse());

        Assert.IsType<CreatedAtActionResult>(result);
        Assert.Single(context.Warehouses);
    }

    [Fact]
    public async Task UpdateWarehouse_ChangesWarehouse()
    {
        using var context = CreateContext();
        context.Warehouses.Add(CreateWarehouse(1));
        await context.SaveChangesAsync();
        var controller = new WarehousesController(context);

        var updated = CreateWarehouse(1);
        updated.name = "Amsterdam";
        var result = await controller.UpdateWarehouse(1, updated);

        Assert.IsType<OkObjectResult>(result);
        Assert.Equal("Amsterdam", context.Warehouses.Find(1)!.name);
    }

    [Fact]
    public async Task UpdateWarehouse_UnknownId_ReturnsNotFound()
    {
        using var context = CreateContext();
        var controller = new WarehousesController(context);

        var result = await controller.UpdateWarehouse(999, CreateWarehouse());

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task DeleteWarehouse_RemovesWarehouse()
    {
        using var context = CreateContext();
        context.Warehouses.Add(CreateWarehouse(1));
        await context.SaveChangesAsync();
        var controller = new WarehousesController(context);

        var result = await controller.DeleteWarehouse(1);

        Assert.IsType<NoContentResult>(result);
        Assert.Empty(context.Warehouses);
    }

    [Fact]
    public async Task DeleteWarehouse_UnknownId_ReturnsNotFound()
    {
        using var context = CreateContext();
        var controller = new WarehousesController(context);

        var result = await controller.DeleteWarehouse(999);

        Assert.IsType<NotFoundResult>(result);
    }
}

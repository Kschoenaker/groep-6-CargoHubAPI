using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApi.Controllers;
using MyApi.Data;
using MyApi.Models;
using Xunit;

namespace MyApi.Tests;

public class SuppliersControllerTests
{
    private static AppDbContext CreateContext() // temporary database
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    private static Supplier CreateSupplier() // mock data
    {
        return new Supplier
        {
            id = 1,
            province = "Brussels",
            country = "Belgium",
            contact_name = "John Doe",
            phone_number = "0123456789",
            reference = "SUP-001",
            created_at = DateTime.UtcNow,
            updated_at = DateTime.UtcNow
        };
    }

    // Integration test
    [Fact] // checks of all suppliers returned zijn
    public async Task GetSuppliers_ReturnsAllSuppliers()
    {
        using var context = CreateContext();

        context.Suppliers.Add(CreateSupplier());
        await context.SaveChangesAsync();

        var controller = new SuppliersController(context);

        var result = await controller.GetSuppliers();

        var okResult = Assert.IsType<OkObjectResult>(result);
        var suppliers = Assert.IsType<List<Supplier>>(okResult.Value);

        Assert.Single(suppliers);
    }

    // checks of HTTP 200 OK is returned
    [Fact]
    public async Task GetSuppliers_ReturnsOk()
    {
        using var context = CreateContext();

        var controller = new SuppliersController(context);

        var result = await controller.GetSuppliers();

        Assert.IsType<OkObjectResult>(result);
    }



[Fact]
public async Task CreateSupplier_Test()
{
    using var context = CreateContext();

    var supplier = CreateSupplier();
    var controller = new SuppliersController(context);

    var result = await controller.CreateSupplier(supplier);

    var okResult = Assert.IsType<OkObjectResult>(result);
    var createdSupplier = Assert.IsType<Supplier>(okResult.Value);

    Assert.Equal(1, createdSupplier.id);
    Assert.Equal("SUP-001", createdSupplier.reference);
    Assert.Single(context.Suppliers);
}


[Fact]
public async Task UpdateSupplier_ReturnsOk()
{
    using var context = CreateContext();

    context.Suppliers.Add(CreateSupplier());
    await context.SaveChangesAsync();

    var supplier = CreateSupplier();
    supplier.province = "Antwerp";

    var controller = new SuppliersController(context);

    var result = await controller.UpdateSupplier(1, supplier);

    var okResult = Assert.IsType<OkObjectResult>(result);
    var updatedSupplier = Assert.IsType<Supplier>(okResult.Value);

    Assert.Equal("Antwerp", updatedSupplier.province);
}


[Fact]
public async Task DeleteSupplier_ReturnsOk()
{
    using var context = CreateContext();

    context.Suppliers.Add(CreateSupplier());
    await context.SaveChangesAsync();

    var controller = new SuppliersController(context);

    var result = await controller.DeleteSupplier(1);

    Assert.IsType<OkResult>(result);
    Assert.Empty(context.Suppliers);
}
}
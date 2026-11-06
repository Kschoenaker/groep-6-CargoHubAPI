using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApi.Controllers;
using MyApi.Data;
using MyApi.Models;

namespace MyApi.Tests.Unit;

public class TransfersControllerTests
{
    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var context = new AppDbContext(options);

        context.Locations.AddRange(
            new Location { id = 1, warehouse_id = 1, code = "A.1.0", name = "Row A Rack 1" },
            new Location { id = 2, warehouse_id = 1, code = "A.2.0", name = "Row A Rack 2" });
        context.SaveChanges();

        return context;
    }

    private static Transfer CreateTransfer(int id = 1, int from = 1, int to = 2)
    {
        return new Transfer
        {
            id = id,
            reference = $"TR0000{id}",
            from_location_id = from,
            to_location_id = to,
            transfer_status = "Scheduled"
        };
    }

    [Fact]
    public async Task GetTransfers_ReturnsAllTransfers()
    {
        using var context = CreateContext();
        context.Transfers.AddRange(CreateTransfer(1), CreateTransfer(2));
        await context.SaveChangesAsync();
        var controller = new TransfersController(context);

        var result = await controller.getTransfers();

        var ok = Assert.IsType<OkObjectResult>(result);
        var transfers = Assert.IsType<List<Transfer>>(ok.Value);
        Assert.Equal(2, transfers.Count);
    }

    [Fact]
    public async Task GetTransferById_ReturnsTransfer()
    {
        using var context = CreateContext();
        context.Transfers.Add(CreateTransfer(1));
        await context.SaveChangesAsync();
        var controller = new TransfersController(context);

        var result = await controller.GetTransferById(1);

        var ok = Assert.IsType<OkObjectResult>(result);
        var transfer = Assert.IsType<Transfer>(ok.Value);
        Assert.Equal("TR00001", transfer.reference);
    }

    [Fact]
    public async Task GetTransferById_UnknownId_ReturnsNotFound()
    {
        using var context = CreateContext();
        var controller = new TransfersController(context);

        var result = await controller.GetTransferById(999);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task CreateTransfer_SavesTransfer()
    {
        using var context = CreateContext();
        var controller = new TransfersController(context);

        var result = await controller.CreateTransfer(CreateTransfer());

        Assert.IsType<CreatedAtActionResult>(result);
        Assert.Single(context.Transfers);
    }

    [Fact]
    public async Task CreateTransfer_SameFromAndToLocation_ReturnsBadRequest()
    {
        using var context = CreateContext();
        var controller = new TransfersController(context);

        var result = await controller.CreateTransfer(CreateTransfer(from: 1, to: 1));

        Assert.IsType<BadRequestObjectResult>(result);
        Assert.Empty(context.Transfers);
    }

    [Fact]
    public async Task CreateTransfer_UnknownLocation_ReturnsBadRequest()
    {
        using var context = CreateContext();
        var controller = new TransfersController(context);

        var result = await controller.CreateTransfer(CreateTransfer(from: 1, to: 999));

        Assert.IsType<BadRequestObjectResult>(result);
        Assert.Empty(context.Transfers);
    }

    [Fact]
    public async Task UpdateTransfer_ChangesTransfer()
    {
        using var context = CreateContext();
        context.Transfers.Add(CreateTransfer(1));
        await context.SaveChangesAsync();
        var controller = new TransfersController(context);

        var updated = CreateTransfer(1);
        updated.transfer_status = "Completed";
        var result = await controller.UpdateTransfer(1, updated);

        Assert.IsType<OkObjectResult>(result);
        Assert.Equal("Completed", context.Transfers.Find(1)!.transfer_status);
    }

    [Fact]
    public async Task UpdateTransfer_SameFromAndToLocation_ReturnsBadRequest()
    {
        using var context = CreateContext();
        context.Transfers.Add(CreateTransfer(1));
        await context.SaveChangesAsync();
        var controller = new TransfersController(context);

        var result = await controller.UpdateTransfer(1, CreateTransfer(1, from: 2, to: 2));

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task UpdateTransfer_UnknownLocation_ReturnsBadRequest()
    {
        using var context = CreateContext();
        context.Transfers.Add(CreateTransfer(1));
        await context.SaveChangesAsync();
        var controller = new TransfersController(context);

        var result = await controller.UpdateTransfer(1, CreateTransfer(1, from: 999, to: 2));

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task UpdateTransfer_UnknownId_ReturnsNotFound()
    {
        using var context = CreateContext();
        var controller = new TransfersController(context);

        var result = await controller.UpdateTransfer(999, CreateTransfer());

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task DeleteTransfer_RemovesTransfer()
    {
        using var context = CreateContext();
        context.Transfers.Add(CreateTransfer(1));
        await context.SaveChangesAsync();
        var controller = new TransfersController(context);

        var result = await controller.DeleteTransfer(1);

        Assert.IsType<NoContentResult>(result);
        Assert.Empty(context.Transfers);
    }

    [Fact]
    public async Task DeleteTransfer_UnknownId_ReturnsNotFound()
    {
        using var context = CreateContext();
        var controller = new TransfersController(context);

        var result = await controller.DeleteTransfer(999);

        Assert.IsType<NotFoundResult>(result);
    }
}

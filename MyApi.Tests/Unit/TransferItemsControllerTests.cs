using System.Collections;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApi.Controllers;
using MyApi.Data;
using MyApi.Models;

namespace MyApi.Tests.Unit;

public class TransferItemsControllerTests
{
    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task GetTransferItems_ReturnsOnlyItemsOfTransfer()
    {
        using var context = CreateContext();
        context.TransferItems.AddRange(
            new TransferItem { id = 1, transfer_id = 1, item_id = 10, amount = 5 },
            new TransferItem { id = 2, transfer_id = 1, item_id = 11, amount = 3 },
            new TransferItem { id = 3, transfer_id = 2, item_id = 12, amount = 20 });
        await context.SaveChangesAsync();
        var controller = new TransfersController(context);

        var result = await controller.GetTransferItems(1);

        var ok = Assert.IsType<OkObjectResult>(result);
        var items = Assert.IsAssignableFrom<IList>(ok.Value);
        Assert.Equal(2, items.Count);
    }

    [Fact]
    public async Task GetTransferItems_NoItems_ReturnsEmptyList()
    {
        using var context = CreateContext();
        var controller = new TransfersController(context);

        var result = await controller.GetTransferItems(1);

        var ok = Assert.IsType<OkObjectResult>(result);
        var items = Assert.IsAssignableFrom<IList>(ok.Value);
        Assert.Empty(items);
    }

    [Fact]
    public async Task DeleteTransfer_AlsoRemovesItsItems()
    {
        using var context = CreateContext();
        context.Transfers.Add(new Transfer { id = 1, reference = "TR00001", from_location_id = 1, to_location_id = 2, transfer_status = "Scheduled" });
        context.TransferItems.AddRange(
            new TransferItem { id = 1, transfer_id = 1, item_id = 10, amount = 5 },
            new TransferItem { id = 2, transfer_id = 1, item_id = 11, amount = 3 });
        await context.SaveChangesAsync();
        var controller = new TransfersController(context);

        await controller.DeleteTransfer(1);

        Assert.Empty(context.TransferItems);
    }
}

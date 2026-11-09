using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApi.Controllers;
using MyApi.Data;
using MyApi.Models;

namespace MyApi.Tests;

public class ItemTypeControllerTests
{
    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    private static ItemType CreateItemType(int id, string name)
    {
        return new ItemType
        {
            Id = id,
            Name = name,
            Description = $"Description for {name}",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    [Fact]
    public async Task GetItemTypes_ReturnsAllItemTypes()
    {
        using var context = CreateContext();
        context.itemTypes.AddRange(
            CreateItemType(1, "Fresh"),
            CreateItemType(2, "Frozen"));
        await context.SaveChangesAsync();

        var controller = new ItemTypesController(context);

        var result = await controller.GetItemTypes();

        var okResult = Assert.IsType<OkObjectResult>(result);
        var itemTypes = Assert.IsAssignableFrom<IEnumerable<ItemType>>(okResult.Value);
        Assert.Equal(2, itemTypes.Count());
    }

    [Fact]
    public async Task GetItemTypeById_ReturnsItemTypeOrExpectedError()
    {
        using var context = CreateContext();
        context.itemTypes.Add(CreateItemType(1, "Fresh"));
        await context.SaveChangesAsync();

        var controller = new ItemTypesController(context);

        var foundResult = await controller.GetItemTypeById(1);
        var okResult = Assert.IsType<OkObjectResult>(foundResult);
        Assert.Equal("Fresh", Assert.IsType<ItemType>(okResult.Value).Name);

        Assert.IsType<BadRequestResult>(await controller.GetItemTypeById(0));
        Assert.IsType<NotFoundResult>(await controller.GetItemTypeById(99));
    }

    [Fact]
    public async Task GetItemsByItemTypeId_ReturnsOnlyItemsOfThatType()
    {
        using var context = CreateContext();
        context.itemTypes.AddRange(
            CreateItemType(1, "Fresh"),
            CreateItemType(2, "Frozen"));
        context.item.AddRange(
            new Item { Id = 1, Code = "ITM-1", Item_Type_Id = 1 },
            new Item { Id = 2, Code = "ITM-2", Item_Type_Id = 1 },
            new Item { Id = 3, Code = "ITM-3", Item_Type_Id = 2 });
        await context.SaveChangesAsync();

        var controller = new ItemTypesController(context);

        var result = await controller.GetItemsByItemTypeId(1);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var items = Assert.IsAssignableFrom<IEnumerable<Item>>(okResult.Value);
        Assert.Equal(new[] { 1, 2 }, items.Select(item => item.Id).OrderBy(id => id));

        Assert.IsType<NotFoundResult>(
            await controller.GetItemsByItemTypeId(99));
    }

    [Fact]
    public async Task GetItemLinesByItemTypeId_ReturnsDistinctMatchingItemLines()
    {
        using var context = CreateContext();
        context.itemTypes.AddRange(
            CreateItemType(1, "Fresh"),
            CreateItemType(2, "Frozen"));
        context.itemLines.AddRange(
            new ItemLine { Id = 10, Name = "Produce" },
            new ItemLine { Id = 20, Name = "Meat" });
        context.item.AddRange(
            new Item { Id = 1, Code = "ITM-1", Item_Type_Id = 1, Item_Line_Id = 10 },
            new Item { Id = 2, Code = "ITM-2", Item_Type_Id = 1, Item_Line_Id = 10 },
            new Item { Id = 3, Code = "ITM-3", Item_Type_Id = 2, Item_Line_Id = 20 });
        await context.SaveChangesAsync();

        var controller = new ItemTypesController(context);

        var result = await controller.GetItemLinesByItemTypeId(1);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var itemLines = Assert.IsAssignableFrom<IEnumerable<ItemLine>>(okResult.Value);
        var itemLine = Assert.Single(itemLines);
        Assert.Equal(10, itemLine.Id);
        Assert.Equal("Produce", itemLine.Name);

        Assert.IsType<NotFoundResult>(
            await controller.GetItemLinesByItemTypeId(99));
    }
}

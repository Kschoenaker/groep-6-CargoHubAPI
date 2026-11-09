using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using MyApi.Controllers;
using MyApi.Data;
using MyApi.Models;
using Xunit;

namespace MyApi.Tests;

public class ItemLineControllerTests
{
    // -------------------------------------------------------
    // HELPERS
    // -------------------------------------------------------

    private static ItemLine CreateItemLine(
        int id,
        string name,
        string? description = null)
    {
        return new ItemLine
        {
            Id = id,
            Name = name,
            Description = description,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    // -------------------------------------------------------
    // 1. DATABASE MAPPING
    // -------------------------------------------------------

    [Fact]
    public void ItemLine_IsMappedToCorrectTable()
    {
        using var context = CreateContext();

        var entity = context.Model.FindEntityType(typeof(ItemLine));

        Assert.NotNull(entity);
        Assert.Equal("item_lines", entity!.GetTableName());
    }

    [Fact]
    public void ItemLine_HasCorrectDatabaseColumns()
    {
        using var context = CreateContext();

        var entity = context.Model.FindEntityType(typeof(ItemLine));

        Assert.NotNull(entity);

        var table = StoreObjectIdentifier.Table(
            entity!.GetTableName()!,
            entity.GetSchema());

        Assert.Equal("id", entity.FindProperty(nameof(ItemLine.Id))!
            .GetColumnName(table));

        Assert.Equal("name", entity.FindProperty(nameof(ItemLine.Name))!
            .GetColumnName(table));

        Assert.Equal("description", entity.FindProperty(nameof(ItemLine.Description))!
            .GetColumnName(table));

        Assert.Equal("created_at", entity.FindProperty(nameof(ItemLine.CreatedAt))!
            .GetColumnName(table));

        Assert.Equal("updated_at", entity.FindProperty(nameof(ItemLine.UpdatedAt))!
            .GetColumnName(table));
    }

    // -------------------------------------------------------
    // 2. GET ALL ITEM LINES
    // -------------------------------------------------------

    [Fact]
    public async Task GetItemLines_ReturnsAllItemLines()
    {
        using var context = CreateContext();

        context.itemLines.AddRange(
            CreateItemLine(1, "Electronics"),
            CreateItemLine(2, "Furniture")
        );

        await context.SaveChangesAsync();

        var controller = new ItemLinesController(context);

        var result = await controller.GetItemLines();

        var okResult = Assert.IsType<OkObjectResult>(result);
        var itemLines = Assert.IsAssignableFrom<IEnumerable<ItemLine>>(
            okResult.Value);

        Assert.Equal(2, itemLines.Count());
    }

    [Fact]
    public async Task GetItemLines_ReturnsEmptyList_WhenNoItemLinesExist()
    {
        using var context = CreateContext();

        var controller = new ItemLinesController(context);

        var result = await controller.GetItemLines();

        var okResult = Assert.IsType<OkObjectResult>(result);
        var itemLines = Assert.IsAssignableFrom<IEnumerable<ItemLine>>(
            okResult.Value);

        Assert.Empty(itemLines);
    }

    // -------------------------------------------------------
    // 3. GET ITEM LINE BY ID
    // -------------------------------------------------------

    [Fact]
    public async Task GetItemLineById_ReturnsItemLine_WhenItExists()
    {
        using var context = CreateContext();

        context.itemLines.Add(CreateItemLine(1, "Electronics"));
        await context.SaveChangesAsync();

        var controller = new ItemLinesController(context);

        var result = await controller.GetItemLineById(1);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var itemLine = Assert.IsType<ItemLine>(okResult.Value);

        Assert.Equal(1, itemLine.Id);
        Assert.Equal("Electronics", itemLine.Name);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task GetItemLineById_ReturnsBadRequest_WhenIdIsInvalid(int id)
    {
        using var context = CreateContext();

        var controller = new ItemLinesController(context);

        var result = await controller.GetItemLineById(id);

        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task GetItemLineById_ReturnsNotFound_WhenItemLineDoesNotExist()
    {
        using var context = CreateContext();

        var controller = new ItemLinesController(context);

        var result = await controller.GetItemLineById(99);

        Assert.IsType<NotFoundResult>(result);
    }

    // -------------------------------------------------------
    // 4. CREATE ITEM LINE
    // -------------------------------------------------------

    [Fact]
    public async Task Create_SavesItemLine()
    {
        using var context = CreateContext();

        var controller = new ItemLinesController(context);

        var newItemLine = CreateItemLine(1, "Electronics", "Electronic products");

        var result = await controller.Create(newItemLine);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedItemLine = Assert.IsType<ItemLine>(okResult.Value);

        Assert.Equal("Electronics", returnedItemLine.Name);

        var savedItemLine = await context.itemLines.FindAsync(1);

        Assert.NotNull(savedItemLine);
        Assert.Equal("Electronics", savedItemLine.Name);
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenItemLineIsNull()
    {
        using var context = CreateContext();

        var controller = new ItemLinesController(context);

        var result = await controller.Create(null!);

        Assert.IsType<BadRequestResult>(result);
        Assert.Empty(context.itemLines);
    }

    // -------------------------------------------------------
    // 5. UPDATE ITEM LINE
    // -------------------------------------------------------

    [Fact]
    public async Task Update_ChangesExistingItemLine()
    {
        using var context = CreateContext();

        context.itemLines.Add(CreateItemLine(1, "Electronics"));
        await context.SaveChangesAsync();

        var controller = new ItemLinesController(context);

        var updatedItemLine = CreateItemLine(
            1,
            "Updated Electronics",
            "Updated description"
        );

        var result = await controller.Update(1, updatedItemLine);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedItemLine = Assert.IsType<ItemLine>(okResult.Value);

        Assert.Equal("Updated Electronics", returnedItemLine.Name);
        Assert.Equal("Updated description", returnedItemLine.Description);

        var savedItemLine = await context.itemLines.FindAsync(1);

        Assert.Equal("Updated Electronics", savedItemLine!.Name);
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(-1, -1)]
    [InlineData(1, 2)]
    public async Task Update_ReturnsBadRequest_WhenIdIsInvalidOrDifferent(
        int id,
        int itemLineId)
    {
        using var context = CreateContext();

        var controller = new ItemLinesController(context);

        var updatedItemLine = CreateItemLine(itemLineId, "Updated");

        var result = await controller.Update(id, updatedItemLine);

        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task Update_ReturnsBadRequest_WhenItemLineIsNull()
    {
        using var context = CreateContext();

        var controller = new ItemLinesController(context);

        var result = await controller.Update(1, null!);

        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task Update_ReturnsNotFound_WhenItemLineDoesNotExist()
    {
        using var context = CreateContext();

        var controller = new ItemLinesController(context);

        var updatedItemLine = CreateItemLine(99, "Updated");

        var result = await controller.Update(99, updatedItemLine);

        Assert.IsType<NotFoundResult>(result);
    }

    // -------------------------------------------------------
    // 6. DELETE ITEM LINE
    // -------------------------------------------------------

    [Fact]
    public async Task Delete_RemovesItemLine()
    {
        using var context = CreateContext();

        context.itemLines.Add(CreateItemLine(1, "Electronics"));
        await context.SaveChangesAsync();

        var controller = new ItemLinesController(context);

        var result = await controller.Delete(1);

        Assert.IsType<NoContentResult>(result);

        var deletedItemLine = await context.itemLines.FindAsync(1);

        Assert.Null(deletedItemLine);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Delete_ReturnsBadRequest_WhenIdIsInvalid(int id)
    {
        using var context = CreateContext();

        var controller = new ItemLinesController(context);

        var result = await controller.Delete(id);

        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task Delete_ReturnsNotFound_WhenItemLineDoesNotExist()
    {
        using var context = CreateContext();

        var controller = new ItemLinesController(context);

        var result = await controller.Delete(99);

        Assert.IsType<NotFoundResult>(result);
    }

    // -------------------------------------------------------
    // 7. GET ITEMS BY ITEM LINE ID
    // -------------------------------------------------------

    [Fact]
    public async Task GetItemsByItemLineId_ReturnsMatchingItems()
    {
        using var context = CreateContext();

        context.itemLines.AddRange(
            CreateItemLine(1, "Electronics"),
            CreateItemLine(2, "Furniture")
        );

        // Items are linked to item line 1.
        context.item.AddRange(
            new Item { Item_Line_Id = 1 },
            new Item { Item_Line_Id = 1 },
            new Item { Item_Line_Id = 2 }
        );

        await context.SaveChangesAsync();

        var controller = new ItemLinesController(context);

        var result = await controller.GetItemsByItemLineId(1);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var items = Assert.IsAssignableFrom<IEnumerable<Item>>(
            okResult.Value);

        Assert.Equal(2, items.Count());
        Assert.All(items, item => Assert.Equal(1, item.Item_Line_Id));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task GetItemsByItemLineId_ReturnsBadRequest_WhenIdIsInvalid(int id)
    {
        using var context = CreateContext();

        var controller = new ItemLinesController(context);

        var result = await controller.GetItemsByItemLineId(id);

        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task GetItemsByItemLineId_ReturnsNotFound_WhenItemLineDoesNotExist()
    {
        using var context = CreateContext();

        var controller = new ItemLinesController(context);

        var result = await controller.GetItemsByItemLineId(99);

        Assert.IsType<NotFoundResult>(result);
    }
}
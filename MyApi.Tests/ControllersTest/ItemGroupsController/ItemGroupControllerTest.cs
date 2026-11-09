using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using MyApi.Data;
using MyApi.Models;
using Xunit;

namespace MyApi.Tests;

public class ItemGroupControllerTests
{
    // TODO - Minimaal nog testen:
    // - GetItemGroups geeft alle groepen terug.
    // - GetItemGroupById geeft de groep terug, BadRequest bij een ongeldig ID en NotFound bij een onbekend ID.
    // - Update werkt naam en beschrijving bij; test ook een afwijkend ID en een onbekende groep.
    // - Delete verwijdert de groep; test ook een ongeldig ID en een onbekende groep.
    // - Implementeer Create eerst; test daarna opslaan en het resultaat.

    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public void ItemGroupMapsToDatabaseTableAndColumns()
    {
        using var context = CreateContext();
        var entity = context.Model.FindEntityType(typeof(ItemGroup));
        var table = StoreObjectIdentifier.Table("item_groups", null);

        Assert.NotNull(entity);
        Assert.Equal("item_groups", entity.GetTableName());
        Assert.Equal("id", entity.FindProperty(nameof(ItemGroup.Id))!.GetColumnName(table));
        Assert.Equal("name", entity.FindProperty(nameof(ItemGroup.Name))!.GetColumnName(table));
        Assert.Equal("description", entity.FindProperty(nameof(ItemGroup.Description))!.GetColumnName(table));
        Assert.Equal("created_at", entity.FindProperty(nameof(ItemGroup.CreatedAt))!.GetColumnName(table));
        Assert.Equal("updated_at", entity.FindProperty(nameof(ItemGroup.UpdatedAt))!.GetColumnName(table));
    }

}

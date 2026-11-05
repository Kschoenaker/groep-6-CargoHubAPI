using Microsoft.EntityFrameworkCore;
using MyApi.Models;

namespace MyApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Warehouse> warehouses { get; set; }
    public DbSet<Item> item { get; set; }
    public DbSet<ItemType> itemTypes { get; set; }
    public DbSet<ItemGroup> itemGroups { get; set; }
    public DbSet<ItemLine> itemLines  { get; set; }

    


}
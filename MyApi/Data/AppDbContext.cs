
using Microsoft.EntityFrameworkCore;
using MyApi.Models;

namespace MyApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Client> Clients { get; set; }
    // public DbSet<Inventory> Inventories { get; set; }
    // public DbSet<ItemGroup> ItemGroups { get; set; }
    // public DbSet<ItemLine> ItemLines { get; set; }
    // public DbSet<ItemType> ItemTypes { get; set; }
    // public DbSet<Item> Items { get; set; }
    // public DbSet<Location> Locations { get; set; }
    // public DbSet<OrderItem> OrderItems { get; set; }
    // public DbSet<Order> Orders { get; set; }
    // public DbSet<ShipmentItem> ShipmentItems { get; set; }
    // public DbSet<Shipment> Shipments { get; set; }
    // public DbSet<Supplier> Suppliers { get; set; }
    // public DbSet<TransferItem> TransferItems { get; set; }
    // public DbSet<Transfer> Transfers { get; set; }
    // public DbSet<User> Users { get; set; }
    public DbSet<Warehouse> Warehouses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Client>().ToTable("clients");
        // modelBuilder.Entity<Inventory>().ToTable("inventories");
        // modelBuilder.Entity<ItemGroup>().ToTable("item_groups");
        // modelBuilder.Entity<ItemLine>().ToTable("item_lines");
        // modelBuilder.Entity<ItemType>().ToTable("item_types");
        // modelBuilder.Entity<Item>().ToTable("items");
        // modelBuilder.Entity<Location>().ToTable("locations");
        // modelBuilder.Entity<OrderItem>().ToTable("order_items");
        // modelBuilder.Entity<Order>().ToTable("orders");
        // modelBuilder.Entity<ShipmentItem>().ToTable("shipment_items");
        // modelBuilder.Entity<Shipment>().ToTable("shipments");
        // modelBuilder.Entity<Supplier>().ToTable("suppliers");
        // modelBuilder.Entity<TransferItem>().ToTable("transfer_items");
        // modelBuilder.Entity<Transfer>().ToTable("tranfers");
        // modelBuilder.Entity<User>().ToTable("users");
        modelBuilder.Entity<Warehouse>().ToTable("warehouses");

        modelBuilder.Entity<Client>()
            .Property(x => x.id).HasColumnName("id");

        // modelBuilder.Entity<ItemGroup>()
        //     .Property(x => x.Id).HasColumnName("id");

        // modelBuilder.Entity<ItemLine>()
        //     .Property(x => x.Id).HasColumnName("id");

        // modelBuilder.Entity<ItemType>()
        //     .Property(x => x.Id).HasColumnName("id");

        // modelBuilder.Entity<Item>()
        //     .Property(x => x.Id).HasColumnName("id");

        // modelBuilder.Entity<Location>()
        //     .Property(x => x.Id).HasColumnName("id");

        // modelBuilder.Entity<OrderItem>()
        //     .Property(x => x.Id).HasColumnName("id");

        // modelBuilder.Entity<Order>()
        //     .Property(x => x.Id).HasColumnName("id");

        // modelBuilder.Entity<ShipmentItem>()
        //     .Property(x => x.Id).HasColumnName("id");

        // modelBuilder.Entity<Shipment>()
        //     .Property(x => x.Id).HasColumnName("id");

        // modelBuilder.Entity<Supplier>()
        //     .Property(x => x.Id).HasColumnName("id");

        // modelBuilder.Entity<TransferItem>()
        //     .Property(x => x.Id).HasColumnName("id");

        // modelBuilder.Entity<Transfer>()
        //     .Property(x => x.Id).HasColumnName("id");

        // modelBuilder.Entity<User>()
        //     .Property(x => x.Id).HasColumnName("id");

        modelBuilder.Entity<Warehouse>()
            .Property(x => x.id).HasColumnName("id");

        // modelBuilder.Entity<Inventory>()
        //     .HasKey(x => new { x.ItemId, x.LocationId });

        // modelBuilder.Entity<Inventory>()
        //     .Property(x => x.ItemId).HasColumnName("item_id");

        // modelBuilder.Entity<Inventory>()
        //     .Property(x => x.LocationId).HasColumnName("location_id");

        base.OnModelCreating(modelBuilder);
    }
}
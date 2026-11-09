
using Microsoft.EntityFrameworkCore;
using MyApi.Models;

namespace MyApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }


    public DbSet<Item> item { get; set; }
    public DbSet<ItemType> itemTypes { get; set; }
    public DbSet<ItemGroup> itemGroups { get; set; }
    public DbSet<ItemLine> itemLines { get; set; }

    public DbSet<Client> Clients { get; set; }
    public DbSet<Inventory> Inventories { get; set; }
    // public DbSet<ItemGroup> ItemGroups { get; set; }
    // public DbSet<ItemLine> ItemLines { get; set; }
    // public DbSet<ItemType> ItemTypes { get; set; }
    // public DbSet<Item> Items { get; set; }
    public DbSet<Location> Locations { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<ShipmentItems> ShipmentItems { get; set; }
    public DbSet<Shipments> Shipments { get; set; }
    public DbSet<Supplier> Suppliers { get; set; }
    public DbSet<TransferItem> TransferItems { get; set; }
    public DbSet<Transfer> Transfers { get; set; }
    // public DbSet<User> Users { get; set; }
    public DbSet<Warehouse> Warehouses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Item>(entity =>
        {
            entity.ToTable("items");
            entity.HasKey(item => item.Id);

            entity.Property(item => item.Id).HasColumnName("id");
            entity.Property(item => item.Code).HasColumnName("code");
            entity.Property(item => item.Description).HasColumnName("description");
            entity.Property(item => item.Barcode).HasColumnName("barcode");
            entity.Property(item => item.Model_Number).HasColumnName("model_number");
            entity.Property(item => item.Commodity_Code).HasColumnName("commodity_code");
            entity.Property(item => item.Unit_Weight).HasColumnName("unit_weight");
            entity.Property(item => item.Item_Line_Id).HasColumnName("item_line_id");
            entity.Property(item => item.Item_Group_Id).HasColumnName("item_group_id");
            entity.Property(item => item.Item_Type_Id).HasColumnName("item_type_id");
            entity.Property(item => item.Min_Purchase_Qty).HasColumnName("min_purchase_qty");
            entity.Property(item => item.Case_Size).HasColumnName("case_size");
            entity.Property(item => item.Packaging_Type).HasColumnName("packaging_type");
            entity.Property(item => item.Order_Multiple).HasColumnName("order_multiple");
            entity.Property(item => item.Supplier_Id).HasColumnName("supplier_id");
            entity.Property(item => item.Supplier_Sku).HasColumnName("supplier_sku");
            entity.Property(item => item.Created_At).HasColumnName("created_at");
            entity.Property(item => item.Updated_At).HasColumnName("updated_at");
        });

        modelBuilder.Entity<ItemGroup>(entity =>
        {
            entity.ToTable("item_groups");
            entity.HasKey(itemGroup => itemGroup.Id);

            entity.Property(itemGroup => itemGroup.Id).HasColumnName("id");
            entity.Property(itemGroup => itemGroup.Name).HasColumnName("name");
            entity.Property(itemGroup => itemGroup.Description).HasColumnName("description");
            entity.Property(itemGroup => itemGroup.CreatedAt).HasColumnName("created_at");
            entity.Property(itemGroup => itemGroup.UpdatedAt).HasColumnName("updated_at");
        });

        modelBuilder.Entity<ItemLine>(entity =>
        {
            entity.ToTable("item_lines");
            entity.HasKey(itemLine => itemLine.Id);

            entity.Property(itemLine => itemLine.Id).HasColumnName("id");
            entity.Property(itemLine => itemLine.Name).HasColumnName("name");
            entity.Property(itemLine => itemLine.Description).HasColumnName("description");
            entity.Property(itemLine => itemLine.CreatedAt).HasColumnName("created_at");
            entity.Property(itemLine => itemLine.UpdatedAt).HasColumnName("updated_at");
        });

        modelBuilder.Entity<Client>().ToTable("clients");
        modelBuilder.Entity<Inventory>().ToTable("inventories");
        modelBuilder.Entity<Inventory>()
            .HasKey(x => new { x.item_id, x.location_id });
        // modelBuilder.Entity<ItemType>().ToTable("item_types");
        // modelBuilder.Entity<Item>().ToTable("items");
        modelBuilder.Entity<Location>().ToTable("locations");
        modelBuilder.Entity<OrderItem>().ToTable("order_items");
        modelBuilder.Entity<Order>().ToTable("orders");
        modelBuilder.Entity<ShipmentItems>().ToTable("shipment_items");
        modelBuilder.Entity<Shipments>().ToTable("shipments");
        modelBuilder.Entity<Supplier>().ToTable("suppliers");
        modelBuilder.Entity<TransferItem>().ToTable("transfer_items");
        modelBuilder.Entity<Transfer>().ToTable("transfers");
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

        modelBuilder.Entity<Location>()
            .Property(x => x.id).HasColumnName("id");

        modelBuilder.Entity<OrderItem>()
            .Property(x => x.id).HasColumnName("id");

        modelBuilder.Entity<Order>()
            .Property(x => x.id).HasColumnName("id");

        modelBuilder.Entity<ShipmentItems>()
            .Property(x => x.id).HasColumnName("id");

        modelBuilder.Entity<Shipments>()
            .Property(x => x.id).HasColumnName("id");

        modelBuilder.Entity<Supplier>()
            .Property(x => x.id).HasColumnName("id");

        modelBuilder.Entity<TransferItem>()
            .Property(x => x.id).HasColumnName("id");

        modelBuilder.Entity<Transfer>()
            .Property(x => x.id).HasColumnName("id");

        // modelBuilder.Entity<User>()
        //     .Property(x => x.Id).HasColumnName("id");

        modelBuilder.Entity<Warehouse>()
            .Property(x => x.id).HasColumnName("id");

        modelBuilder.Entity<Inventory>()
            .HasKey(x => new { x.item_id, x.location_id });

        modelBuilder.Entity<Inventory>()
            .Property(x => x.item_id).HasColumnName("item_id");

        modelBuilder.Entity<Inventory>()
            .Property(x => x.location_id).HasColumnName("location_id");

        base.OnModelCreating(modelBuilder);
    }
}

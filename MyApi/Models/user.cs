namespace MyApi.Models;

public class User
{
    public string ApiKey { get; set; }
    public string App { get; set; }

    // Warehouses
    public bool GetWarehouses { get; set; }
    public bool PostWarehouses { get; set; }
    public bool PutWarehouses { get; set; }
    public bool DeleteWarehouses { get; set; }

    // Locations
    public bool GetLocations { get; set; }
    public bool PostLocations { get; set; }
    public bool PutLocations { get; set; }
    public bool DeleteLocations { get; set; }

    // Transfers
    public bool GetTransfers { get; set; }
    public bool PostTransfers { get; set; }
    public bool PutTransfers { get; set; }
    public bool DeleteTransfers { get; set; }

    // Items
    public bool GetItems { get; set; }
    public bool PostItems { get; set; }
    public bool PutItems { get; set; }
    public bool DeleteItems { get; set; }

    // Item_lines
    public bool GetItemLines { get; set; }
    public bool PostItemLines { get; set; }
    public bool PutItemLines { get; set; }
    public bool DeleteItemLines { get; set; }

    // Item_groups
    public bool GetItemGroups { get; set; }
    public bool PostItemGroups { get; set; }
    public bool PutItemGroups { get; set; }
    public bool DeleteItemGroups { get; set; }

    // Item_types
    public bool GetItemTypes { get; set; }
    public bool PostItemTypes { get; set; }
    public bool PutItemTypes { get; set; }
    public bool DeleteItemTypes { get; set; }

    // Suppliers
    public bool GetSuppliers { get; set; }
    public bool PostSuppliers { get; set; }
    public bool PutSuppliers { get; set; }
    public bool DeleteSuppliers { get; set; }

    // Orders
    public bool GetOrders { get; set; }
    public bool PostOrders { get; set; }
    public bool PutOrders { get; set; }
    public bool DeleteOrders { get; set; }

    // Clients
    public bool GetClients { get; set; }
    public bool PostClients { get; set; }
    public bool PutClients { get; set; }
    public bool DeleteClients { get; set; }

    // Shipments
    public bool GetShipments { get; set; }
    public bool PostShipments { get; set; }
    public bool PutShipments { get; set; }
    public bool DeleteShipments { get; set; }

    // Inventories
    public bool GetInventories { get; set; }
    public bool PostInventories { get; set; }
    public bool PutInventories { get; set; }
    public bool DeleteInventories { get; set; }
}

using System;
using System.Collections.Generic;

namespace Warehouse.Data;

public partial class Product
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public int? CategoryId { get; set; }

    public string Unit { get; set; } = null!;

    public decimal PurchasePrice { get; set; }

    public decimal SellingPrice { get; set; }

    public int? MinStockLevel { get; set; }

    public int? MaxStockLevel { get; set; }

    public string? StorageConditions { get; set; }

    public string? Barcode { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Category? Category { get; set; }

    public virtual ICollection<CustomerOrderItem> CustomerOrderItems { get; set; } = new List<CustomerOrderItem>();

    public virtual ICollection<InventoryDiscrepancy> InventoryDiscrepancies { get; set; } = new List<InventoryDiscrepancy>();

    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();

    public virtual ICollection<Stock> Stocks { get; set; } = new List<Stock>();

    public virtual ICollection<SupplyOrderItem> SupplyOrderItems { get; set; } = new List<SupplyOrderItem>();

    public virtual ICollection<TransferItem> TransferItems { get; set; } = new List<TransferItem>();
}

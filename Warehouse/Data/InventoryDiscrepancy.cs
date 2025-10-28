using System;
using System.Collections.Generic;

namespace Warehouse.Data;

public partial class InventoryDiscrepancy
{
    public int Id { get; set; }

    public int InventoryId { get; set; }

    public int ProductId { get; set; }

    public int ExpectedQuantity { get; set; }

    public int ActualQuantity { get; set; }

    public int? Difference { get; set; }

    public string? Notes { get; set; }

    public virtual Inventory Inventory { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}

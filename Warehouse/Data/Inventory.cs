using System;
using System.Collections.Generic;

namespace Warehouse.Data;

public partial class Inventory
{
    public int Id { get; set; }

    public string InventoryNumber { get; set; } = null!;

    public DateOnly InventoryDate { get; set; }

    public string? Location { get; set; }

    public string? Status { get; set; }

    public int? CreatedBy { get; set; }

    public string? Notes { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual User? CreatedByNavigation { get; set; }

    public virtual ICollection<InventoryDiscrepancy> InventoryDiscrepancies { get; set; } = new List<InventoryDiscrepancy>();
}

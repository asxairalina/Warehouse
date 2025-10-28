using System;
using System.Collections.Generic;

namespace Warehouse.Data;

public partial class SupplyOrderItem
{
    public int Id { get; set; }

    public int SupplyOrderId { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal? TotalPrice { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual SupplyOrder SupplyOrder { get; set; } = null!;
}

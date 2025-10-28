using System;
using System.Collections.Generic;

namespace Warehouse.Data;

public partial class CustomerOrderItem
{
    public int Id { get; set; }

    public int CustomerOrderId { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal? DiscountRate { get; set; }

    public decimal? TotalPrice { get; set; }

    public virtual CustomerOrder CustomerOrder { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}

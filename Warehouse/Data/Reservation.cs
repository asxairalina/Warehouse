using System;
using System.Collections.Generic;

namespace Warehouse.Data;

public partial class Reservation
{
    public int Id { get; set; }

    public int CustomerOrderId { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public DateTime? ReservedUntil { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual CustomerOrder CustomerOrder { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}

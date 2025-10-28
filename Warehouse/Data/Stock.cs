using System;
using System.Collections.Generic;

namespace Warehouse.Data;

public partial class Stock
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public string? Location { get; set; }

    public string? BatchNumber { get; set; }

    public DateOnly? ExpirationDate { get; set; }

    public DateTime? LastUpdated { get; set; }

    public virtual Product Product { get; set; } = null!;
}

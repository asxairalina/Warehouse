using System;
using System.Collections.Generic;

namespace Warehouse.Data;

public partial class TransferItem
{
    public int Id { get; set; }

    public int TransferId { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual Transfer Transfer { get; set; } = null!;
}

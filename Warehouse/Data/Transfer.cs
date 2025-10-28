using System;
using System.Collections.Generic;

namespace Warehouse.Data;

public partial class Transfer
{
    public int Id { get; set; }

    public string TransferNumber { get; set; } = null!;

    public string FromLocation { get; set; } = null!;

    public string ToLocation { get; set; } = null!;

    public DateOnly TransferDate { get; set; }

    public string? Status { get; set; }

    public int? CreatedBy { get; set; }

    public string? Notes { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual User? CreatedByNavigation { get; set; }

    public virtual ICollection<TransferItem> TransferItems { get; set; } = new List<TransferItem>();
}

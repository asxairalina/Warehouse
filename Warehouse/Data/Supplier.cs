using System;
using System.Collections.Generic;

namespace Warehouse.Data;

public partial class Supplier
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? ContactPerson { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public string? Address { get; set; }

    public string? Inn { get; set; }

    public string? BankDetails { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<SupplyOrder> SupplyOrders { get; set; } = new List<SupplyOrder>();
}

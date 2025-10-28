using System;
using System.Collections.Generic;

namespace Warehouse.Data;

public partial class CustomerOrder
{
    public int Id { get; set; }

    public string OrderNumber { get; set; } = null!;

    public int? CustomerId { get; set; }

    public DateOnly OrderDate { get; set; }

    public string? Status { get; set; }

    public decimal? TotalAmount { get; set; }

    public decimal? DiscountAmount { get; set; }

    public decimal? FinalAmount { get; set; }

    public int? CreatedBy { get; set; }

    public string? Notes { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual User? CreatedByNavigation { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual ICollection<CustomerOrderItem> CustomerOrderItems { get; set; } = new List<CustomerOrderItem>();

    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}

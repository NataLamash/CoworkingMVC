using System;
using System.Collections.Generic;

namespace CoworkingDomain.Model;

public partial class Payment : Entity
{

    public int BookingId { get; set; }

    public decimal Amount { get; set; }

    public string PaymentMethod { get; set; } = null!;

    public DateTime PaymentDate { get; set; }

    public string Status { get; set; } = null!;

    public virtual Booking Booking { get; set; } = null!;
}

using System;
using System.Collections.Generic;

namespace CoworkingDomain.Model;

public partial class BookingsFacility : Entity
{
    public int BookingId { get; set; }

    public int FacilityId { get; set; }

    public decimal Price { get; set; }

    public virtual Booking Booking { get; set; } = null!;

    public virtual Facility Facility { get; set; } = null!;
}

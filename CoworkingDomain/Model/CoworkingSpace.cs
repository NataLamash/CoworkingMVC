using System;
using System.Collections.Generic;

namespace CoworkingDomain.Model;

public partial class CoworkingSpace : Entity
{

    public string Name { get; set; } = null!;

    public string Location { get; set; } = null!;

    public string? Description { get; set; }

    public int Capacity { get; set; }

    public decimal HourlyRate { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ICollection<CoworkingFacilityPrice> CoworkingFacilityPrices { get; set; } = new List<CoworkingFacilityPrice>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
}

using System;
using System.Collections.Generic;

namespace CoworkingDomain.Model;

public partial class Facility : Entity
{

    public string Name { get; set; } = null!;

    public virtual ICollection<BookingsFacility> BookingsFacilities { get; set; } = new List<BookingsFacility>();

    public virtual ICollection<CoworkingFacilityPrice> CoworkingFacilityPrices { get; set; } = new List<CoworkingFacilityPrice>();
}

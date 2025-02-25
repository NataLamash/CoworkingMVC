using System;
using System.Collections.Generic;

namespace CoworkingDomain.Model;

public partial class CoworkingFacilityPrice : Entity
{
    public int CoworkingSpaceId { get; set; }

    public int FacilityId { get; set; }

    public decimal Price { get; set; }

    public virtual CoworkingSpace CoworkingSpace { get; set; } = null!;

    public virtual Facility Facility { get; set; } = null!;
}

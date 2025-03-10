using System;
using System.Collections.Generic;

namespace CoworkingDomain.Model;

public partial class Review : Entity
{

    public string? UserId { get; set; }

    public int CoworkingSpaceId { get; set; }

    public int BookingId { get; set; }

    public int Rating { get; set; }

    public string? Comment { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Booking Booking { get; set; } = null!;

    public virtual CoworkingSpace CoworkingSpace { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}

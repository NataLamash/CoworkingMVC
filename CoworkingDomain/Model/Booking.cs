using System;
using System.Collections.Generic;

namespace CoworkingDomain.Model;

public partial class Booking : Entity
{

    public string? UserId { get; set; }

    public int CoworkingSpaceId { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public int Duration { get; set; }

    public decimal TotalPrice { get; set; }

    public string Status { get; set; } = null!;

    public virtual ICollection<BookingsFacility> BookingsFacilities { get; set; } = new List<BookingsFacility>();

    public virtual CoworkingSpace CoworkingSpace { get; set; } = null!;

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual User User { get; set; } = null!;
}

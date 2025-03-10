using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace CoworkingDomain.Model;

public partial class User : IdentityUser
{
    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
}

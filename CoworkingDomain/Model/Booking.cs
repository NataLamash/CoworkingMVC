using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoworkingDomain.Model
{
    [Table("bookings")]
    public partial class Booking : Entity
    {
        // Primary key inherited from Entity.Id, mapped in Entity

        [Column("user_id")]
        public string? UserId { get; set; }

        [Column("coworking_space_id")]
        public int CoworkingSpaceId { get; set; }

        [Column("start_time")]
        public DateTime StartTime { get; set; }

        [Column("end_time")]
        public DateTime EndTime { get; set; }

        [Column("duration")]
        public int Duration { get; set; }

        [Column("total_price")]
        public decimal TotalPrice { get; set; }

        [Column("status")]
        public string Status { get; set; } = null!;

        [ForeignKey(nameof(CoworkingSpaceId))]
        [InverseProperty(nameof(CoworkingSpace.Bookings))]
        public virtual CoworkingSpace CoworkingSpace { get; set; } = null!;

        [ForeignKey(nameof(UserId))]
        [InverseProperty(nameof(User.Bookings))]
        public virtual User User { get; set; } = null!;

        [InverseProperty(nameof(BookingsFacility.Booking))]
        public virtual ICollection<BookingsFacility> BookingsFacilities { get; set; } = new List<BookingsFacility>();

        [InverseProperty(nameof(Payment.Booking))]
        public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

        [InverseProperty(nameof(Review.Booking))]
        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}

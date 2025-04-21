using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoworkingDomain.Model
{
    [Table("reviews")]
    public partial class Review : Entity
    {
        // Id успадковується з Entity і мапиться на колонку "id"

        [Column("user_id")]
        public string? UserId { get; set; }

        [Column("coworking_space_id")]
        public int CoworkingSpaceId { get; set; }

        [Column("booking_id")]
        public int BookingId { get; set; }

        [Column("rating")]
        public int Rating { get; set; }

        [Column("comment")]
        public string? Comment { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [ForeignKey(nameof(BookingId))]
        [InverseProperty(nameof(Booking.Reviews))]
        public virtual Booking Booking { get; set; } = null!;

        [ForeignKey(nameof(CoworkingSpaceId))]
        [InverseProperty(nameof(CoworkingSpace.Reviews))]
        public virtual CoworkingSpace CoworkingSpace { get; set; } = null!;

        [ForeignKey(nameof(UserId))]
        [InverseProperty(nameof(User.Reviews))]
        public virtual User User { get; set; } = null!;
    }
}

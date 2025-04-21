using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoworkingDomain.Model
{
    [Table("payments")]
    public partial class Payment : Entity
    {
        // Id успадковується з Entity і мапиться на колонку "id"

        [Column("booking_id")]
        public int BookingId { get; set; }

        [Column("amount")]
        public decimal Amount { get; set; }

        [Column("payment_method")]
        public string PaymentMethod { get; set; } = null!;

        [Column("payment_date")]
        public DateTime PaymentDate { get; set; }

        [Column("status")]
        public string Status { get; set; } = null!;

        [ForeignKey(nameof(BookingId))]
        [InverseProperty(nameof(Booking.Payments))]
        public virtual Booking Booking { get; set; } = null!;
    }
}

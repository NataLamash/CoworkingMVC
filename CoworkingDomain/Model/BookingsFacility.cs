using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoworkingDomain.Model
{
    [Table("bookings_facilities")]
    public partial class BookingsFacility : Entity
    {
        // — успадковуємо Id з Entity, воно вже мапиться на колонку "id"

        [Column("booking_id")]
        public int BookingId { get; set; }

        [Column("facility_id")]
        public int FacilityId { get; set; }

        [Column("price")]
        public decimal Price { get; set; }

        [ForeignKey(nameof(BookingId))]
        [InverseProperty(nameof(Booking.BookingsFacilities))]
        public virtual Booking Booking { get; set; } = null!;

        [ForeignKey(nameof(FacilityId))]
        [InverseProperty(nameof(Facility.BookingsFacilities))]
        public virtual Facility Facility { get; set; } = null!;
    }
}

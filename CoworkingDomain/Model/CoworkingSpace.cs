using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;


namespace CoworkingDomain.Model
{
    [Table("coworking_spaces")]
    public partial class CoworkingSpace : Entity
    {
        // PK — успадковується з Entity.Id та мапиться на колонку "id"

        [Column("name")]
        public string Name { get; set; } = null!;

        [Column("location")]
        public string Location { get; set; } = null!;

        [Column("description")]
        public string? Description { get; set; }

        [Column("capacity")]
        public int Capacity { get; set; }

        [Column("hourly_rate")]
        public decimal HourlyRate { get; set; }

        // Навігаційні властивості — на самому minimal достатньо InverseProperty,
        // але EF зрозуміє їх і без нього, бо ФКі конвекцією видно з імен
        [InverseProperty(nameof(Booking.CoworkingSpace))]
        public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

        [InverseProperty(nameof(CoworkingFacilityPrice.CoworkingSpace))]
        public virtual ICollection<CoworkingFacilityPrice> CoworkingFacilityPrices { get; set; } = new List<CoworkingFacilityPrice>();

        [InverseProperty(nameof(Review.CoworkingSpace))]
        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

        [InverseProperty(nameof(CoworkingSpaceImage.CoworkingSpace))]
        public virtual ICollection<CoworkingSpaceImage> CoworkingSpaceImages { get; set; }
           = new List<CoworkingSpaceImage>();
    }
}

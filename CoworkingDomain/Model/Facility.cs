using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoworkingDomain.Model
{
    [Table("facilities")]
    public partial class Facility : Entity
    {
        // Id успадковується з Entity і мапиться на колонку "id"

        [Column("name")]
        public string Name { get; set; } = null!;

        [InverseProperty(nameof(BookingsFacility.Facility))]
        public virtual ICollection<BookingsFacility> BookingsFacilities { get; set; } = new List<BookingsFacility>();

        [InverseProperty(nameof(CoworkingFacilityPrice.Facility))]
        public virtual ICollection<CoworkingFacilityPrice> CoworkingFacilityPrices { get; set; } = new List<CoworkingFacilityPrice>();
    }
}

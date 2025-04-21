using System.ComponentModel.DataAnnotations.Schema;


namespace CoworkingDomain.Model
{
    [Table("coworking_facility_prices")]
    public partial class CoworkingFacilityPrice : Entity
    {
        // ← ця властивість "ховає" базову Id
        [NotMapped]
        public override int Id { get; set; }

        [Column("coworking_space_id")]
        public int CoworkingSpaceId { get; set; }

        [Column("facility_id")]
        public int FacilityId { get; set; }

        [Column("price")]
        public decimal Price { get; set; }

        [ForeignKey(nameof(CoworkingSpaceId))]
        [InverseProperty(nameof(CoworkingSpace.CoworkingFacilityPrices))]
        public virtual CoworkingSpace CoworkingSpace { get; set; } = null!;

        [ForeignKey(nameof(FacilityId))]
        [InverseProperty(nameof(Facility.CoworkingFacilityPrices))]
        public virtual Facility Facility { get; set; } = null!;
    }
}

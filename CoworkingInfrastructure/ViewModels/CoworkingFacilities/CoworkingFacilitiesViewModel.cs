using System.ComponentModel.DataAnnotations;
using CoworkingInfrastructure.ViewModels.Shared;

namespace CoworkingInfrastructure.ViewModels.CoworkingFacilities
{
    public class CoworkingFacilitiesViewModel
    {
        public int CoworkingSpaceId { get; set; }

        public List<FacilitySelection> Facilities { get; set; }
    }

}

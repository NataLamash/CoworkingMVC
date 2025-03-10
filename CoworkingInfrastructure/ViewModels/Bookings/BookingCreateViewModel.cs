using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CoworkingInfrastructure.ViewModels.Bookings
{
    public class BookingCreateViewModel
    {
        [Required]
        public int CoworkingSpaceId { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime StartTime { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime EndTime { get; set; }

        // public List<int> SelectedFacilities { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;


namespace CoworkingInfrastructure.ViewModels.Bookings
{
    public class BookingCreateViewModel
    {
        public int CoworkingSpaceId { get; set; }

        // Якщо хочете працювати з датою саме як з датою, зробіть:
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }

        // Якщо є час
        [DataType(DataType.Time)]
        public string? StartTime { get; set; }

        [DataType(DataType.Time)]
        public string? EndTime { get; set; }

        public string? BookingType { get; set; }
    }

}

using CoworkingInfrastructure.ViewModels.Shared;

namespace CoworkingInfrastructure.ViewModels.CoworkingSpaces
{
    public class CreateCoworkingSpaceViewModel
    {
        // властивості самого коворкінгу
        public string Name { get; set; } = "";
        public string Location { get; set; } = "";
        public string? Description { get; set; }
        public int Capacity { get; set; }
        public decimal HourlyRate { get; set; }

        // список усіх можливих фасіліті з чекбоксами + полем для ціни
        public List<FacilitySelection> Facilities { get; set; } = new();

        public List<IFormFile> Photos { get; set; } = new();
    }
}

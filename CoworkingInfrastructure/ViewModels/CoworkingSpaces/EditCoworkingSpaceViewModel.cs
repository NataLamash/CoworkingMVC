using CoworkingInfrastructure.ViewModels.Shared;

namespace CoworkingInfrastructure.ViewModels.CoworkingSpaces
{
    public class EditCoworkingSpaceViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Location { get; set; } = "";
        public string? Description { get; set; }
        public int Capacity { get; set; }
        public decimal HourlyRate { get; set; }

        // Замість простої колекції рядків — список DTO із можливістю видалення
        public List<PhotoItem> ExistingPhotos { get; set; } = new();

        // Нові фото — тепер не Required
        public List<IFormFile> NewPhotos { get; set; } = new();

        public List<FacilitySelection> Facilities { get; set; } = new();
    }

    // Допоміжний DTO для кращого біндінгу
    public class PhotoItem
    {
        public int Id { get; set; }             // первинний ключ CoworkingSpaceImage
        public string FilePath { get; set; } = null!;
        public bool Remove { get; set; }        // якщо true → видалити
    }

}

namespace CoworkingInfrastructure.ViewModels.Shared
{
    public class FacilitySelection
    {
        public int FacilityId { get; set; }
        public string FacilityName { get; set; } = "";
        public bool IsSelected { get; set; }      // чекбокс
        public decimal? Price { get; set; }       // ціна, якщо обрано
    }
}

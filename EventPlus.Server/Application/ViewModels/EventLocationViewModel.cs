namespace EventPlus.Server.Application.ViewModels
{
    public class EventLocationViewModel
    {
        public string? Name { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public int? Capacity { get; set; }
        public string? Contacts { get; set; }
        public double? Price { get; set; }
        public int? HoldingEquipment { get; set; }
        public int IdEventLocation { get; set; }
        public int IDEquipment { get; set; }
        public List<int> SectorIds { get; set; } = new List<int>();
    }
}

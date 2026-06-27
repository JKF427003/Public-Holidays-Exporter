namespace PublicHolidaysExporter.Models
{
    public class Holiday
    {
        public string CountryCode { get; set; } = string.Empty;

        public string Language { get; set; } = string.Empty;

        public DateTime Date { get; set; }

        public string Name { get; set; } = string.Empty;
    }
}
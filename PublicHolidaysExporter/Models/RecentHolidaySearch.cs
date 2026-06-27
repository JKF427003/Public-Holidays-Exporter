namespace PublicHolidaysExporter.Models
{
    public class RecentHolidaySearch
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string CountryCode { get; set; } = string.Empty;

        public string Language { get; set; } = string.Empty;

        public int Year { get; set; }

        public bool UseDateRange { get; set; }

        public DateTime ValidFrom { get; set; }

        public DateTime ValidTo { get; set; }

        public DateTime SearchedAt { get; set; } = DateTime.Now;

        public List<Holiday> Holidays { get; set; } = new();
    }
}

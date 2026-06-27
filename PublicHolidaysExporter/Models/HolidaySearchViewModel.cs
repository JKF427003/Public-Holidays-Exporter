using System.ComponentModel.DataAnnotations;

namespace PublicHolidaysExporter.Models
{
    public class HolidaySearchViewModel
    {
        [Required(ErrorMessage = "Please select a country.")]
        public string CountryCode { get; set; } = string.Empty;

        [Required]
        [Range(1900, 2100, ErrorMessage = "Enter a valid year.")]
        public int Year { get; set; } = DateTime.Today.Year;
        public List<int> AvailableYears { get; set; } = new();

        [Required]
        public string Language { get; set; } = "EN";

        public List<Holiday> Holidays { get; set; } = new();
        
        public List<Country> Countries { get; set; } = new();

        public bool UseDateRange { get; set; }

        [DataType(DataType.Date)]
        public DateTime? StartDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }
    }
}

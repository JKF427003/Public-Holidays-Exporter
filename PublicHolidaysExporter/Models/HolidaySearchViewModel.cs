using System.ComponentModel.DataAnnotations;

namespace PublicHolidaysExporter.Models
{
    public class HolidaySearchViewModel
    {
        [Required]
        [StringLength(2, MinimumLength = 2)]
        [RegularExpression("^[A-Za-z]{2}$", ErrorMessage = "Enter a valid 2-letter country code, for example MT, IT, or DE.")]
        public string CountryCode { get; set; } = string.Empty;

        [Required]
        [Range(1900, 2100, ErrorMessage = "Enter a valid year.")]
        public int Year { get; set; } = DateTime.Today.Year;

        [Required]
        public string Language { get; set; } = "EN";

        public List<Holiday> Holidays { get; set; } = new();
        
        public List<Country> Countries { get; set; } = new();
    }
}

using System.Net.Http.Json;
using PublicHolidaysExporter.Models;

namespace PublicHolidaysExporter.Services
{
    public class OpenHolidaysService : IOpenHolidaysService
    {
        private readonly HttpClient _httpClient;

        public OpenHolidaysService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Holiday>> GetPublicHolidaysAsync(string countryCode, string language, DateTime validFrom, DateTime validTo)
        {
            var validFromText = validFrom.ToString("yyyy-MM-dd");
            var validToText = validTo.ToString("yyyy-MM-dd");

            var url = $"PublicHolidays?countryIsoCode={countryCode.ToUpper()}" + $"&languageIsoCode={language.ToUpper()}" + $"&validFrom={validFromText}&validTo={validToText}";

            var apiHolidays = await _httpClient.GetFromJsonAsync<List<OpenHolidayResponse>>(url);

            if (apiHolidays == null)
            {
                return new List<Holiday>();
            }

            return apiHolidays.Select(apiHoliday => new Holiday
            {
                Date = apiHoliday.StartDate,
                Name = GetHolidayName(apiHoliday, language),
                CountryCode = countryCode,
                Language = language
            }).ToList();
        }

        public async Task<List<Country>> GetCountriesAsync()
        {
            var apiCountries = await _httpClient.GetFromJsonAsync<List<OpenCountryResponse>>("Countries");

            if (apiCountries == null)
            {
                return new List<Country>();
            }

            return apiCountries
                .Select(country => new Country
                {
                    IsoCode = country.IsoCode.Trim().ToUpperInvariant(),
                    Name = country.Name.FirstOrDefault()?.Text ?? country.IsoCode
                })
                .OrderBy(country => country.Name)
                .ToList();
        }

        private class OpenHolidayResponse
        {
            public DateTime StartDate { get; set; }

            public List<OpenHolidayName> Name { get; set; } = new();
        }

        private class OpenHolidayName
        {
            public string Language { get; set; } = string.Empty;
            public string Text { get; set; } = string.Empty;
        }

        private static string GetHolidayName(OpenHolidayResponse apiHoliday, string language)
        {
            var matchingName = apiHoliday.Name.FirstOrDefault(name => string.Equals(name.Language, language, StringComparison.OrdinalIgnoreCase));

            if (matchingName != null && !string.IsNullOrWhiteSpace(matchingName.Text))
            {
                return matchingName.Text;
            }
            
            var fallbackName = apiHoliday.Name.FirstOrDefault(name => !string.IsNullOrWhiteSpace(name.Text));

            return fallbackName?.Text ?? "Unknown Holiday";
        }

        private class OpenCountryResponse
        {
            public string IsoCode { get; set; } = string.Empty;

            public List<OpenCountryName> Name { get; set; } = new();
        }

        private class OpenCountryName
        {
            public string Language { get; set; } = string.Empty;

            public string Text { get; set; } = string.Empty;
        }
    }
}

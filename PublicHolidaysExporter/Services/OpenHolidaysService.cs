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

        public async Task<List<Holiday>> GetPublicHolidaysAsync(string countryCode, int year, string language)
        {
            var validFrom = $"{year}-01-01";
            var validTo = $"{year}-12-31";

            var url = $"PublicHolidays?countryIsoCode={countryCode.ToUpper()}" + $"&languageIsoCode={language.ToUpper()}" + $"&validFrom={validFrom}" + $"&validTo={validTo}";

            var apiHolidays = await _httpClient.GetFromJsonAsync<List<OpenHolidayResponse>>(url);

            if (apiHolidays == null)
            {
                return new List<Holiday>();
            }

            return apiHolidays.Select(apiHoliday => new Holiday
            {
                Date = apiHoliday.StartDate,
                Name = GetHolidayName(apiHoliday, language)
            }).ToList();
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

            return fallbackName?.Text ?? "Unkown Holiday";
        }
    }
}

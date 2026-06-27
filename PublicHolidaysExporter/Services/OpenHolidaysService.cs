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
                Name = apiHoliday.Name.FirstOrDefault()?.Text ?? "Unknown holiday"
            }).ToList();
        }

        private class OpenHolidayResponse
        {
            public DateTime StartDate { get; set; }

            public List<OpenHolidayName> Name { get; set; } = new();
        }

        private class OpenHolidayName
        {
            public string Text { get; set; } = string.Empty;
        }
    }
}

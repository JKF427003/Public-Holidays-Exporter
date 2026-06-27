using PublicHolidaysExporter.Models;

namespace PublicHolidaysExporter.Services
{
    public interface IOpenHolidaysService
    {
        Task<List<Holiday>> GetPublicHolidaysAsync(string countryCode, int year, string language);

        Task<List<Country>> GetCountriesAsync();
    }
}

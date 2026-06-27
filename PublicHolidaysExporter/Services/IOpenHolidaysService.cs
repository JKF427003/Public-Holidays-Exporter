using PublicHolidaysExporter.Models;

namespace PublicHolidaysExporter.Services
{
    public interface IOpenHolidaysService
    {
        Task<List<Holiday>> GetPublicHolidaysAsync(string countryCode, string language, DateTime validFrom, DateTime validTo);
        Task<List<Country>> GetCountriesAsync();
    }
}

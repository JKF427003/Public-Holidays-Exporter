using PublicHolidaysExporter.Models;

namespace PublicHolidaysExporter.Services
{
    public interface ICsvExportService
    {
        byte[] GenerateCsv(IEnumerable<Holiday> holidays, string countryCode, string language, DateTime validFrom, DateTime validTo);
    }
}

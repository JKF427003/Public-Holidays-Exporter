using System.Text;
using PublicHolidaysExporter.Models;

namespace PublicHolidaysExporter.Services
{
    public class CsvExportService : ICsvExportService
    {
        public byte[] GenerateCsv(IEnumerable<Holiday> holidays)
        {
            var csv = new StringBuilder();

            csv.AppendLine("Date, Name");

            foreach (var holiday in holidays)
            {
                csv.AppendLine($"{holiday.Date:yyyy-MM-dd},{EscapeCsvValue(holiday.Name)}");
            }

            return Encoding.UTF8.GetBytes(csv.ToString());
        }

        private static string EscapeCsvValue(string value)
        {
            if (value.Contains(',') || value.Contains('"') || value.Contains('\n'))
            {
                return $"\"{value.Replace("\"", "\"\"")}\"";
            }

            return value;   
        }
    }
}

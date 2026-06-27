using PublicHolidaysExporter.Models;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PublicHolidaysExporter.Services
{
    public class CsvExportService : ICsvExportService
    {
        public byte[] GenerateCsv(IEnumerable<Holiday> holidays, string countryCode, string language, DateTime validFrom, DateTime validTo)
        {
            var csv = new StringBuilder();


            csv.AppendLine("Public Holidays Export");
            csv.AppendLine($"Country Code,{EscapeCsvValue(countryCode)}");
            csv.AppendLine($"Language,{EscapeCsvValue(language)}");
            csv.AppendLine($"Date From,{validFrom:yyyy-MM-dd}");
            csv.AppendLine($"Date To,{validTo:yyyy-MM-dd}");
            csv.AppendLine($"Generated On,{DateTime.Now:yyyy-MM-dd HH:mm}");
            csv.AppendLine();

            csv.AppendLine("Country Code, Language, Date, Name");

            foreach (var holiday in holidays)
            {
                csv.AppendLine($"{holiday.Date:yyyy-MM-dd},{EscapeCsvValue(holiday.Name)},{holiday.CountryCode},{holiday.Language}");
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

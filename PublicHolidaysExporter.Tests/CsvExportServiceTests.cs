using PublicHolidaysExporter.Models;
using PublicHolidaysExporter.Services;
using System.Text;

namespace PublicHolidaysExporter.Tests;

public class CsvExportServiceTests
{
    [Fact]
    public void GenerateCsv_ShouldIncludeHeader()
    {
        var service = new CsvExportService();
        var holidays = new List<Holiday>();

        var result = service.GenerateCsv(holidays, "MT", "EN", new DateTime(2026, 1, 1), new DateTime(2026, 12, 31));
        var csv = Encoding.UTF8.GetString(result);

        Assert.Contains("Country Code", csv);
        Assert.Contains("Language", csv);
        Assert.Contains("Date", csv);
        Assert.Contains("Name", csv);
    }

    [Fact]
    public void GenerateCsv_ShouldIncludeHolidayData()
    {
        var service = new CsvExportService();

        var holidays = new List<Holiday>
    {
        new Holiday
        {
            Date = new DateTime(2026, 1, 1),
            Name = "New Year's Day"
        }
    };

        var result = service.GenerateCsv(holidays, "MT", "EN", new DateTime(2026, 1, 1), new DateTime(2026, 12, 31));
        var csv = Encoding.UTF8.GetString(result);

        Assert.Contains("MT", csv);
        Assert.Contains("EN", csv);
        Assert.Contains("2026-01-01", csv);
        Assert.Contains("New Year's Day", csv);
    }
}
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using PublicHolidaysExporter.Models;
using PublicHolidaysExporter.Services;

namespace PublicHolidaysExporter.Controllers
{
    public class HolidaysController : Controller
    {
        private readonly IOpenHolidaysService _openHolidaysService;
        private readonly ICsvExportService _csvExportService;

        public HolidaysController(IOpenHolidaysService openHolidaysService, ICsvExportService csvExportService)
        {
            _openHolidaysService = openHolidaysService;
            _csvExportService = csvExportService;
        }

        private static List<int> GetAvailableYears()
        {
            var currentYear = DateTime.Today.Year;

            return Enumerable.Range(currentYear - 20, 31).Reverse().ToList();
        }

        private List<RecentHolidaySearch> GetRecentSearchesFromSession()
        {
            var json = HttpContext.Session.GetString("RecentSearches");

            if (string.IsNullOrWhiteSpace(json))
            {
                return new List<RecentHolidaySearch>();
            }

            return JsonSerializer.Deserialize<List<RecentHolidaySearch>>(json) ?? new List<RecentHolidaySearch>();
        }

        private void SaveRecentSearchesToSession(List<RecentHolidaySearch> searches)
        {
            HttpContext.Session.SetString("RecentSearches", JsonSerializer.Serialize(searches));
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = new HolidaySearchViewModel
            {
                Countries = await _openHolidaysService.GetCountriesAsync(),
                AvailableYears = GetAvailableYears()
            };

            return View(model);
        }

        [HttpGet]
        public IActionResult RecentSearches()
        {
            var searches = GetRecentSearchesFromSession();

            return View(searches);
        }

        [HttpPost]
        public async Task<IActionResult> Index(HolidaySearchViewModel model)
        {
            model.CountryCode = model.CountryCode.Trim().ToUpperInvariant();
            model.Language = model.Language.Trim().ToUpperInvariant();

            ModelState.Clear();

            var validFrom = new DateTime(model.Year, 1, 1);
            var validTo = new DateTime(model.Year, 12, 31);

            if (model.UseDateRange)
            {
                if (!model.StartDate.HasValue || !model.EndDate.HasValue)
                {
                    ModelState.AddModelError(string.Empty, "Please enter both start and end dates.");
                }
                else if (model.StartDate.Value > model.EndDate.Value)
                {
                    ModelState.AddModelError(string.Empty, "Start date must be before end date.");
                }
                else
                {
                    validFrom = model.StartDate.Value;
                    validTo = model.EndDate.Value;
                }
            }

            if (!TryValidateModel(model) || !ModelState.IsValid)
            {
                model.Countries = await _openHolidaysService.GetCountriesAsync();
                model.AvailableYears = GetAvailableYears();
                return View(model);
            }

            try
            {
                model.Holidays = await _openHolidaysService.GetPublicHolidaysAsync(model.CountryCode, model.Language, validFrom, validTo);

                var searches = GetRecentSearchesFromSession();

                searches.Insert(0, new RecentHolidaySearch
                {
                    CountryCode = model.CountryCode,
                    Language = model.Language,
                    Year = model.Year,
                    UseDateRange = model.UseDateRange,
                    ValidFrom = validFrom,
                    ValidTo = validTo,
                    Holidays = model.Holidays
                });

                searches = searches.Take(10).ToList();

                SaveRecentSearchesToSession(searches);
            }
            catch
            {
                ModelState.AddModelError(string.Empty, "Could not retrieve holidays from the API. Please try again later.");
            }

            model.Countries = await _openHolidaysService.GetCountriesAsync();
            model.AvailableYears = GetAvailableYears();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DownloadCsv(HolidaySearchViewModel model)
        {
            try
            {
                model.CountryCode = model.CountryCode.Trim().ToUpperInvariant();
                model.Language = model.Language.Trim().ToUpperInvariant();

                var validFrom = new DateTime(model.Year, 1, 1);
                var validTo = new DateTime(model.Year, 12, 31);

                if (model.UseDateRange && model.StartDate.HasValue && model.EndDate.HasValue)
                {
                    validFrom = model.StartDate.Value;
                    validTo = model.EndDate.Value;
                }

                var holidays = await _openHolidaysService.GetPublicHolidaysAsync(
                    model.CountryCode,
                    model.Language,
                    validFrom,
                    validTo
                );

                if (!holidays.Any())
                {
                    TempData["ErrorMessage"] = "No holidays were found to export.";
                    return RedirectToAction(nameof(Index));
                }

                var csvBytes = _csvExportService.GenerateCsv(
                    holidays,
                    model.CountryCode,
                    model.Language,
                    validFrom,
                    validTo
                );

                var fileName = $"public-holidays-{model.CountryCode}-{model.Year}.csv";
                return File(csvBytes, "text/csv", fileName);
            }
            catch
            {
                TempData["ErrorMessage"] = "Could not generate the CSV file because the holiday API is currently unavailable.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public IActionResult DownloadRecentCsv(string id)
        {
            var searches = GetRecentSearchesFromSession();
            var search = searches.FirstOrDefault(search => search.Id == id);

            if (search == null)
            {
                return RedirectToAction(nameof(RecentSearches));
            }

            var csvBytes = _csvExportService.GenerateCsv(
                search.Holidays,
                search.CountryCode,
                search.Language,
                search.ValidFrom,
                search.ValidTo);

            var fileName = $"public-holidays-{search.CountryCode}-{search.ValidFrom:yyyy-MM-dd}-{search.ValidTo:yyyy-MM-dd}.csv";

            return File(csvBytes, "text/csv", fileName);
        }
    }
}
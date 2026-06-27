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

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = new HolidaySearchViewModel
            {
                Countries = await _openHolidaysService.GetCountriesAsync()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Index(HolidaySearchViewModel model)
        {
            model.Countries = await _openHolidaysService.GetCountriesAsync();

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            model.CountryCode = model.CountryCode.ToUpper();
            model.Language = model.Language.ToUpper();

            try
            {
                model.Holidays = await _openHolidaysService.GetPublicHolidaysAsync(
                    model.CountryCode,
                    model.Year,
                    model.Language);
            }
            catch
            {
                ModelState.AddModelError(string.Empty, "Could not retrieve holidays from the API. Please try again later.");
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DownloadCsv(HolidaySearchViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Countries = await _openHolidaysService.GetCountriesAsync();
                return View("Index", model);
            }

            var holidays = await _openHolidaysService.GetPublicHolidaysAsync(model.CountryCode.ToUpper(), model.Year, model.Language.ToUpper());

            var csvBytes = _csvExportService.GenerateCsv(holidays);

            var fileName = $"public-holidays-{model.CountryCode.ToUpper()}-{model.Year}.csv";

            return File(csvBytes, "text/csv", fileName);
        }
    }
}
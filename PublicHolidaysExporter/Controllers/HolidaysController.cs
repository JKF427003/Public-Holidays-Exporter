using Microsoft.AspNetCore.Mvc;
using PublicHolidaysExporter.Models;
using PublicHolidaysExporter.Services;

namespace PublicHolidaysExporter.Controllers
{
    public class HolidaysController : Controller
    {
        private readonly IOpenHolidaysService _openHolidaysService;

        public HolidaysController(IOpenHolidaysService openHolidaysService)
        {
            _openHolidaysService = openHolidaysService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var model = new HolidaySearchViewModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Index(HolidaySearchViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);   
            }

            model.CountryCode = model.CountryCode.ToUpper();
            model.Language = model.Language.ToUpper();

            try
            {
                model.Holidays = await _openHolidaysService.GetPublicHolidaysAsync(model.CountryCode, model.Year, model.Language);
            }
            catch 
            {
                ModelState.AddModelError(string.Empty, "Could not retrieve holidays from the API. Please try again later.");
            }

            return View(model);
        }
    }
}
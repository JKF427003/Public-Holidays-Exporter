using Microsoft.AspNetCore.Mvc;

namespace PublicHolidaysExporter.Controllers
{
    public class HolidaysController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

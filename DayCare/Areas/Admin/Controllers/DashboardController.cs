using Microsoft.AspNetCore.Mvc;

namespace DayCare.Areas.Admin.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

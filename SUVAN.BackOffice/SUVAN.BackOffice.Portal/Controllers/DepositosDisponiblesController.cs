using Microsoft.AspNetCore.Mvc;

namespace SUVAN.BackOffice.Portal.Controllers
{
    public class DepositosDisponiblesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

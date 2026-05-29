using Microsoft.AspNetCore.Mvc;

namespace SUVAN.BackOffice.Portal.Controllers
{
  public class PagoController : Controller
  {
    public IActionResult Success()
    {
      return View();
    }

    public IActionResult Error()
    {
      return View();
    }
  }
}

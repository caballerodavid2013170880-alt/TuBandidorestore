using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SUVAN.BackOffice.Portal.Controllers
{
    [Authorize]
    [Route("ModuloAdministrativo/LlantaAsignacion")]
    public class LlantaAsignacionController : Controller
    {
        [HttpGet("")]
        [HttpGet("~/llantaasignacion")]
        [HttpGet("~/llantasasignacion")]
        [HttpGet("~/Administrativo/ModuloAdministrativo/LlantaAsignacion")]
        public IActionResult Index()
        {
            return View();
        }
    }
}

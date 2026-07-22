using Microsoft.AspNetCore.Mvc;
using SUVAN.BackOffice.Portal.Helper;
using SUVAN.BackOffice.Service.Administrativo;

namespace SUVAN.BackOffice.Portal.Controllers
{
    [Route("ModuloAdministrativo/Llantas")]
    public class LlantasController : Controller
    {
        private readonly ILlantaService llantaService;

        public LlantasController(ILlantaService llantaService)
        {
            this.llantaService = llantaService;
        }

        [HttpGet("")]
        [HttpGet("~/Llantas")]
        [HttpGet("~/Administrativo/ModuloAdministrativo/Llantas")]
        public async Task<IActionResult> Index()
        {
            var llantas = await llantaService.GetLlantas(User.GetEmpresaId());
            return View(llantas);
        }
    }
}

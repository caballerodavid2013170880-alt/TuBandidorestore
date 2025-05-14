using Microsoft.AspNetCore.Mvc;
using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Service.Logistica;

namespace SUVAN.BackOffice.Portal.Controllers
{
    public class TipoServicioController : Controller
    {
        private readonly ILogger<TipoServicioController> _logger;
        private readonly ITipoServicioService tipoServicio;

        public TipoServicioController(ILogger<TipoServicioController> logger,
        ITipoServicioService tipoServicio)

        {
            _logger = logger;
            this.tipoServicio = tipoServicio;

        }
        public async Task<IActionResult> Index()
        {
            var tipo = await tipoServicio.GetTipoService();
            return View(tipo);
        }
    }
}

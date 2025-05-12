using Microsoft.AspNetCore.Mvc;
using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Service.Configuracion;
using SUVAN.BackOffice.Service.Logistica;

namespace SUVAN.BackOffice.Portal.Controllers
{
    public class MotivoAuxilioVialController : Controller
    {
        private readonly ILogger<MotivoAuxilioVialController> _logger;
        private readonly IMotivoAuxilioVialService MotivoAuxilioVial;

        public MotivoAuxilioVialController(ILogger<MotivoAuxilioVialController> logger,
        IMotivoAuxilioVialService MotivoAuxilioVial)

        {
            _logger = logger;
            this.MotivoAuxilioVial = MotivoAuxilioVial;

        }
        public async Task<IActionResult> Index()
        {
            var motivoauxilio = await MotivoAuxilioVial.GetMotivoAuxilioVial();
            
            return View(motivoauxilio);
        }

    }
}

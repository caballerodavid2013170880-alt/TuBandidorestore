using Microsoft.AspNetCore.Mvc;
using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.ViewModel.Logistica;
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
        public async Task<IActionResult> AgregarMotivoAuxilioVial(int id)
        {
            var agregarMotivoModel = await MotivoAuxilioVial.GetMotivoAuxilioViewModel(id);
            return View(agregarMotivoModel);
        }

        [HttpPost]
        public async Task<IActionResult> AgregarMotivoAuxilioVial(MotivoAuxilioVialViewModel model)
        {
            try
            {
                var result = await MotivoAuxilioVial.AgregarMotivoAuxilioVial(model);

                if (result)
                {
                    return RedirectToAction("Index", "MotivoAuxilioVial");
                }

                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }


        }

        [HttpPost]
        public async Task<IActionResult> EliminarMotivo([FromBody] MotivoAuxilioVialViewModel model)
        {
            var motivo_id = Convert.ToInt32(model.MotivoId);
            try
            {
                await MotivoAuxilioVial.EliminarMotivoAuxilioVial(motivo_id);

                return Ok(new { success = true });

            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

    }
}

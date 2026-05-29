using Microsoft.AspNetCore.Mvc;
using SUVAN.BackOffice.Models.ViewModel.Logistica;
using SUVAN.BackOffice.Service.Logistica;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using static SUVAN.BackOffice.Models.ViewModel.Logistica.CausaMantenimientoViewModel;
using SUVAN.BackOffice.Portal.Helper;
using static SUVAN.BackOffice.Models.ViewModel.Logistica.VehiculoDetalleViewModel;

namespace SUVAN.BackOffice.Portal.Controllers
{
    public class MarcaController : Controller
    {
        private readonly ILogger<MarcaController> _logger;
        private readonly IMarcaService marcaService;

        public MarcaController(ILogger<MarcaController> logger,
        IMarcaService marcaService)

        {
            _logger = logger;
            this.marcaService = marcaService;

        }
        public async Task<IActionResult> Index()
        {
            var marca = await marcaService.GetMarca();
            return View(marca);
        }

        public async Task<IActionResult> AgregarMarca(int id)
        {
            var agregarModel = await marcaService.GetMarcaViewModel(id);
            return View(agregarModel);
        }

        [HttpPost]
        public async Task<IActionResult> AgregarMarca(MarcaViewModel model)
        {
            try
            {
                var result = await marcaService.AgregarMarca(model);

                if (result)
                {
                    return RedirectToAction("Index", "Marca");
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
        public async Task<IActionResult> EliminarMarca([FromBody] MarcaViewModel model)
        {
            try
            {
                await marcaService.EliminarMarca(model.IdMarca);


                return Ok(new { success = true });


            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

    }
}
using Microsoft.AspNetCore.Mvc;
using SUVAN.BackOffice.Service.Logistica;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using SUVAN.BackOffice.Portal.Models;
using SUVAN.BackOffice.Models.ViewModel.Logistica;
using SUVAN.BackOffice.Portal.Helper;
using static SUVAN.BackOffice.Models.ViewModel.Logistica.VehiculoDetalleViewModel;

namespace SUVAN.BackOffice.Portal.Controllers
{
    public class ModeloController : Controller
    {
        private readonly ILogger<ModeloController> _logger;
        private readonly IModeloService modeloService;

        public ModeloController(ILogger<ModeloController> logger,
        IModeloService modeloService)

        {
            _logger = logger;
            this.modeloService = modeloService;

        }
        public async Task<IActionResult> Index()
        {
            var modelo = await modeloService.GetModelo();
            return View(modelo);
        }

        public async Task<IActionResult> AgregarModelo(int id)
        {
            var agregarModel = await modeloService.GetModeloViewModel(id);
            agregarModel.MarcasView = modeloService.ObtenerMarca();
            agregarModel.TipoVehiculoView = modeloService.ObtenerTipoVehiculo();
            return View(agregarModel);
        }

        [HttpPost]
        public async Task<IActionResult> AgregarModelo(ModeloViewModel model)
        {
            try
            {
                var result = await modeloService.AgregarModelo(model);

                if (result)
                {
                    return RedirectToAction("Index", "Modelo");
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
        public async Task<IActionResult> EliminarModelo([FromBody] ModeloViewModel model)
        {
            try
            {
                await modeloService.EliminarModelo(model.IdModelo);


                return Ok(new { success = true });


            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }
    }
}
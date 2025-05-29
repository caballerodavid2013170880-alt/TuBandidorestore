using Microsoft.AspNetCore.Mvc;
using SUVAN.BackOffice.Models.ViewModel.Logistica;
using SUVAN.BackOffice.Service.Logistica;
using static SUVAN.BackOffice.Models.ViewModel.Logistica.CausaMantenimientoViewModel;

namespace SUVAN.BackOffice.Portal.Controllers
{
    public class MarcaController : Controller
    {
        private readonly ILogger<MarcaController> _logger;
        private readonly IMarcaService marcaService;

        private MarcaController(ILogger<MarcaController> logger, IMarcaService marcaService)
        {
            _logger = logger;
            this.marcaService = marcaService;
        }

        public async Task<IActionResult> Index()
        {
            var marcas = await marcaService.GetMarcas();
            return View(marcas);
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
                    return RedirectToAction("MarcaIndex", "Marca");
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
                await marcaService.EliminarMarca(model.Id_Marca);
                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }
    }
}
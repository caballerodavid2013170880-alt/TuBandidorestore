using Microsoft.AspNetCore.Mvc;
using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Portal.Helper;
using SUVAN.BackOffice.Service.Logistica;

namespace SUVAN.BackOffice.Portal.Controllers
{
    public class CatalogsController : Controller
    {
        private readonly ILogger<CatalogsController> _logger;
        private readonly ICatalogsService catalogsService;

        public CatalogsController(ILogger<CatalogsController> logger,
            ICatalogsService catalogsService)

        {
            _logger = logger;
            this.catalogsService = catalogsService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCatalogosSearch(string id, int IdEmpresa, int nClaveFiltro)
        {
            try
            {
                IdEmpresa = User.GetEmpresaId();
                var result = await catalogsService.GetCatalog(id, IdEmpresa, nClaveFiltro);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

    }
}

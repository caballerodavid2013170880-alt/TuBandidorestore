using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SUVAN.BackOffice.API.Provider;
using SUVAN.BackOffice.Models.Contenido;
using SUVAN.BackOffice.Models.Favoritos;
using SUVAN.BackOffice.Models.ManejoRespuesta;
using SUVAN.BackOffice.Service.Contenidos;
using SUVAN.BackOffice.Service.Favoritos;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;

namespace SUVAN.BackOffice.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ContenidoController : ControllerBase
    {
        private readonly ISuVanResponseService _suVanResponseService;
        private readonly IContenidoService _contenidoService;
        public ContenidoController(ISuVanResponseService suVanResponseService, IContenidoService contenidoService)
        {
            _contenidoService = contenidoService;
            _suVanResponseService = suVanResponseService;
        }

        [HttpGet]
        [Route("ObtenContenidoPorId")]
        [SwaggerOperation(Description = "Este servicio regresa el contenido por id")]
        [SwaggerResponse(200, Type = typeof(SuVanResponse<ContenidoResponse>))]
        [AllowAnonymous]
        public async Task<ActionResult> ObtenerContenidoPorId(int clave)
        {
            var response = await _contenidoService.ObtenContenidoPorId(clave);
            return _suVanResponseService.Handle(response);
        }

        [HttpGet]
        [Route("ObtenContenidoPorTipo")]
        [SwaggerOperation(Description = "Este servicio regresa el contenido por tipo de contenido")]
        [SwaggerResponse(200, Type = typeof(SuVanResponse<List<ContenidoResponse>>))]
        [AllowAnonymous]
        public async Task<ActionResult> ObtenContenidoPorTipo(int tipo)
        {
            var response = await _contenidoService.ObtenContenidoPorTipo(tipo);
            return _suVanResponseService.Handle(response);
        }

        [HttpGet]
        [Route("ObtenContenidosGenerales")]
        [SwaggerOperation(Description = "Este servicio regresa una lista de contenidos generales")]
        [SwaggerResponse(200, Type = typeof(SuVanResponse<List<ContenidoGeneral>>))]
        [AllowAnonymous]
        public async Task<ActionResult> ObtenContenidosGenerales()
        {
            var response = await _contenidoService.ObtenContenidosGenerales();
            return _suVanResponseService.Handle(response);
        }

        [HttpGet]
        [Route("ObtenContenidoMembresia")]
        [SwaggerOperation(Description = "Este servicio regresa una lista de contenido de membresia")]
        [SwaggerResponse(200, Type = typeof(SuVanResponse<ContenidoMembresiaResponse>))]
        [AllowAnonymous]
        public async Task<ActionResult> ObtenContenidoMembresia()
        {
            var response = await _contenidoService.ObtenContenidoMembresia();
            return _suVanResponseService.Handle(response);
        }
    }
}

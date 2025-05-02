using Microsoft.AspNetCore.Mvc;
using SUVAN.BackOffice.API.Provider;
using SUVAN.BackOffice.Models.Corrida;
using SUVAN.BackOffice.Models.ManejoRespuesta;
using SUVAN.BackOffice.Service.Corrida;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;

namespace SUVAN.BackOffice.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CorridaController : ControllerBase
    {
        private readonly ISuVanResponseService _suVanResponseService;
        private readonly ICorridaService _corridaService;
        public CorridaController(ISuVanResponseService suVanResponseService, ICorridaService viajeService)
        {
            _corridaService = viajeService;
            _suVanResponseService = suVanResponseService;
        }

        [HttpGet]
        [Route("Estaciones/{idCorrida}")]
        [SwaggerOperation(Description = "Este servicio lista las estaciones de la corrida")]
        [SwaggerResponse(200, Type = typeof(SuVanResponse<CorridaEstacionesResponse>))]
        public async Task<ActionResult> Estaciones(int idCorrida)
        {
            var response = await _corridaService.Estaciones(idCorrida);
            if (response.CodigoMensaje != "200")
            {
                return _suVanResponseService.Handle(response);
            }

            return _suVanResponseService.Handle(response);
        }

        [HttpPost]
        [Route("ComenzarAbordaje")]
        [SwaggerOperation(Description = "Este servicio listara los usuarios que suben y bajan y actualiza el estado de la parada durante la corrida")]
        [SwaggerResponse(200, Type = typeof(SuVanResponse<CorridaParadaResponse>))]
        public async Task<ActionResult> ComenzarAbordaje([FromBody] ComenzarAbordajeRequest data)
        {
            var conductor = getUser();
            var response = await _corridaService.ComenzarAbordaje(data, conductor);
            if (response.CodigoMensaje != "200")
            {
                return _suVanResponseService.Handle(response);
            }

            return _suVanResponseService.Handle(response);
        }

        [HttpPost]
        [Route("RegresarAbordaje")]
        [SwaggerOperation(Description = "Este servicio regresa el abordaje y actualiza el estado de la parada en la corrida")]
        [SwaggerResponse(200, Type = typeof(SuVanResponse<CorridaParadaResponse>))]
        public async Task<ActionResult> RegresarAbordaje([FromBody] AbordajeRequest data)
        {
            var response = await _corridaService.RegresarAbordaje(data);
            if (response.CodigoMensaje != "200")
            {
                return _suVanResponseService.Handle(response);
            }

            return _suVanResponseService.Handle(response);
        }

        [HttpPost]
        [Route("TerminarAbordaje")]
        [SwaggerOperation(Description = "Este servicio termina el abordaje y actualiza el estado de la parada en la corrida")]
        [SwaggerResponse(200, Type = typeof(SuVanResponse<CorridaParadaResponse>))]
        public async Task<ActionResult> TerminarAbordaje([FromBody] AbordajeRequest data)
        {
            var response = await _corridaService.TerminarAbordaje(data);
            if (response.CodigoMensaje != "200")
            {
                return _suVanResponseService.Handle(response);
            }

            return _suVanResponseService.Handle(response);
        }

        [HttpPost]
        [Route("CheckIn")]
        [SwaggerOperation(Description = "Este servicio realiza el check in del viaje")]
        [SwaggerResponse(200, Type = typeof(SuVanResponse<CorridaParadaResponse>))]
        [SwaggerResponse(400, "Cuando los datos no coinciden con un viaje-boleto valido")]
        public async Task<ActionResult> CheckIn([FromBody] BoletoCheckRequest data)
        {
            var conductor = getUser();
            var response = await _corridaService.CheckIn(data, conductor);
            if (response.CodigoMensaje != "200")
            {
                return _suVanResponseService.Handle(response);
            }

            return _suVanResponseService.Handle(response);
        }

        private int getUser()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                var resultClaim = identity.Claims.Where(x => x.Type == "userId").Select(x => x.Value).FirstOrDefault();
                return int.Parse(resultClaim);
            }

            return 0;
        }

    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SUVAN.BackOffice.API.Provider;
using SUVAN.BackOffice.Models.ManejoRespuesta;
using SUVAN.BackOffice.Models.Viajes;
using SUVAN.BackOffice.Service.Viajes;
using Swashbuckle.AspNetCore.Annotations;

namespace SUVAN.BackOffice.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class OperadorController : ControllerBase
    {
        private readonly ISuVanResponseService _suVanResponseService;
        private readonly IViajeService _viajeService;
        public OperadorController(ISuVanResponseService suVanResponseService, IViajeService viajeService)
        {
            _viajeService = viajeService;
            _suVanResponseService = suVanResponseService;
        }

        [HttpPost]
        [Route("ObtenRecorrido")]
        [SwaggerOperation(Description = "Este servicio obtiene la información de las estaciones que tendra el recorrido")]
        [SwaggerResponse(200, Type = typeof(SuVanResponse<List<RecorridoViajeResponse>>))]
        public async Task<ActionResult> ObtenRecorrido([FromBody] RecorridoViajeOperadorRequest data)
        {
            var response = await _viajeService.ObtenRecorridoOperador(data);
            return _suVanResponseService.Handle(response);
        }
    }
}

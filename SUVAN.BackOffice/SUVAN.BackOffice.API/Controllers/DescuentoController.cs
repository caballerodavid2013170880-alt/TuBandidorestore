using Microsoft.AspNetCore.Mvc;
using SUVAN.BackOffice.API.Provider;
using SUVAN.BackOffice.Models.Descuento;
using SUVAN.BackOffice.Models.ManejoRespuesta;
using SUVAN.BackOffice.Service.Descuento;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;

namespace SUVAN.BackOffice.API.Controllers
{

    [ApiController]
    [Route("Descuento")]

    public class DescuentoController : ControllerBase
    {
        private readonly ISuVanResponseService _suVanResponseService;
        private readonly IDescuentoService _descuentoService;

        public DescuentoController(ISuVanResponseService suVanResponseService, IDescuentoService descuentoService)
        {
            _suVanResponseService = suVanResponseService;
            _descuentoService = descuentoService;

        }

        [HttpPost]
        [Route("ValidarCodigo")]
        [SwaggerOperation(Description = "Servicio para validar un codigo de descuento")]
        [SwaggerResponse(200, Type = typeof(SuVanResponse<DescuentoCodigoResponse>))]
        public async Task<ActionResult> ValidarCodigo([FromBody] DescuentoCodigoRequest data)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                var resultClaim = identity.Claims.Where(x => x.Type == "userId").Select(x => x.Value).FirstOrDefault();

                var response = await _descuentoService.ValidarCodigo(int.Parse(resultClaim ?? "0"), data);
                if (response.CodigoMensaje != "200")
                {
                    return _suVanResponseService.Handle(response);
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

    }
}

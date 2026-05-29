using Microsoft.AspNetCore.Mvc;
using SUVAN.BackOffice.API.Provider;
using SUVAN.BackOffice.Models.ManejoRespuesta;
using SUVAN.BackOffice.Models.Monedero;
using SUVAN.BackOffice.Models.Viajes;
using SUVAN.BackOffice.Service.Monedero;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;

namespace SUVAN.BackOffice.API.Controllers
{

    [ApiController]
    [Route("Monedero")]

    public class MonederoController : ControllerBase
    {
        private readonly ISuVanResponseService _suVanResponseService;
        private readonly IMonederoService _monederoService;


        public MonederoController(ISuVanResponseService suVanResponseService, IMonederoService monederoService)
        {
            _suVanResponseService = suVanResponseService;
            _monederoService = monederoService;

        }

        [HttpGet]
        [Route("ObtenMonedero")]
        [SwaggerOperation(Description = "Servicio para el saldo en el monedero")]
        [SwaggerResponse(200, Type = typeof(SuVanResponse<MonederoSaldoResponse>))]
        public async Task<ActionResult> ObtenMonedero()
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                var resultClaim = identity.Claims.Where(x => x.Type == "userId").Select(x => x.Value).FirstOrDefault();

                var response = await _monederoService.ObtenMonedero(int.Parse(resultClaim ?? "0"));
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

        [HttpPost]
        [Route("RecargaMonederoConTarjeta")]
        [SwaggerOperation(Description = "Servicio para realizar la recarga de monedero por medio de tarjeta")]
        [SwaggerResponse(200, Type = typeof(SuVanResponse<List<RecargaMonederoResponse>>))]
        public async Task<ActionResult> RecargaMonederoConTarjeta([FromBody] RecargaMonederoRequest data)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                var resultClaim = identity.Claims.Where(x => x.Type == "userId").Select(x => x.Value).FirstOrDefault();
                var resultClaimEmail = identity.Claims.Where(x => x.Type == "email").Select(x => x.Value).FirstOrDefault();

                var response = await _monederoService.RecargaMonederoConTarjeta(int.Parse(resultClaim ?? "0"), data, resultClaimEmail);
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

        [HttpPost]
        [Route("ConfirmaRecargaMonederoConTarjeta")]
        [SwaggerOperation(Description = "Servicio para confirmar la recarga realizada al monedero")]
        [SwaggerResponse(200, Type = typeof(SuVanResponse<List<ConfirmaRecargaMonederoResponse>>))]
        public async Task<ActionResult> ConfirmaRecargaMonederoConTarjeta([FromBody] ConfirmaRecargaMonederoRequest data)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                var resultClaim = identity.Claims.Where(x => x.Type == "userId").Select(x => x.Value).FirstOrDefault();

                var response = await _monederoService.ConfirmaRecargaMonederoConTarjeta(int.Parse(resultClaim ?? "0"), data);
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

        #region Controladores para PayPal
        [HttpPost]
        [Route("RecargaMonederoConPayPal")]
        [SwaggerOperation(Description = "Servicio para realizar la recarga de monedero por medio de PayPal")]
        [SwaggerResponse(200, Type = typeof(SuVanResponse<List<RecargaMonederoResponse>>))]
        public async Task<ActionResult> RecargaMonederoConPayPal([FromBody] RecargaMonederoRequest data)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                var resultClaim = identity.Claims.Where(x => x.Type == "userId").Select(x => x.Value).FirstOrDefault();
                var resultClaimEmail = identity.Claims.Where(x => x.Type == "email").Select(x => x.Value).FirstOrDefault();

                var response = await _monederoService.RecargaMonederoConPayPal(int.Parse(resultClaim ?? "0"), data, resultClaimEmail);
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

        [HttpPost]
        [Route("ConfirmaRecargaMonederoConPayPal")]
        [SwaggerOperation(Description = "Servicio para confirmar la recarga realizada al monedero con PayPal")]
        [SwaggerResponse(200, Type = typeof(SuVanResponse<List<ConfirmaRecargaMonederoResponse>>))]
        public async Task<ActionResult> ConfirmaRecargaMonederoConPayPal([FromBody] ConfirmaRecargaMonederoRequest data)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                var resultClaim = identity.Claims.Where(x => x.Type == "userId").Select(x => x.Value).FirstOrDefault();

                var response = await _monederoService.ConfirmaRecargaMonederoConPayPal(int.Parse(resultClaim ?? "0"), data);
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
        #endregion

    }

}

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SUVAN.BackOffice.API.Provider;
using SUVAN.BackOffice.Models.ManejoRespuesta;
using SUVAN.BackOffice.Models.Membresia;
using SUVAN.BackOffice.Models.Monedero;
using SUVAN.BackOffice.Models.Pago;
using SUVAN.BackOffice.Service.Membresia;
using SUVAN.BackOffice.Service.Notificaciones;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;

namespace SUVAN.BackOffice.API.Controllers
{
    [Route("Membresia")]
    [ApiController]
    public class MembresiaController : ControllerBase
    {
        private readonly ISuVanResponseService _suVanResponseService;
        private readonly IMembresiaService _membresiaService;
        private readonly INotificacionCorreoService _notificacionCorreoService;


        public MembresiaController(ISuVanResponseService suVanResponseService, IMembresiaService membresiaService, INotificacionCorreoService notificacionCorreoService)
        {
            _suVanResponseService = suVanResponseService;
            _membresiaService = membresiaService;
            _notificacionCorreoService = notificacionCorreoService;

        }

        [HttpPost]
        [Route("PagaMembresiaConTarjeta")]
        [SwaggerOperation(Description = "Servicio para realizar pago de membresía por medio de tarjeta")]
        [SwaggerResponse(200, Type = typeof(SuVanResponse<List<PagaMembresiaConTarjetaResponse>>))]
        public async Task<ActionResult> PagaMembresiaConTarjeta([FromBody] PagaMembresiaConTarjetaRequest data)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                var resultClaim = identity.Claims.Where(x => x.Type == "userId").Select(x => x.Value).FirstOrDefault();
                var resultClaimEmail = identity.Claims.Where(x => x.Type == "email").Select(x => x.Value).FirstOrDefault();

                var response = await _membresiaService.PagaMembresiaConTarjeta(int.Parse(resultClaim ?? "0"), data, resultClaimEmail);
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
        [Route("ConfirmaPagoMembresiaConTarjeta")]
        [SwaggerOperation(Description = "Servicio para confirmar la recarga realizada al monedero")]
        [SwaggerResponse(200, Type = typeof(SuVanResponse<List<ConfirmaPagoMembresiaConTarjetaResponse>>))]
        public async Task<ActionResult> ConfirmaPagoMembresiaConTarjeta([FromBody] ConfirmaPagoMembresiaConTarjetaRequest data)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                var resultClaim = identity.Claims.Where(x => x.Type == "userId").Select(x => x.Value).FirstOrDefault();

                var response = await _membresiaService.ConfirmaPagoMembresiaConTarjeta(int.Parse(resultClaim ?? "0"), data);
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
        [Route("PagaMembresiaConMonedero")]
        [SwaggerOperation(Description = "Servicio para realizar el pago de membresia por medio del monedero")]

        [SwaggerResponse(200, Type = typeof(SuVanResponse<PagaMembresiaConMonederoResponse>))]
        public async Task<ActionResult> PagaMembresiaConMonedero([FromBody] PagaMembresiaConMonederoRequest data)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                var resultClaim = identity.Claims.Where(x => x.Type == "userId").Select(x => x.Value).FirstOrDefault();
                var resultClaimEmail = identity.Claims.Where(x => x.Type == "email").Select(x => x.Value).FirstOrDefault();

                string code = await Utilities.GeneraCodigos.GetGeneraCodigo(9999);
                var response = await _membresiaService.PagaMembresiaConMonedero(int.Parse(resultClaim ?? "0"), data, resultClaimEmail,code);
                if (response.CodigoMensaje != "200" && response.CodigoMensaje != "206")
                {
                    return _suVanResponseService.Handle(response);
                }
                #region Envio de Correo
                if (response.CodigoMensaje == "206" && response.Mensaje == "Código generado")
                {
                    if (!string.IsNullOrEmpty(resultClaimEmail))
                    {
                        await _notificacionCorreoService.EnviarCodigoPagoMonedero(resultClaimEmail, string.Empty, code, string.Empty, string.Empty);
                    }
                }
                #endregion
                return Ok(response);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }


        #region Controladores para PayPal
        [HttpPost]
        [Route("PagaMembresiaConPayPal")]
        [SwaggerOperation(Description = "Servicio para realizar pago de membresía por medio de PayPal")]
        [SwaggerResponse(200, Type = typeof(SuVanResponse<List<PagaMembresiaConPayPal>>))]
        public async Task<ActionResult> PagaMembresiaConPayPal([FromBody] PagaMembresiaConPayPalRequest data)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                var resultClaim = identity.Claims.Where(x => x.Type == "userId").Select(x => x.Value).FirstOrDefault();
                var resultClaimEmail = identity.Claims.Where(x => x.Type == "email").Select(x => x.Value).FirstOrDefault();

                var response = await _membresiaService.PagaMembresiaConPayPal(int.Parse(resultClaim ?? "0"), data, resultClaimEmail);
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
        [Route("ConfirmaPagoMembresiaConPaypal")]
        [SwaggerOperation(Description = "Servicio para confirmar la recarga realizada al monedero con PayPal")]
        [SwaggerResponse(200, Type = typeof(SuVanResponse<List<ConfirmaPagoMembresiaConTarjetaResponse>>))]
        public async Task<ActionResult> ConfirmaPagoMembresiaConPayPal([FromBody] ConfirmaPagoMembresiaConPayPalRequest data)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                var resultClaim = identity.Claims.Where(x => x.Type == "userId").Select(x => x.Value).FirstOrDefault();

                var response = await _membresiaService.ConfirmaPagoMembresiaConPayPal(int.Parse(resultClaim ?? "0"), data);
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

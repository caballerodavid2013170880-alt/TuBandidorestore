using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SUVAN.BackOffice.API.Provider;
using SUVAN.BackOffice.Models.ManejoRespuesta;
using SUVAN.BackOffice.Models.Pago;
using SUVAN.BackOffice.Models.Ubicacion;
using SUVAN.BackOffice.Service.Monitoreo;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;

namespace SUVAN.BackOffice.API.Controllers
{
    [ApiController]
    [Route("Monitoreo")]

    public class MonitoreoController : ControllerBase
    {
        private readonly ISuVanResponseService _suVanResponseService;
        private readonly IRastreoService _rastreoServicio;


        public MonitoreoController(ISuVanResponseService suVanResponseService, IRastreoService rastreoService)
        {
            _suVanResponseService = suVanResponseService;
            _rastreoServicio = rastreoService;
        }


        [HttpPost]
        [Route("RegistraUbicacion")]
        [SwaggerOperation(Description = "Servicio que nos permite registrar la ubicación GPS de la unidad.")]
        public async Task<ActionResult> RegistraUbicacion([FromBody] RegistraUbicacionRequest data)
        {
            try
            {
                var response = await _rastreoServicio.SetUbicacion(data);
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

        [HttpGet]
        [Route("ObtieneUbicacion/{idCorridaAsignacion}")]
        [SwaggerOperation(Description = "Servicio que nos permite obtener la ubicación GPS de la unidad.")]
        [SwaggerResponse(200, Type = typeof(SuVanResponse<UbicacionResponse>))]
        public async Task<ActionResult> ObtieneUbicacion(int idCorridaAsignacion)
        {
            try
            {
                var response = await _rastreoServicio.GetUbicacion(idCorridaAsignacion);

                return Ok(response);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /*
        [HttpPost]
        [Route("WHSuvanPay")]
        [AllowAnonymous]
        public async Task<ActionResult> WHSuvanPay([FromBody] object data)
        {
            string entradaPagoUnlimit = string.Empty;
            try
            {
                entradaPagoUnlimit = data.ToString();

                var response = await _pagoService.GuardaBitacoraTransaccion(entradaPagoUnlimit);
                if (response)
                {
                    return Ok();
                }
                else
                {
                    return BadRequest();
                }

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
        //public async Task<ActionResult> GetResponsePay()
        //{
        //    try
        //    {
        //        //Stream req = Request.InputStream;
        //        //req.Seek(0, System.IO.SeekOrigin.Begin);
        //        //string json_entrada = new StreamReader(req).ReadToEnd();
        //        return Ok();

        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(StatusCodes.Status500InternalServerError);
        //    }
        //}

        [HttpPost]
        [Route("ConfirmaPago")]
        [SwaggerOperation(Description = "Servicio para confirmar pago")]
        [SwaggerResponse(200, Type = typeof(SuVanResponse<ConfirmarPagoResponse>))]
        public async Task<ActionResult> ConfirmaPago([FromBody] ConfirmarPagoRequest data)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                var resultClaim = identity.Claims.Where(x => x.Type == "userId").Select(x => x.Value).FirstOrDefault();

                var response = await _pagoService.ConfirmaPago(int.Parse(resultClaim ?? "0"), data);
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
        [Route("GeneraPagoMonedero")]
        [SwaggerOperation(Description = "Servicio para realizar pago por medio de monedero")]

        [SwaggerResponse(200, Type = typeof(SuVanResponse<PagoMonederoResponse>))]
        public async Task<ActionResult> GeneraPagoMonedero([FromBody] PagoMonederoRequest data)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                var resultClaim = identity.Claims.Where(x => x.Type == "userId").Select(x => x.Value).FirstOrDefault();
                var resultClaimEmail = identity.Claims.Where(x => x.Type == "email").Select(x => x.Value).FirstOrDefault();
                
                string code = await Utilities.GeneraCodigos.GetGeneraCodigo(9999);
                var response = await _pagoService.GenerarPagoMonedero(int.Parse(resultClaim ?? "0"), resultClaimEmail, data, code);
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

        #region Controladores para PayaPal

        [HttpPost]
        [Route("GeneraPagoConPayPal")]
        [SwaggerOperation(Description = "Servicio para realizar pago por PayPal")]

        [SwaggerResponse(200, Type = typeof(SuVanResponse<PagoResponse>))]
        public async Task<ActionResult> GeneraPagoConPayPal([FromBody] PagoRequest data)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                var resultClaim = identity.Claims.Where(x => x.Type == "userId").Select(x => x.Value).FirstOrDefault();
                var resultClaimEmail = identity.Claims.Where(x => x.Type == "email").Select(x => x.Value).FirstOrDefault();

                var response = await _pagoService.GenerarPagoConPayPal(int.Parse(resultClaim ?? "0"), resultClaimEmail, data);
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
        [Route("ConfirmaPagoPayPal")]
        [SwaggerOperation(Description = "Servicio para confirmar pago")]
        [SwaggerResponse(200, Type = typeof(SuVanResponse<ConfirmarPagoResponse>))]
        public async Task<ActionResult> ConfirmaPagoPayPal([FromBody] ConfirmarPagoRequest data)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                var resultClaim = identity.Claims.Where(x => x.Type == "userId").Select(x => x.Value).FirstOrDefault();

                var response = await _pagoService.ConfirmaPagoPayPal(int.Parse(resultClaim ?? "0"), data);
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
        */
    }
}

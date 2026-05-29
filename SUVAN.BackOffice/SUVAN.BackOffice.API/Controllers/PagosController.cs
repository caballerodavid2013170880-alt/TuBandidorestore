using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SUVAN.BackOffice.API.Provider;
using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.ManejoRespuesta;
using SUVAN.BackOffice.Models.Pago;
using SUVAN.BackOffice.Models.RegistroUsuario;
using SUVAN.BackOffice.Models.UnlimintPay.Pago;
using SUVAN.BackOffice.Models.UnlimintPay.Token;
using SUVAN.BackOffice.Service.Notificaciones;
using SUVAN.BackOffice.Service.Pago;
using SUVAN.BackOffice.Service.PayPal;
using SUVAN.BackOffice.Service.RegistroUsuario;
using SUVAN.BackOffice.Service.UnlimintPay;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;
using static SUVAN.BackOffice.Models.UnlimintPay.Pago.RespuestaValidacionPagoWS;

namespace SUVAN.BackOffice.API.Controllers
{
  [ApiController]
  [Route("Pago")]

  public class PagosController : ControllerBase
  {
    private readonly ISuVanResponseService _suVanResponseService;
    private readonly IUnlimintPayService _unlimintPayService;
    private readonly IPagoService _pagoService;
    private readonly INotificacionCorreoService _notificacionCorreoService;
    private readonly IPayPalService _payPalService;

    public PagosController(ISuVanResponseService suVanResponseService, IUnlimintPayService unlimintPayService, IPagoService pagoService, INotificacionCorreoService notificacionCorreoService, IPayPalService payPalService)
    {
      _suVanResponseService = suVanResponseService;
      _unlimintPayService = unlimintPayService;
      _pagoService = pagoService;
      _notificacionCorreoService = notificacionCorreoService;
      _payPalService = payPalService;
    }


    [HttpPost]
    [Route("GeneraPago")]
    [SwaggerOperation(Description = "Servicio para realizar pago")]

    [SwaggerResponse(200, Type = typeof(SuVanResponse<PagoResponse>))]
    public async Task<ActionResult> GeneraPago([FromBody] PagoRequest data)
    {
      try
      {
        var identity = HttpContext.User.Identity as ClaimsIdentity;
        var resultClaim = identity.Claims.Where(x => x.Type == "userId").Select(x => x.Value).FirstOrDefault();
        var resultClaimEmail = identity.Claims.Where(x => x.Type == "email").Select(x => x.Value).FirstOrDefault();

        var response = await _pagoService.GenerarPago(int.Parse(resultClaim ?? "0"), data, resultClaimEmail);
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
            response.Data.PeticionId = code;
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


  }
}

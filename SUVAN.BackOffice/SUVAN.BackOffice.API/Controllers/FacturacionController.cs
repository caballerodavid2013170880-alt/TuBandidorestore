using Microsoft.AspNetCore.Mvc;
using SUVAN.BackOffice.API.Provider;
using SUVAN.BackOffice.Models.Facturacion;
using SUVAN.BackOffice.Models.ManejoRespuesta;
using SUVAN.BackOffice.Models.Pago;
using SUVAN.BackOffice.Service.Facturacion;
using SUVAN.BackOffice.Service.Pago;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;


namespace SUVAN.BackOffice.API.Controllers
{
    [ApiController]
    [Route("Facturacion")]

    public class FacturacionController  : ControllerBase
    {
        private readonly ISuVanResponseService _suVanResponseService;
        private readonly IFacturacionService _facturacionService;

        public FacturacionController(ISuVanResponseService suVanResponseService, IFacturacionService facturacionService)
        {
            _suVanResponseService = suVanResponseService;
            _facturacionService = facturacionService;
        }


        [HttpPost]
        [Route("GeneraFactura")]
        [SwaggerOperation(Description = "Servicio para timbrar factura")]

        [SwaggerResponse(200, Type = typeof(SuVanResponse<GeneraFacturaResponse>))]
        public async Task<ActionResult> GeneraFactura([FromBody] GeneraFacturaRequest data)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                var resultClaim = identity.Claims.Where(x => x.Type == "userId").Select(x => x.Value).FirstOrDefault();
                var resultClaimEmail = identity.Claims.Where(x => x.Type == "email").Select(x => x.Value).FirstOrDefault();

                var response = await _facturacionService.GenerarFactura(int.Parse(resultClaim ?? "0"), resultClaimEmail, data);
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
        [Route("GuardaDatosFacturacionReceptor")]
        [SwaggerOperation(Description = "Servicio para guardar datos de facturación")]

        [SwaggerResponse(200, Type = typeof(SuVanResponse<GuardaDatosFacturacionReceptorResponse>))]
        public async Task<ActionResult> GuardaDatosFacturacionReceptor([FromBody] GuardaDatosFacturacionReceptorRequest data)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                var resultClaim = identity.Claims.Where(x => x.Type == "userId").Select(x => x.Value).FirstOrDefault();

                var response = await _facturacionService.GuardaDatosFacturacionReceptor(int.Parse(resultClaim ?? "0"), data);
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

        //ObtenRegimenFiscalReceptor

        [HttpGet]
        [Route("ObtenRegimenFiscalReceptor")]
        [SwaggerOperation(Description = "Servicio para obtener el catálogo de datos del Régimen Fiscal del receptor dependiendo de la personalidad jurídica")]

        [SwaggerResponse(200, Type = typeof(SuVanResponse<List<DatosRegimenFiscalUsoCFDIResponse.RegimenFiscalReceptorResponse>>))]
        public async Task<ActionResult> ObtenRegimenFiscalReceptor(string rfc)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                var resultClaim = identity.Claims.Where(x => x.Type == "userId").Select(x => x.Value).FirstOrDefault();

                var response = await _facturacionService.ObtenRegimenFiscalReceptor(int.Parse(resultClaim ?? "0"), rfc);
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
        [Route("ObtenUsoCFDIReceptor")]
        [SwaggerOperation(Description = "Servicio para obtener el catálogo de datos de Uso de CFDI del receptor")]

        [SwaggerResponse(200, Type = typeof(SuVanResponse<List<DatosRegimenFiscalUsoCFDIResponse.RegimenFiscalReceptorResponse>>))]
        public async Task<ActionResult> ObtenUsoCFDIReceptor(string rfc, int idregimenfiscalreceptor)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                var resultClaim = identity.Claims.Where(x => x.Type == "userId").Select(x => x.Value).FirstOrDefault();

                var response = await _facturacionService.ObtenUsoCFDIReceptor(int.Parse(resultClaim ?? "0"),  rfc, idregimenfiscalreceptor);
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
        [Route("ObtenDatosFacturacionReceptor")]
        [SwaggerOperation(Description = "Servicio para obtener los datos de facturación del receptor")]

        [SwaggerResponse(200, Type = typeof(SuVanResponse<ObtenDatosFacturacionReceptorResponse>))]
        public async Task<ActionResult> ObtenDatosFacturacionReceptor()
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                var resultClaim = identity.Claims.Where(x => x.Type == "userId").Select(x => x.Value).FirstOrDefault();

                var response = await _facturacionService.ObtenDatosFacturacionReceptor(int.Parse(resultClaim ?? "0"));
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


        [HttpDelete]
        [Route("RemueveDatosFacturacionReceptor")]
        [SwaggerOperation(Description = "Servicio para eliminar datos de facturación del receptor")]

        [SwaggerResponse(200, Type = typeof(SuVanResponse<string>))]
        public async Task<ActionResult> RemueveDatosFacturacionReceptor(int datosfacturacionreceptorId)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                var resultClaim = identity.Claims.Where(x => x.Type == "userId").Select(x => x.Value).FirstOrDefault();

                var response = await _facturacionService.RemueveDatosFacturacionReceptor(datosfacturacionreceptorId, int.Parse(resultClaim ?? "0"));
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

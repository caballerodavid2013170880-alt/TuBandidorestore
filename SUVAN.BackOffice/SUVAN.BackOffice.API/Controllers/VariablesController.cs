using Microsoft.AspNetCore.Mvc;
using SUVAN.BackOffice.API.Provider;
using SUVAN.BackOffice.Models.ManejoRespuesta;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using SUVAN.BackOffice.Service.Variables;
using SUVAN.BackOffice.Models.Variables;

namespace SUVAN.BackOffice.API.Controllers
{
    [ApiController]
    [Route("Variables")]
    public class VariablesController : ControllerBase
    {
        private readonly ISuVanResponseService _suVanResponseService;
        private readonly IVariablesService _variablesService;


        public VariablesController(ISuVanResponseService suVanResponseService, IVariablesService variablesService)
        {
            _suVanResponseService = suVanResponseService;
            _variablesService = variablesService;
        }
        [HttpGet]
        [Route("ObtenValorVariable")]
        [SwaggerOperation(Description = "Servicio para obtener el valor de variables")]

        [SwaggerResponse(200, Type = typeof(SuVanResponse<ObtenValorVariableResponse>))]
        public async Task<ActionResult> ObtenValorVariable(string codigo, int empresaid)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                var resultClaim = identity.Claims.Where(x => x.Type == "userId").Select(x => x.Value).FirstOrDefault();

                var response = await _variablesService.ObtenValorVariable(codigo, empresaid);
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

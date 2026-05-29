using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SUVAN.BackOffice.API.Provider;
using SUVAN.BackOffice.Models.Auth.User;
using SUVAN.BackOffice.Models.Favoritos;
using SUVAN.BackOffice.Models.ManejoRespuesta;
using SUVAN.BackOffice.Service.Favoritos;
using Swashbuckle.AspNetCore.Annotations;
using System.Net.Http;
using System.Security.Claims;

namespace SUVAN.BackOffice.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class FavoritosController : ControllerBase
    {
        private readonly ISuVanResponseService _suVanResponseService;
        private readonly IFavoritosService _favoritosService;
        public FavoritosController(ISuVanResponseService suVanResponseService,IFavoritosService favoritosService)
        {
            _favoritosService = favoritosService;
            _suVanResponseService = suVanResponseService;
        }

        [HttpGet]
        [Route("ObtenerFavoritos")]
        [SwaggerOperation(Description = "Este servicio regresa una lista con las ubicaciones de los usuarios guardadas como Casa o Trabajo")]
        [SwaggerResponse(200, Type = typeof(SuVanResponse<List<FavoritoModel>>))]
        public async Task<ActionResult> ObtenerFavoritos()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var resultClaim = identity.Claims.Where(x => x.Type == "userId").Select(x => x.Value).FirstOrDefault();
            var response = await _favoritosService.ConsultaFavoritos(resultClaim ?? "0");
            return _suVanResponseService.Handle(response);
        }
        [HttpPost]
        [Route("EstableceFavorito")]
        [SwaggerOperation(Description = "Este servicio se utiliza para que los usuarios actualicen (agregan si no existen o actualizan) una ubicación GPS (latitud,longitud) como Casa o Trabajo")]
        [SwaggerResponse(200, Type = typeof(SuVanResponse<FavoritoModel>))]
        public async Task<IActionResult> EstableceFavorito([FromBody] FavoritoModel model)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var resultClaim = identity.Claims.Where(x => x.Type == "userId").Select(x => x.Value).FirstOrDefault();
            var response = await _favoritosService.EstableceFavorito(resultClaim ?? "0", model);
            return _suVanResponseService.Handle(response);
        }

        [HttpGet]
        [Route("FavoritosPersonales")]
        [SwaggerOperation(Description = "Este servicio regresa una lista con las ubicaciones de los usuarios guardadas en donde él asignó el nombre a las ubicaciones ")]
        [SwaggerResponse(200, Type = typeof(SuVanResponse<List<FavoritoPersonalResponse>>))]
        public async Task<ActionResult> FavoritosPersonales()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var resultClaim = identity.Claims.Where(x => x.Type == "userId").Select(x => x.Value).FirstOrDefault();
            var response = await _favoritosService.FavoritosPersonal(resultClaim ?? "0");
            return _suVanResponseService.Handle(response);
        }

        [HttpPost]
        [Route("CreaFavoritoPersonal")]
        [SwaggerOperation(Description = "Este servicio se utiliza para que los usuarios registren una ubicación GPS (latitud,longitud) o actualicen en donde el usuario pone el nombre de la ubicación")]
        [SwaggerResponse(200, Type = typeof(SuVanResponse<FavoritoPersonalResponse>))]
        public async Task<ActionResult> CreaFavoritoPersonal(FavoritoPersonalModel model)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var resultClaim = identity.Claims.Where(x => x.Type == "userId").Select(x => x.Value).FirstOrDefault();
            var response = await _favoritosService.CreaFavoritoPersonal(resultClaim ?? "0", model);
            return _suVanResponseService.Handle(response);
        }
        [HttpDelete]
        [Route("RemueveFavoritoPersonal")]
        [SwaggerOperation(Description = "Este servicio se utiliza para que los usuarios registren una ubicación GPS (latitud,longitud) en donde el usuario pone el nombre de la ubicación")]
        [SwaggerResponse(200, Type = typeof(SuVanResponse<string>))]
        public async Task<ActionResult> RemueveFavoritoPersonal(string favoritopersonalId)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var resultClaim = identity.Claims.Where(x => x.Type == "userId").Select(x => x.Value).FirstOrDefault();

            var response = await _favoritosService.RemueveFavorito(favoritopersonalId, resultClaim ?? "0");
            return _suVanResponseService.Handle(response);
        }

    }
}

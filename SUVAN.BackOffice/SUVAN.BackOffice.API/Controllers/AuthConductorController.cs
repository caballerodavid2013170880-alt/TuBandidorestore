using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SUVAN.BackOffice.API.Provider;
using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.Auth.User;
using SUVAN.BackOffice.Models.GeneraCodigo;
using SUVAN.BackOffice.Models.ManejoRespuesta;
using SUVAN.BackOffice.Service;
using SUVAN.BackOffice.Service.Conductor;
using SUVAN.BackOffice.Service.Notificaciones;
using SUVAN.BackOffice.Service.RegistroUsuario;
using Swashbuckle.AspNetCore.Annotations;

namespace SUVAN.BackOffice.API
{
    [Route("[controller]")]
    [ApiController]
    public class AuthConductorController : ControllerBase
    {
        private readonly ISuVanResponseService _suVanResponseService;
        private readonly IUserService _userService;
        private readonly IJwtAuthProvider _jwtAuthProvider;
        private readonly IConductorService _conductor;
        private readonly INotificacionCorreoService _notificacionCorreoService;

        public AuthConductorController(IUserService userService, IJwtAuthProvider jwtAuthProvider, IConductorService conductor, ISuVanResponseService suVanResponseService, INotificacionCorreoService notificacionCorreoService)
        {
            _jwtAuthProvider = jwtAuthProvider;
            _userService = userService;
            _conductor = conductor;
            _suVanResponseService = suVanResponseService;
            _notificacionCorreoService = notificacionCorreoService;
        }

        [HttpPost]
        [Route("Login")]
        [SwaggerOperation(Description = "Servicio de generacion de recredenciales")]
        [SwaggerResponse(200, Type = typeof(SuVanResponse<LoginResponse>))]
        [AllowAnonymous]
        public async Task<ActionResult> Login([FromBody] LoginRequest userLogin)
        {
            SuVanResponse<LoginResponse> response = new();
            try
            {
                var result = await _conductor.getInfo(userLogin.UserName, userLogin.Password);
                if (result == null)
                {
                    return ResponseExcepcionesLogin(response,"400", "Email o password incorrectos");
                }
                else
                {
                    if (result.Activo == 0)
                    {
                        return ResponseExcepcionesLogin(response, "401", "Usuario inactivo");
                    }
                        if (result.Validado is null || result.Validado == 0)
                        {
                            //enviar correo y setear campo
                            #region Envio de Correo
                            await EnvioCorreoCodigo(userLogin, result);
                            #endregion
                        return ResponseExcepcionesLogin(response, "206", "Usuario inactivo se ha enviado código para que actives tu cuenta");
                    }
                }
                var dataUser = await _userService.dataConductor(result);
                //_userService.ValidateUser(); //Este metodo retornara el objecto de la BD con la respuesta exitosa fallida
                response = await _jwtAuthProvider.GenerateToken(userLogin, dataUser.Idconductor.ToString(), dataUser.Email);          
                return _suVanResponseService.Handle(response);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

        }

        private ActionResult ResponseExcepcionesLogin(SuVanResponse<LoginResponse> response, string CodigoMensaje, string Mensaje)
        {
            response.CodigoMensaje = CodigoMensaje;
            response.Mensaje = Mensaje;
            return _suVanResponseService.Handle(response);
        }

        private async Task EnvioCorreoCodigo(LoginRequest userLogin, Conductor result)
        {
            GeneraCodigoRequest data = new GeneraCodigoRequest();
            data.email = userLogin.UserName;
            data.password = userLogin.Password;
            var responseEnvioCodigo = await _conductor.GeneraCodigo(data);
            await _notificacionCorreoService.EnviaCodigo(result.Email!, result.Nombre, result.CodigoAuth, string.Empty, string.Empty);
        }

    }
}

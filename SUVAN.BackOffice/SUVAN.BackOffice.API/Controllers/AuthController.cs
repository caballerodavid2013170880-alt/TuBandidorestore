using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.IdentityModel.Tokens;
using SUVAN.BackOffice.API.Controllers;
using SUVAN.BackOffice.API.Provider;
using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.Auth.Token;
using SUVAN.BackOffice.Models.Auth.User;
using SUVAN.BackOffice.Models.GeneraCodigo;
using SUVAN.BackOffice.Models.ManejoRespuesta;
using SUVAN.BackOffice.Models.ObjectUsuarios;
using SUVAN.BackOffice.Service;
using SUVAN.BackOffice.Service.Notificaciones;
using SUVAN.BackOffice.Service.RegistroUsuario;
using SUVAN.BackOffice.Utilities.Tools;
using Swashbuckle.AspNetCore.Annotations;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SUVAN.BackOffice.API
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ISuVanResponseService _suVanResponseService;
        private readonly IUserService _userService;
        private readonly IJwtAuthProvider _jwtAuthProvider;
        private readonly IUsuarioService _usuario;
        private readonly INotificacionCorreoService _notificacionCorreoService;

        public AuthController(IUserService userService, IJwtAuthProvider jwtAuthProvider, IUsuarioService usuario, ISuVanResponseService suVanResponseService, INotificacionCorreoService notificacionCorreoService)
        {
            _jwtAuthProvider = jwtAuthProvider;
            _userService = userService;
            _usuario = usuario;
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
                var usuarioResult = await _usuario.getInfoUsuario(userLogin.UserName, userLogin.Password);
                if (usuarioResult == null)
                {
                    return ResponseExcepcionesLogin(response,"400", "Email o password incorrectos");
                }
                else
                {
                    if (usuarioResult.Activo == 0)
                    {
                        return ResponseExcepcionesLogin(response, "401", "Usuario inactivo");
                    }
                        if (usuarioResult.Validado is null || usuarioResult.Validado == 0)
                        {
                            //enviar correo y setear campo
                            #region Envio de Correo
                            await EnvioCorreoCodigo(userLogin, usuarioResult);
                            #endregion
                        return ResponseExcepcionesLogin(response, "206", "Usuario inactivo se ha enviado código para que actives tu cuenta");
                    }
                }
                var dataUser = await _userService.dataUser(usuarioResult);
                //_userService.ValidateUser(); //Este metodo retornara el objecto de la BD con la respuesta exitosa fallida
                response = await _jwtAuthProvider.GenerateToken(userLogin, dataUser.Idusuario.ToString(), dataUser.Email);          
                return _suVanResponseService.Handle(response);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

        }

        [HttpPost]
        [Route("RefreshToken")]
        [SwaggerOperation(Description = "Servicio de actualización de token")]
        [SwaggerResponse(200, Type = typeof(SuVanResponse<LoginResponse>))]
        [AllowAnonymous]
        public async Task<ActionResult> RefreshToken([FromBody] RefreshTokenRequest data)
        {
            SuVanResponse<LoginResponse> response = new();
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.ReadJwtToken(data.AccessToken);

                var resultClaim = token.Claims.Where(x => x.Type == "userId").Select(x => x.Value).FirstOrDefault();
                var resultClaimEmail = token.Claims.Where(x => x.Type == "email").Select(x => x.Value).FirstOrDefault();

                LoginRequest loginRequest = new LoginRequest { UserName = resultClaimEmail };
                response = await _jwtAuthProvider.GenerateToken(loginRequest, resultClaim.ToString(), resultClaimEmail);
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

        private async Task EnvioCorreoCodigo(LoginRequest userLogin, Usuario usuarioResult)
        {
            string code = await Utilities.GeneraCodigos.GetGeneraCodigo(9999);
            GeneraCodigoRequest data = new GeneraCodigoRequest();
            data.email = userLogin.UserName;
            data.password = userLogin.Password;
            var responseEnvioCodigo = await _usuario.GeneraCodigo(data, code);
            await _notificacionCorreoService.EnviaCodigo(usuarioResult.Email!, usuarioResult.Nombre, code, string.Empty, string.Empty);
        }

    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.AppSettingsModels;
using SUVAN.BackOffice.Models.ViewModel;
using SUVAN.BackOffice.Portal.Helper;
using SUVAN.BackOffice.Service.Contenidos;
using SUVAN.BackOffice.Service.Notificaciones;
using SUVAN.BackOffice.Service.Seguridad;
using SUVAN.BackOffice.Utilities;

namespace SUVAN.BackOffice.Portal.Controllers
{
    public class SeguridadController : Controller
    {
        private readonly ILogger<SeguridadController> _logger;
        private readonly MFASettingsOptions mfaSettings;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IAdminService adminService;
        private readonly INotificacionCorreoService notificacionCorreoService;
        private readonly IPerfilService perfilService;
        private readonly IMFAPortalService mFAPortalService;
        private readonly IAuthenticationClaimService authenticationClaimService;
        private readonly IBitacoraWebService bitacoraWebService;
        private readonly IContenidoService contenidoService;

        public SeguridadController(ILogger<SeguridadController> logger, IOptions<MFASettingsOptions> mfaSettings, IHttpContextAccessor httpContextAccessor,
          IAdminService adminService, INotificacionCorreoService notificacionCorreoService, IPerfilService perfilService,
          IMFAPortalService mFAPortalService, IAuthenticationClaimService authenticationClaimService, IBitacoraWebService bitacoraWebService,
          IContenidoService contenidoService)
        {
            _logger = logger;
            this.mfaSettings = mfaSettings.Value;
            this.httpContextAccessor = httpContextAccessor;
            this.adminService = adminService;
            this.notificacionCorreoService = notificacionCorreoService;
            this.perfilService = perfilService;
            this.mFAPortalService = mFAPortalService;
            this.authenticationClaimService = authenticationClaimService;
            this.bitacoraWebService = bitacoraWebService;
            this.contenidoService = contenidoService;
        }

        [Authorize]
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            await bitacoraWebService.AddBitacora(new Bitacoraloginweb()
            {
                Email = model.Email,
                Fechaaccion = DateTime.UtcNow,
                Detalle = "Solicitud Inicio de sesión",
            });
            var adminResult = await adminService.AutenticarUsuario(model.Email, model.Password);
            if (adminResult == null)
            {
                await bitacoraWebService.AddBitacora(new Bitacoraloginweb()
                {
                    Email = model.Email,
                    Fechaaccion = DateTime.UtcNow,
                    Detalle = "Solicitud Inicio de sesión",
                    Error = "Correo electrónico o contraseña incorrectos"
                });
                ViewBag.ErrorLogin = "Correo electrónico o contraseña incorrectos";
                return View(model);
            }
            await bitacoraWebService.AddBitacora(new Bitacoraloginweb()
            {
                Email = model.Email,
                Fechaaccion = DateTime.UtcNow,
                Detalle = "Inicio de sesión correcto",
            });
            if (mfaSettings.EnableMFA)
            {
                return await MFAControl(model, adminResult);
            }
            await SingInUsuario(adminResult);
            return RedirectToAction("Index", "Home");
        }

        private async Task<IActionResult> MFAControl(LoginViewModel model, Admin adminResult)
        {
            await bitacoraWebService.AddBitacora(new Bitacoraloginweb()
            {
                Email = adminResult.Email!,
                Idusuario = adminResult.Idadmin,
                Fechaaccion = DateTime.UtcNow,
                Detalle = "Inicio Solicitud MFA",
            });
            MFAViewModel mfaModel = new()
            {
                Email = adminResult.Email!
            };
            var envioCodigo = await mFAPortalService.GenerarMFACode(adminResult.Email!);
            if (!string.IsNullOrEmpty(envioCodigo))
            {
                return RedirectToAction("VerificarCodigo", "Seguridad", mfaModel);
            }
            else
            {
                ViewBag.ErrorLogin = "Se generó un problema al generar el codigo de verficación";
                return View(model);
            }
        }

        public async Task<IActionResult> VerificarCodigo(MFAViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Login");
            }
            await bitacoraWebService.AddBitacora(new Bitacoraloginweb()
            {
                Email = model.Email!,
                Fechaaccion = DateTime.UtcNow,
                Detalle = "Carga de pantalla verificacion MFA",
            });
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerificarCodigoUsuario(MFAViewModel model)
        {
            try
            {
                await bitacoraWebService.AddBitacora(new Bitacoraloginweb()
                {
                    Email = model.Email!,
                    Codigo = model.OTP!,
                    Fechaaccion = DateTime.UtcNow,
                    Detalle = "Inicia Verificacion de codigo de Usuario "
                });
                var verificacionAdmin = await mFAPortalService.ValidaMFACode(model.Email!, model.OTP!);
                if (verificacionAdmin != null)
                {
                    await bitacoraWebService.AddBitacora(new Bitacoraloginweb()
                    {
                        Email = model.Email!,
                        Codigo = model.OTP!,
                        Fechaaccion = DateTime.UtcNow,
                        Detalle = "Codigo correcto se realiza login"
                    });
                    var objTemp = new
                    {
                        nombre = verificacionAdmin.Nombre ?? string.Empty,
                        email = verificacionAdmin.Email ?? string.Empty,
                        id = verificacionAdmin.Idadmin,
                        empresas = verificacionAdmin.AdminEmpresas.Select(x => new
                        {
                            id = x.EmpresaIdempresa,
                            nombre = x.EmpresaIdempresaNavigation.Nombre,
                            perfil = x.PerfilIdperfil,
                            principal = x.Principal
                        }),
                        activo = verificacionAdmin.Activo
                    };
                    var jsonVerificacionAdmin = JsonConvert.SerializeObject(objTemp);
                    await bitacoraWebService.AddBitacora(new Bitacoraloginweb()
                    {
                        Email = model.Email!,
                        Codigo = model.OTP!,
                        Fechaaccion = DateTime.UtcNow,
                        Detalle = "Objeto verificacionAdmin",
                        Error = jsonVerificacionAdmin
                    });
                    await SingInUsuario(verificacionAdmin);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewBag.ErrorCodigo = "Código incorrecto o expirado";
                    return View("VerificarCodigo", model);
                }
            }
            catch (Exception ex)
            {
                await bitacoraWebService.AddBitacora(new Bitacoraloginweb()
                {
                    Email = model.Email!,
                    Codigo = model.OTP!,
                    Fechaaccion = DateTime.UtcNow,
                    Detalle = "Error en verificacion de codigo",
                    Error = $"{ex.Message} ---> {ex.InnerException!.Message}"
                });
                ViewBag.ErrorCodigo = "Código incorrecto o expirado";
                return View("VerificarCodigo", model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> ReenviarCodigo([FromBody] MFAViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { success = false, message = "Error al reenviar el código" });
                }
                var envioCodigo = await mFAPortalService.GenerarMFACode(model.Email);
                return Ok(new { success = true, message = "Se envió un nuevo código a su correo." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary> 
        /// Carga las claims del usuario para la autenticacion
        /// </summary>
        /// <param name="adminResult"></param>
        /// <returns></returns>
        private async Task SingInUsuario(Admin adminResult)
        {
            var empresa = adminResult.AdminEmpresas.FirstOrDefault(x => x.Principal == 1);
            await authenticationClaimService.SignInAsync(adminResult, empresa!);
        }

        public async Task<IActionResult> Logout()
        {
            await authenticationClaimService.LogoutAsync();
            return RedirectToAction("Login");
        }

        public IActionResult AccesoDenegado()
        {
            return View();
        }

        public IActionResult Olvido()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Olvido(OlvidoContraViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View();
                }
                var olvidoResult = await adminService.OlvidoContra(model.Email);
                return View("NotificacionCorreo");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorOlvido = ex.Message;
                return View();
            }
        }

        public IActionResult NotificacionCorreo()
        {
            return View();
        }

        public IActionResult ReestablecerContra(ActivarUsuarioViewModel model)
        {
            return View(model);
        }

        public IActionResult ReestablecerContraApp(ActivarUsuarioViewModel model)
        {
            return View(model);
        }

        public IActionResult ReestablecerContraAppCond(ActivarUsuarioViewModel model)
        {
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReestrablecerContraUsuario(ActivarUsuarioViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View("ReestablecerContra", model);
                }
                var usuarioAdmin = await adminService.ActivarAdmin(model, false);
                await SingInUsuario(usuarioAdmin);
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorActivar = ex.Message;
                return View("ReestablecerContra", model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReestrablecerContraUsuarioApp(ActivarUsuarioViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View("ReestablecerContra", model);
                }
                var usuarioAdmin = await adminService.ActivarUsuarioApp(model, false);
                return View("NotificacionCambioApp");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorActivar = ex.Message;
                return View("ReestablecerContra", model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReestrablecerContraUsuarioAppCond(ActivarUsuarioViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View("ReestablecerContraAppCond", model);
                }
                var usuarioAdmin = await adminService.ActivarUsuarioAppCond(model, false);
                return View("NotificacionCambioApp");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorActivar = ex.Message;
                return View("ReestablecerContraAppCond", model);
            }
        }

        public IActionResult NotificacionCambioApp()
        {
            return View();
        }

        public IActionResult VerificarOlvido()
        {
            try
            {
                string valueToken = HttpContext.Request.Query["value"].ToString();
                if (valueToken.Length == 0)
                {
                    throw new Exception("No se encontró el token");
                }
                var email = TokenCorreoPortal.ValidateToken(valueToken);
                ActivarUsuarioViewModel model = new ActivarUsuarioViewModel()
                {
                    Email = email
                };
                return RedirectToAction("ReestablecerContra", "Seguridad", model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Error", "Seguridad");
            }
        }

        public IActionResult VerificarOlvidoApp()
        {
            try
            {
                string valueToken = HttpContext.Request.Query["value"].ToString();
                if (valueToken.Length == 0)
                {
                    throw new Exception("No se encontró el token");
                }
                var email = TokenCorreoPortal.ValidateToken(valueToken);
                ActivarUsuarioViewModel model = new ActivarUsuarioViewModel()
                {
                    Email = email
                };
                return RedirectToAction("ReestablecerContraApp", "Seguridad", model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Error", "Seguridad");
            }
        }

        public IActionResult VerificarOlvidoAppCond()
        {
            try
            {
                string valueToken = HttpContext.Request.Query["value"].ToString();
                if (valueToken.Length == 0)
                {
                    throw new Exception("No se encontró el token");
                }
                var email = TokenCorreoPortal.ValidateToken(valueToken);
                ActivarUsuarioViewModel model = new ActivarUsuarioViewModel()
                {
                    Email = email
                };
                return RedirectToAction("ReestablecerContraAppCond", "Seguridad", model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Error", "Seguridad");
            }
        }

        public IActionResult Verificar()
        {
            try
            {
                string valueToken = HttpContext.Request.Query["value"].ToString();
                if (valueToken.Length == 0)
                {
                    throw new Exception("No se encontró el token");
                }
                var email = TokenCorreoPortal.ValidateToken(valueToken);
                ActivarUsuarioViewModel model = new ActivarUsuarioViewModel()
                {
                    Email = email
                };
                return RedirectToAction("ActivarCuenta", "Seguridad", model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Error", "Seguridad");
            }
        }

        public IActionResult ActivarCuenta(ActivarUsuarioViewModel model)
        {
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActivarCuentaUsuario(ActivarUsuarioViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View("ActivarCuenta", model);
                }
                var usuarioAdmin = await adminService.ActivarAdmin(model);
                await SingInUsuario(usuarioAdmin);
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorLogin = ex.Message;
                return View("ActivarCuenta", model);
            }
        }

        public IActionResult Error(int? statusCode = null)
        {
            if (statusCode.HasValue)
            {
                if (statusCode.Value == 404)
                {
                    ViewBag.ErrorMessage = "La página que estás buscando no existe.";
                }
                // Agragar mas codigos de error.
            }
            else
            {
                if (TempData["ErrorMessage"]!.ToString()!.Length > 0)
                {
                    ViewBag.ErrorMessage = TempData["ErrorMessage"]!.ToString();
                }
            }
            return View();
        }

        public async Task<IActionResult> AvisoDePrivacidad()
        {
            var contenido = await contenidoService.ObtenContenidoPorId(1);
            return View(contenido);
        }

        public async Task<IActionResult> TerminosCondiciones()
        {
            var contenido = await contenidoService.ObtenContenidoPorId(2);
            return View(contenido);
        }
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SUVAN.BackOffice.Models.AppSettingsModels;
using SUVAN.BackOffice.Models.ViewModel.Dashboard;
using SUVAN.BackOffice.Portal.Helper;
using SUVAN.BackOffice.Portal.Models;
using SUVAN.BackOffice.Service.Dashboard;
using SUVAN.BackOffice.Service.Seguridad;
using SUVAN.BackOffice.Service.Comercial;
using System.Diagnostics;
using System.Text.Json;
using SUVAN.BackOffice.Service.MensajeriaService;

namespace SUVAN.BackOffice.Portal.Controllers
{
    //[ValidateAntiForgeryToken]
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IOptions<MFASettingsOptions> mfaSettings;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IAdminService adminService;
        private readonly IAuthenticationClaimService authenticationClaimService;
        private readonly IDashboardService dashboardService;
        private readonly IViajesService viajeService;
        private readonly IMensajeAdminService mensajeAdminService;
        private readonly IUsuarioJerarquiaService usuarioJerarquiaService;


        public HomeController(ILogger<HomeController> logger, IOptions<MFASettingsOptions> mfaSettings, IHttpContextAccessor httpContextAccessor,
          IAdminService adminService, IAuthenticationClaimService authenticationClaimService, IDashboardService dashboardService,
          IViajesService viajeService, IMensajeAdminService mensajeAdminService, IUsuarioJerarquiaService usuarioJerarquiaService)
        {
            _logger = logger;
            this.mfaSettings = mfaSettings;
            this.httpContextAccessor = httpContextAccessor;
            this.adminService = adminService;
            this.authenticationClaimService = authenticationClaimService;
            this.dashboardService = dashboardService;
            this.viajeService = viajeService;
            this.mensajeAdminService = mensajeAdminService;
            this.usuarioJerarquiaService = usuarioJerarquiaService;
        }

        public async Task<IActionResult> Index()
        {
            if (User.Identity!.IsAuthenticated)
            {
                var username = User.Identity.Name;
                var email = User.Claims
                  .FirstOrDefault(i => i.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")!.Value;
            }
            var empresaId = User.GetEmpresaId();
            var perfilId = User.GetPerfilId();
            var rutas = await dashboardService.GetRutasHorarios(empresaId);
            ViewBag.Rutas = JsonSerializer.Serialize(rutas);
            return View();
        }

        public async Task<IActionResult> CambiarEmpresa(int id)
        {
            try
            {
                await adminService.CambiarEmpresa(User.GetUserId(), id);
                var usuario = await adminService.GetAdmin(User.GetUserId());

                var empresa = usuario.AdminEmpresas.FirstOrDefault(x => x.Principal == 1);
                await authenticationClaimService.SignInAsync(usuario, empresa!);
                return RedirectToAction("Index", "Home");
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public async Task<IActionResult> CambiarDeposito(int idEmpresa, int idJerarquia)
        {
            try
            {
                int userId = User.GetUserId();
                if (idEmpresa != User.GetEmpresaId())
                {
                    await adminService.CambiarEmpresa(userId, idEmpresa);
                }
                await usuarioJerarquiaService.CambiarDepositoPrincipal("Admin", userId, idEmpresa, idJerarquia);
                var usuario = await adminService.GetAdmin(userId);

                var empresa = usuario.AdminEmpresas.FirstOrDefault(x => x.Principal == 1);
                await authenticationClaimService.SignInAsync(usuario, empresa!);
                return RedirectToAction("Index", "Home");
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public async Task<IActionResult> DashboardIngresos([FromBody] ViajeIngresosOcupacionFiltro model)
        {
            try
            {
                var result = await dashboardService.GetViajeOcupacionIngreso(model, User.GetEmpresaId());
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> MostrarNuevosMensajes()
        {
            try
            {
                var messages = await mensajeAdminService.GetMessage(User.GetUserId());
                return Ok(new { success = true, data = messages });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> MarcarMensajesLeidos([FromBody] MensajesLeidosModel model)
        {
            try
            {
                var result = await mensajeAdminService.MarcarMensajesLeidos(Convert.ToInt32(model.ConversacionId));
                return Ok(new
                {
                    success = true,
                    data = result
                });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Construccion()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
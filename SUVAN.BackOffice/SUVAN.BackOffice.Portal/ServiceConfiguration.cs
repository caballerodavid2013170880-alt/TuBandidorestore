using SUVAN.BackOffice.Portal.Helper;
using SUVAN.BackOffice.Service.Comercial;
using SUVAN.BackOffice.Service.Configuracion;
using SUVAN.BackOffice.Service.Contenidos;
using SUVAN.BackOffice.Service.Notificaciones;
using SUVAN.BackOffice.Service.Seguridad;
using SUVAN.BackOffice.Service.Monitoreo;
using SUVAN.BackOffice.Service.Ingresos;
using SUVAN.BackOffice.Service.LogEntidades;
using SUVAN.BackOffice.Service.Dashboard;
using SUVAN.BackOffice.Service.MensajeriaService;
using SUVAN.BackOffice.Service.Pago;
using SUVAN.BackOffice.Service.Logistica;

namespace SUVAN.BackOffice.Portal
{
  public static class ServiceConfiguration
  {
    /// <summary>
    /// Metodo de extension para inyectar dependencias de los servicios
    /// </summary>
    /// <param name="services"></param>
    public static void AddCatalogServices(this IServiceCollection services)
    {
      services.AddTransient<ITipoContenidoService, TipoContenidoService>();
      services.AddTransient<IContenidoService, ContenidoService>();
      services.AddTransient<IEmpresasService, EmpresasService>();
      services.AddTransient<IConductorService, ConductorService>();
      services.AddTransient<Service.Conductor.IConductorService, Service.Conductor.ConductorService>();
      services.AddTransient<ITipoVehiculoService, TipoVehiculoService>();
      services.AddTransient<IVehiculoService, VehiculoService>();
      services.AddTransient<IParadasService, ParadasService>();
      services.AddTransient<IRutasService, RutasService>();
      services.AddTransient<ICodigoDescuentoService, CodigoDescuentoService>();
      services.AddTransient<ICorridasService, CorridasService>();
      services.AddTransient<IChoferUnidadService, ChoferUnidadService>();
      services.AddTransient<IVariablesService, VariablesService>();
      services.AddTransient<IReporteIngresosService, ReporteIngresosService>();
      services.AddTransient<IDashboardService, DashboardService>();
      services.AddTransient<INotificacionCorreoService, NotificacionCorreoService>();
      services.AddTransient<ITarifaService, TarifaService>();
      services.AddTransient<IPromocionesService, PromocionesService>();
      services.AddTransient<IViajesService, ViajesService>();
      services.AddTransient<IRastreoService, RastreoService>();
      services.AddTransient<IClientesService, ClientesService>();
      services.AddTransient<IOperadoresService, OperadoresService>();
      services.AddTransient<IConversacionesService, ConversacionesService>();
      services.AddTransient<IUnidadesService, UnidadesService>();
      services.AddTransient<IPoliticasCompensacionService, PoliticasCompensacionService>();
      services.AddTransient<IMensajeAdminService, MensajeAdminService>();
      services.AddTransient<IDepositosDisponibles, DepositosDisponiblesService>();
    }

    /// <summary>
    /// Metodo de extension para inyectar dependencias de los servicios de seguridad y administracion de usuarios
    /// </summary>
    /// <param name="services"></param>
    public static void AddAdminServices(this IServiceCollection services)
    {
      services.AddTransient<IAdminService, AdminService>();
      services.AddTransient<IPermisoService, PermisoService>();
      services.AddTransient<IMenuService, MenuService>();
      services.AddTransient<IUsuarioHelper, UsuarioHelper>();
      services.AddTransient<IRutasHelper, RutasHelper>();
      services.AddTransient<IAuthenticationClaimService, AuthenticationClaimService>();
      services.AddTransient<IPerfilService, PerfilService>();
      services.AddTransient<IMFAPortalService, MFAPortalService>();
      services.AddTransient<IBitacoraWebService, BitacoraWebService>();
      services.AddTransient<ILogTransaccionesEntidadesService, LogTransaccionesEntidadesService>();
      services.AddTransient<INotificacionPushService, NotificacionPushService>();

    }
  }
}

using SUVAN.BackOffice.API.Provider;
using SUVAN.BackOffice.RegistroUsuario.Service;
using SUVAN.BackOffice.Service;
using SUVAN.BackOffice.Service.Contenidos;
using SUVAN.BackOffice.Service.Favoritos;
using SUVAN.BackOffice.Service.Notificaciones;
using SUVAN.BackOffice.Service.Pago;
using SUVAN.BackOffice.Service.Monedero;
using SUVAN.BackOffice.Service.Descuento;
using SUVAN.BackOffice.Service.RegistroUsuario;
using SUVAN.BackOffice.Service.UnlimintPay;
using SUVAN.BackOffice.Service.Viajes;
using SUVAN.BackOffice.Service.Facturacion;
using System.Security.Claims;
using System.Security.Principal;
using SUVAN.BackOffice.Service.Conductor;
using SUVAN.BackOffice.Service.Membresia;
using SUVAN.BackOffice.Service.Variables;
using SUVAN.BackOffice.Service.Corrida;
using SUVAN.BackOffice.Service.PayPal;
using SUVAN.BackOffice.Service.Seguridad;
using SUVAN.BackOffice.Service.Monitoreo;
using SUVAN.BackOffice.Service.MensajeriaService;

namespace SUVAN.BackOffice.API
{
  public static class ServiceConfiguration
  {
    /// <summary>
    /// Metodo de extension para inyectar dependencias de los servicios
    /// </summary>
    /// <param name="services"></param>
    public static void AddCatalogServices(this IServiceCollection services)
    {
      services.AddTransient<ISuVanResponseService, SuVanResponseService>();
      services.AddTransient<ITipoContenidoService, TipoContenidoService>();
      services.AddTransient<IUserService, UserService>();
      services.AddTransient<IJwtAuthProvider, JwtAuthProvider>();
      services.AddTransient<IFavoritosService, FavoritosService>();
      services.AddTransient<INotificacionCorreoService, NotificacionCorreoService>();
      services.AddTransient<IUsuarioService, UsuarioService>();
      services.AddTransient<IContenidoService, ContenidoService>();
      services.AddTransient<ICorridaService, CorridaService>();
      services.AddTransient<IViajeService, ViajeService>();
      services.AddTransient<IUnlimintPayService, UnlimintPayService>();
      services.AddTransient<IPagoService, PagoService>();
      services.AddTransient<IFacturacionService, FacturacionService>();
      services.AddTransient<IMonederoService, MonederoService>();
      services.AddTransient<IDescuentoService, DescuentoService>();
      services.AddTransient<IConductorService, ConductorService>();
      services.AddTransient<IMembresiaService, MembresiaService>();
      services.AddTransient<IVariablesService, VariablesService>();
      services.AddTransient<IPayPalService, PayPalService>();
      services.AddTransient<IConversacionesService, ConversacionesService>();
      // aqui irian mas servicios 
      services.AddTransient<IAdminService, AdminService>();
      services.AddTransient<IPerfilService, PerfilService>();
      services.AddTransient<IPermisoService, PermisoService>();
      services.AddTransient<Service.Configuracion.IEmpresasService, Service.Configuracion.EmpresasService>();

      services.AddTransient<IRastreoService, RastreoService>();
      services.AddTransient<INotificacionPushService, NotificacionPushService>();

      //services.AddSingleton(new SemaphoreSlim(1, 1));
    }
  }
}


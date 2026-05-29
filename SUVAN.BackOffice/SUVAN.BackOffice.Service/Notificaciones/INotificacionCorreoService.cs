using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Service.Notificaciones
{
  public interface INotificacionCorreoService
  {
    Task<bool> OlvidoPassword(string correo, string usuario, string token);

    Task<bool> ActivacionCuenta(string correo, string usuario, string codigo, string token, string ruta);

    Task<bool> EnviaCodigo(string correo, string usuario, string codigo, string token, string ruta);


    Task<bool> InstruccionesRecuperaPassword(string correo, string usuario, string codigo, string token, string ruta);
    Task<bool> EnviarCorreoActivacionPortal(string correo, string usuario, string token);
    Task<bool> EnviarCodigoPortal(string correo, string usuario, string codigo);

    Task<bool> SolicitudActualizaTelefono(string correo, string usuario, string codigo, string token, string ruta);
    Task<bool> EnviarCodigoDescuentoPortal(string correo, string codigo, string descuento, DateTime hasta);

    Task<bool> EnviarCodigoPagoMonedero(string correo, string usuario, string codigo, string token, string ruta);
    Task<bool> EnvioFactura(string correo, string usuario, string codigo, string token, string ruta, Dictionary<string, byte[]> Attachments = null);
    Task<bool> OlvidoPasswordApp(string correo, string usuario, string token);
  }
}

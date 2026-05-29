
using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Utilities.Tools;

namespace SUVAN.BackOffice.Service.Seguridad
{
  public interface IMFAPortalService
  {
    /// <summary>
    /// Elimina el código de autenticación de dos factores asociado a un usuario.
    /// </summary>
    /// <param name="userId">Identificador del usuario.</param>
    /// <returns>Indica si la operación fue exitosa.</returns>
    Task<bool> EliminaMFACodeByUser(int userId);
    /// <summary>
    /// Genera un código de autenticación de dos factores para un usuario.
    /// </summary>
    ///  <param name="email">Email del usuario.</param>
    /// <returns>Código de autenticación generado.</returns>
    Task<string> GenerarMFACode(string email);
    /// <summary>
    /// Valida un código de autenticación de dos factores para un usuario.
    /// </summary>
    /// <param name = "email" > Email del usuario.</param>
    /// <param name="code">Código de autenticación a validar.</param>
    /// <returns>Regresa el Usuario.</returns>
    Task<Admin> ValidaMFACode(string email, string code);
  }
}
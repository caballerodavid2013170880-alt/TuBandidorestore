using Microsoft.EntityFrameworkCore;
using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Service.Notificaciones;
using SUVAN.BackOffice.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Service.Seguridad
{
  public class MFAPortalService : IMFAPortalService
  {
    private readonly SuvanDbContext context;
    private readonly INotificacionCorreoService notificacionCorreoService;
    private readonly IAdminService adminService;
    private readonly IBitacoraWebService bitacoraWebService;

    public MFAPortalService(SuvanDbContext context, INotificacionCorreoService notificacionCorreoService, IAdminService adminService, IBitacoraWebService bitacoraWebService)
    {
      this.context = context;
      this.notificacionCorreoService = notificacionCorreoService;
      this.adminService = adminService;
      this.bitacoraWebService = bitacoraWebService;
    }

    /// <summary>
    /// Genera un código de autenticación de dos factores para un usuario.
    /// </summary>
    /// <param name="email">Email del usuario.</param>
    /// <returns>Código de autenticación generado.</returns>
    public async Task<string> GenerarMFACode(string email)
    {
      var admin = await adminService.GetAdmin(email);
      if (admin == null)
      {
        return string.Empty;
      }
      await EliminaMFACodeByUser(admin.Idadmin);

      var codigo = GeneraCodigos.GeneraCodigo();

      await GuardaCodigo(admin.Idadmin, codigo);

      // Envia codigo de autenticacion
      await notificacionCorreoService.EnviarCodigoPortal(email, admin.Nombre!, codigo);


      await bitacoraWebService.AddBitacora(new Bitacoraloginweb()
      {
        Idusuario = admin.Idadmin,
        Email = email,
        Codigo = codigo,
        Fechaaccion = DateTime.UtcNow,
        Detalle = "Envio de correo de notificacion de codigo MFA",
      });

      return codigo;
    }

    /// <summary>
    /// Guarda el codigo de autenticacion de dos factores para un usuario.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="codigo"></param>
    /// <returns></returns>
    private async Task GuardaCodigo(int userId, string codigo)
    {
      var MFAEntity = new Mfaportal()
      {
        AdminIdadmin = userId,
        Codigo = codigo,
        Expira = DateTime.UtcNow.AddMinutes(10)
      };

      context.Mfaportals.Add(MFAEntity);
      await context.SaveChangesAsync();

      await bitacoraWebService.AddBitacora(new Bitacoraloginweb()
      {
        Idusuario = userId,
        Codigo = codigo,
        Fechaexpiracodigo = MFAEntity.Expira,
        Fechaaccion = DateTime.UtcNow,
        Detalle = "Generacion de codigo MFA",
      });
    }

    /// <summary>
    /// Valida un código de autenticación de dos factores para un usuario.
    /// </summary>
    /// <param name="userId">Identificador del usuario.</param>
    /// <param name="code">Código de autenticación a validar.</param>
    /// <returns>Indica si el código de autenticación es válido.</returns>
    public async Task<Admin> ValidaMFACode(string email, string code)
    {
      try
      {

        await bitacoraWebService.AddBitacora(new Bitacoraloginweb()
        {
          Email = email,
          Codigo = code,
          Fechaaccion = DateTime.UtcNow,
          Detalle = "Verificacion de codigo de Usuario "
        });

        var admin = await adminService.GetAdmin(email.Trim());
        if (admin == null)
        {
          await bitacoraWebService.AddBitacora(new Bitacoraloginweb()
          {
            Email = email,
            Codigo = code,
            Fechaaccion = DateTime.UtcNow,
            Detalle = "Validacion de existencia de usuario",
            Error = "El usuario no existe"
          });
          return null!;
        }

        await bitacoraWebService.AddBitacora(new Bitacoraloginweb()
        {
          Idusuario = admin.Idadmin,
          Email = email,
          Codigo = code,
          Fechaaccion = DateTime.UtcNow,
          Detalle = $"El usuario existe inicia validaicon de codigo, fecha de validacion {DateTime.UtcNow}",
        });

        var MFAEntity = await context.Mfaportals
          .FirstOrDefaultAsync(x => x.AdminIdadmin == admin.Idadmin
          && x.Codigo == code.Trim()
          && x.Expira >= DateTime.UtcNow);

        if (MFAEntity != null)
        {
          await bitacoraWebService.AddBitacora(new Bitacoraloginweb()
          {
            Idusuario = MFAEntity.AdminIdadmin,
            Email = email,
            Codigo = code,
            Fechaaccion = DateTime.UtcNow,
            Detalle = $"El codigo fue validado correctamente {DateTime.UtcNow}",
          });

          await EliminaMFACodeByUser(admin.Idadmin);
          return admin!;
        }

        await bitacoraWebService.AddBitacora(new Bitacoraloginweb()
        {
          Idusuario = admin.Idadmin,
          Email = email,
          Codigo = code,
          Fechaaccion = DateTime.UtcNow,
          Detalle = "El codigo no es valido",
          Error = "El codigo no es valido"
        });

        var intentos = await context.Mfaportals
          .FirstOrDefaultAsync(x => x.AdminIdadmin == admin.Idadmin);

        await bitacoraWebService.AddBitacora(new Bitacoraloginweb()
        {
          Idusuario = admin.Idadmin,
          Email = email,
          Codigo = code,
          Fechaaccion = DateTime.UtcNow,
          Detalle = "no se encontro registro del usuario",
          Error = intentos == null ? $"no se encontro registro en la tabla MFA para el usuario {admin.Idadmin}" : $"Tabla MFA {intentos!.Codigo} {intentos!.Expira.ToString()}"
        });

        return null!;
      }
      catch (Exception)
      {

        throw;
      }

    }

    /// <summary>
    /// Elimina el código de autenticación de dos factores asociado a un usuario.
    /// </summary>
    /// <param name="userId">Identificador del usuario.</param>
    /// <returns>Indica si la operación fue exitosa.</returns>
    public async Task<bool> EliminaMFACodeByUser(int userId)
    {
      var MFAEntity = await context.Mfaportals
        .FirstOrDefaultAsync(x => x.AdminIdadmin == userId);

      if (MFAEntity != null)
      {
        context.Mfaportals.Remove(MFAEntity);
        await context.SaveChangesAsync();
        return true;
      }
      return false;

    }

  }
}

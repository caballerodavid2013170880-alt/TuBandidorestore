using Microsoft.EntityFrameworkCore;
using SUVAN.BackOffice.Database.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using SUVAN.BackOffice.Models.RegistroUsuario;
using SUVAN.BackOffice.Models.Errores;
using SUVAN.BackOffice.Service.RegistroUsuario;
using SUVAN.BackOffice.Models.Auth.User;
using SUVAN.BackOffice.Models.ActivacionUsuario;
using SUVAN.BackOffice.Models.GeneraCodigo;
using static System.Runtime.InteropServices.JavaScript.JSType;
using SUVAN.BackOffice.Models.RecuperaPassword;
using SUVAN.BackOffice.Models.ActualizaPassword;
using Microsoft.Win32;
using SUVAN.BackOffice.Models.ObjectParentResponse;
using System.Globalization;
using System.Security.Principal;
using System.Security.Claims;
using SUVAN.BackOffice.Models.ManejoRespuesta;
using SUVAN.BackOffice.Models.Favoritos;
using SUVAN.BackOffice.Utilities;

using System.Net.Http;
using SUVAN.BackOffice.Models.ActualizaFotografia;
using SUVAN.BackOffice.Models.Viajes;
using SUVAN.BackOffice.Models.ViewModel.Enums;
using SUVAN.BackOffice.Service.Seguridad;

namespace SUVAN.BackOffice.RegistroUsuario.Service
{
  public class UsuarioService : IUsuarioService
  {
    private readonly SuvanDbContext context;
    private readonly IAdminService adminService;

    public UsuarioService(SuvanDbContext context, IAdminService adminService)
    {
      this.context = context;
      this.adminService = adminService;
    }

    public async Task<SuVanResponse<string>> Registro(RegistroUsuarioRequest data, string code)
    {
      SuVanResponse<string> _result = new();
      //ObjectParentResponse resultWS = new ObjectParentResponse();
      //resultWS.Resultado = true;
      Codigopai? CodigopaiEntity = await GetCodigopais(data.codigopais);
      if (CodigopaiEntity == null)
      {
        _result.CodigoMensaje = "400";
        _result.Mensaje = "Código de país incorrecto";
        return _result;
      }

      DateTime expiracion = SUVAN.BackOffice.Utilities.GeneraCodigos.ExpiracionCodigoActivacion();
      var entity = new Database.Entities.Usuario()
      {
        Email = data.email,
        Hashpass = data.password.GetHashSHA256(),
        CodigopaisIdcodigopais = data.codigopais,
        Telefono = data.telefono,
        Fecharegistro = DateTime.Now,
        CodigoAuth = code,
        Activo = 1,
        Validado = 0,
        CodigoExp = expiracion,
        Nombre = data.nombre,
      };
      context.Usuarios.Add(entity);
      await context.SaveChangesAsync();

      _result.CodigoMensaje = "200";
      _result.Mensaje = "Registro exitoso";
      _result.Data = null;
      return _result;
    }


        public async Task<SuVanResponse<string>> Firebase(int UserID, string firebaseid)
        {
            SuVanResponse<string> _result = new();
            Usuario? info = await context.Usuarios.Where(x => x.Idusuario == UserID).FirstOrDefaultAsync();
            if (info != null)
            {
                info.FirebaseId = firebaseid;
                context.SaveChanges();
                _result.CodigoMensaje = "200";
                _result.Mensaje = "Activación exitosa";
                _result.Data = null;
            }
            else
            {
                _result.CodigoMensaje = "400";
                _result.Mensaje = "Error en la información";
                _result.Data = null;
            }

            return _result;
        }


        public async Task<Codigopai?> GetCodigopais(int? codigopais)
    {
      return await context.Codigopais.Where(x => x.Idcodigopais == codigopais).FirstOrDefaultAsync();
    }

    public async Task<SuVanResponse<string>> Activacion(ActivacionUsuarioRequest data)
    {
      SuVanResponse<string> _result = new();
      Usuario? info = await context.Usuarios.Where(x => x.Email == data.email).FirstOrDefaultAsync();
      if (info != null)
      {
        info.Validado = 1;
        info.CodigoAuth = null;
        info.CodigoExp = null;
        context.SaveChanges();
        _result.CodigoMensaje = "200";
        _result.Mensaje = "Activación exitosa";
        _result.Data = null;
      }
      return _result;
    }


    public async Task<SuVanResponse<string>> GeneraCodigo(GeneraCodigoRequest data, string code)
    {
      SuVanResponse<string> _result = new();
      Usuario? info = await context.Usuarios.Where(x => x.Email == data.email && data.password == data.password).FirstOrDefaultAsync();
      if (info != null)
      {
        DateTime expiracion = SUVAN.BackOffice.Utilities.GeneraCodigos.ExpiracionCodigoActivacion();
        info.CodigoAuth = code;
        info.CodigoExp = expiracion;
        context.SaveChanges();

        _result.CodigoMensaje = "200";
        _result.Mensaje = "Código generado exitoso";
        _result.Data = null;
      }
      return _result;
    }


    public async Task<SuVanResponse<string>> RecuperaPassword(RecuperaPasswordRequest data)
    {
      SuVanResponse<string> _result = new();
      var user = await adminService.OlvidoContraUsuarioApp(data.email);

      if (user)
      {
        _result.CodigoMensaje = "200";
        _result.Mensaje = "Recuperación exitosa";
        _result.Data = null;

      }
      return _result;

    }

    public async Task<SuVanResponse<string>> ActualizaPassword(ActualizaPasswordRequest data)
    {
      SuVanResponse<string> _result = new();
      Usuario? info = await context.Usuarios.Where(x => x.Email == data.email).FirstOrDefaultAsync();
      if (info != null)
      {
        info.Hashpass = data.nuevopassword.GetHashSHA256();
        context.SaveChanges();
        _result.CodigoMensaje = "200";
        _result.Mensaje = "Actualización exitosa";
        _result.Data = null;
      }
      return _result;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="usuario"></param>
    /// <param name="email"></param>
    /// <returns></returns>
    public async Task<Usuario> getInfoUsuario(string email, string password)
    {
      var usuario = await context.Usuarios
          .Where(x => (x.Email == email)
                   && (x.Hashpass == (!string.IsNullOrEmpty(password) ? password.GetHashSHA256() : x.Hashpass))
          ).FirstOrDefaultAsync();

      return usuario!;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="usuario"></param>
    /// <param name="email"></param>
    /// <returns></returns>
    public async Task<Usuario> getInfoUsuarioID(int UserID)
    {
      var usuario = await context.Usuarios
          .Where(x => x.Idusuario == UserID)
          .FirstOrDefaultAsync();

      return usuario!;
    }

    public async Task<SuVanResponse<PerfilUsuarioModel>> ObtenerPerfil(int userId)
        {

            DateTime hoy = DateTime.Now;

      SuVanResponse<PerfilUsuarioModel> response = new();
      PerfilUsuarioModel? result = await (from u in context.Usuarios
                                          join m in context.Membresia on u.Idusuario equals m.UsuarioIdusuario into um
                                          where u.Idusuario == userId
                                           && u.Activo == 1
                                           && u.Validado == 1
                                          select new PerfilUsuarioModel()
                                          {
                                            Nombre = u.Nombre,
                                            Telefono = u.Telefono,
                                            Email = u.Email,
                                            Codigopais = u.CodigopaisIdcodigopais,
                                            Fotografia = context.Fotografia.Where(k => k.UsuarioIdusuario == userId).Select(x => x.Imagen).FirstOrDefault(),
                                            membresia = context.Membresia.Where(k => k.UsuarioIdusuario == userId && k.Hasta >= hoy).OrderByDescending(x => x.Hasta).ToList().Count == 0 ? false : true,
                                            VigenciaMembresia = (context.Membresia.Where(k => k.UsuarioIdusuario == userId && k.Hasta >= hoy).OrderByDescending(x => x.Hasta).Select(x => x.Hasta).FirstOrDefault()??DateTime.Now).ToString("dd-MM-yyyy"),
                                          }).FirstOrDefaultAsync();


	  response.CodigoMensaje = "200";
      response.Mensaje = "Búsqueda exitosa";

      response.Data = result;
      return response;
    }

    public async Task<SuVanResponse<ActualizaPerfilRequest>> ActualizaPerfil(int userId, ActualizaPerfilRequest model, string code)
    {
      SuVanResponse<ActualizaPerfilRequest> response = new();
      Usuario? perfilEntity = context.Usuarios.FirstOrDefault(x => x.Idusuario == userId);

      model.CodigoCambioTelefono = !model.CodigoCambioTelefono.Equals("0000") ? model.CodigoCambioTelefono : string.Empty;
      ActualizaPerfilRequest? result = new ActualizaPerfilRequest();

      if (perfilEntity != null)
      {
        if ((perfilEntity.Telefono != model.Telefono || perfilEntity.CodigopaisIdcodigopais != model.Codigopais) && perfilEntity.CodigoAuth != model.CodigoCambioTelefono)
        //if ((perfilEntity.Telefono != model.Telefono || perfilEntity.CodigopaisIdcodigopais != model.Codigopais) && !string.IsNullOrEmpty(perfilEntity.CodigoAuth))
        {
          //1 validamos que el codigo que nos mandan no venga vacio
          if (!string.IsNullOrEmpty(model.CodigoCambioTelefono))
          {
            //2 Validamos si los códigos son diferentes de nulo o vacios
            if (!string.IsNullOrEmpty(perfilEntity.CodigoAuth) && !string.IsNullOrEmpty(model.CodigoCambioTelefono))
            {
              //3 Si el código expiro y son diferentes mandamos que el codigo ha sido expirado
              if (DateTime.Now > perfilEntity.CodigoExp && perfilEntity.CodigoAuth != model.CodigoCambioTelefono)
              {
                response.CodigoMensaje = "400";
                response.Mensaje = "Código expirado";
                return response;
              }

              //4 Validamos la expiración sigue vigente pero son codigos diferentes
              if (perfilEntity.CodigoExp > DateTime.Now && perfilEntity.CodigoAuth != model.CodigoCambioTelefono)
              {
                response.CodigoMensaje = "400";
                response.Mensaje = "Código no válido";
                return response;
              }
            }
          }


          DateTime expiracion = SUVAN.BackOffice.Utilities.GeneraCodigos.ExpiracionCodigoActivacion();
          //enviamos correo y actualizamos en bd el codigo que mandamos
          perfilEntity.CodigoAuth = code;
          perfilEntity.CodigoExp = expiracion;
          response.Mensaje = "Solicitud de actualización exitosa";
          response.CodigoMensaje = "206";

          result.Nombre = perfilEntity.Nombre;
          result.Email = perfilEntity.Email;
          result.Telefono = perfilEntity.Telefono;
          result.Codigopais = perfilEntity.CodigopaisIdcodigopais;
          response.Data = result;
        }
        else
        {
          if ((!string.IsNullOrEmpty(model.CodigoCambioTelefono) && model.CodigoCambioTelefono != "0000") && perfilEntity.CodigoAuth != null)
          {
            if (perfilEntity.CodigoAuth != model.CodigoCambioTelefono)
            {
              response.CodigoMensaje = "400";
              response.Mensaje = "Código no válido";
              return response;
            }
            if (DateTime.Now > perfilEntity.CodigoExp)
            {
              response.CodigoMensaje = "400";
              response.Mensaje = "Código expirado";
              return response;
            }
          }
          perfilEntity.Nombre = model.Nombre;
          perfilEntity.Email = model.Email;
          perfilEntity.Telefono = !string.IsNullOrEmpty(model.CodigoCambioTelefono) ? model.Telefono : perfilEntity.Telefono;
          perfilEntity.CodigopaisIdcodigopais = model.Codigopais;
          perfilEntity.CodigoAuth = null;
          perfilEntity.CodigoExp = null;

          result.Nombre = model.Nombre;
          result.Email = model.Email;
          result.Telefono = model.Telefono;
          result.Codigopais = model.Codigopais;
          response.Data = result;
          response.Mensaje = "Actualización exitosa";
          response.CodigoMensaje = "200";
        }
      }

      context.Usuarios.Entry(perfilEntity);
      await context.SaveChangesAsync();
      return response;
    }

    public async Task<SuVanResponse<string>> ActualizaFotografia(int userId, ActualizaFotografiaRequest data)
    {
      SuVanResponse<string> response = new();
      Fotografium? perfilEntity = context.Fotografia.FirstOrDefault(x => x.UsuarioIdusuario == userId);
      if (perfilEntity == null)
      {
        var fotografiaEntity = new Database.Entities.Fotografium()
        {
          UsuarioIdusuario = userId,
          Imagen = data.Imagen,
          Fecharegistro = DateTime.Now,
          Activo = 1
        };
        context.Fotografia.Add(fotografiaEntity);
      }
      else
      {
        perfilEntity.Imagen = data.Imagen;
        context.Fotografia.Entry(perfilEntity);
      }
      await context.SaveChangesAsync();

      response.Data = null;
      response.Mensaje = "Actualización exitosa";
      response.CodigoMensaje = "200";

      return response;
    }

    public async Task<SuVanResponse<ViajeCalificacion>> CalificaConductor(ViajeCalificacion data)
    {

      CalificacionConductor calif = await context.CalificacionConductors.Where(c => c.ViajeIdviaje == data.ReservacionId).FirstOrDefaultAsync();

      SuVanResponse<ViajeCalificacion> response = new();
      response.CodigoMensaje = "400";
      response.Mensaje = "Datos incorrectos";

      if (calif == null)
      {
        calif = await (from a in context.Viajes
                       join b in context.CorridaAsignacions on a.CorridaAsignacionIdcorridaAsignacion equals b.IdcorridaAsignacion
                       where a.Idviaje == data.ReservacionId
                       && (a.EstatusviajeIdestatusviaje == Convert.ToInt32(EnumEstatusViaje.EN_CURSO)
                       || a.EstatusviajeIdestatusviaje == Convert.ToInt32(EnumEstatusViaje.FINALIZADO)
                       || a.EstatusviajeIdestatusviaje == Convert.ToInt32(EnumEstatusViaje.PERDIDO)
                       )
                       select new CalificacionConductor
                       {
                         ViajeIdviaje = a.Idviaje,
                         UsuarioIdusuario = a.UsuarioIdusuario,
                         ConductorIdconductor = b.ConductorIdconductor,
                         Calificacion = data.Calificacion,
                         Mensaje = data.Mensaje
                       }
            ).FirstOrDefaultAsync();

        if (calif != null)
        {
          context.CalificacionConductors.Add(calif);
          await context.SaveChangesAsync();

          response.Data = data;
          response.CodigoMensaje = "200";
          response.Mensaje = "Calificación de conductor realizada con exito";
        }
      }

      return response;
    }






  }
}

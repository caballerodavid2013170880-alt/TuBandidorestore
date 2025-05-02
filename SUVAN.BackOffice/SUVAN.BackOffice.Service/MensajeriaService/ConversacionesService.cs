using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RestSharp;
using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.AppSettingsModels;
using SUVAN.BackOffice.Models.ManejoRespuesta;
using SUVAN.BackOffice.Models.Mensajeria;
using SUVAN.BackOffice.Service.Notificaciones;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SUVAN.BackOffice.Service.MensajeriaService
{
  public class ConversacionesService : IConversacionesService
  {
    private readonly SuvanDbContext _context;
    private readonly INotificacionPushService _notificacionPushService;
    public ConversacionesService(SuvanDbContext context, INotificacionPushService pNotificacionPushService)
    {
      _context = context;
      _notificacionPushService = pNotificacionPushService;
    }
    public async Task<SuVanResponse<List<ConversacionModel>>> ConsultaConversaciones(int usuarioAdmin)
    {
      SuVanResponse<List<ConversacionModel>> response = new();
      response.CodigoMensaje = "200";

      response.Data = await (from o in _context.Conversacionportals.Include(x => x.Conversacionusuarios)
                             where o.UsuarioCreacion == usuarioAdmin && o.EstatusConversacion == 1
                             select new ConversacionModel()
                             {
                               ConversacionId = o.ConversacionId,
                               TipoConversacion = o.TipoConversacion,
                               EmpresaId = o.EmpresaId,
                               RutaId = o.Ruta,
                               OperadorId = o.Operador,

                               NombreConversacion = o.Titulo,
                               ConversacionCerrada = o.EstatusConversacion == 0,
                               UsuarioCreacion = (from u in _context.Admins
                                                  where u.Idadmin == usuarioAdmin
                                                  select u.Nombre).First(),
                               FechaCreacion = o.FechaHoraCreacion



                             }).OrderByDescending(x => x.FechaCreacion).ToListAsync();
      return response;
    }

    public async Task<SuVanResponse<List<ConversacionModel>>> ConsultaConversacionesUsuario(int usuario_id)
    {
      SuVanResponse<List<ConversacionModel>> response = new();
      response.CodigoMensaje = "200";


      var conversaciones = await (from o in _context.Conversacionportals
                             .Include(x => x.Conversacionusuarios)
                             .Where(z => z.Conversacionusuarios.Any(y => y.UsuarioId == usuario_id))
                                  select new ConversacionModel()
                                  {
                                    ConversacionId = o.ConversacionId,
                                    Estatus = (int)o.EstatusConversacion,
                                    TipoConversacion = o.TipoConversacion,
                                    EmpresaId = o.EmpresaId,
                                    RutaId = o.Ruta,
                                    OperadorId = o.Operador,
                                    NombreConversacion = o.Titulo,
                                    ConversacionCerrada = o.EstatusConversacion == 0,
                                    UsuarioCreacion = (from u in _context.Admins
                                                       where u.Idadmin == o.UsuarioCreacion
                                                       select u.Nombre).First(),
                                    FechaCreacion = o.FechaHoraCreacion
                                  }).OrderByDescending(x => x.FechaCreacion).ToListAsync();

      // regresa solo conversaciones con estatus = 1
      response.Data = conversaciones.Where(x => x.Estatus == 1).ToList();

      return response;
    }

    public async Task<List<BandejaConversacionesModel>> ConsultaBandeja(int usuarioAdmin, int? conversacionId)
    {
      List<BandejaConversacionesModel> response = new();
      response = await (from o in _context.Conversacionportals.Include(x => x.Conversacionusuarios)
                        where o.UsuarioCreacion == usuarioAdmin && o.EstatusConversacion == 1 && (!conversacionId.HasValue || o.ConversacionId == conversacionId.Value)
                        select new BandejaConversacionesModel()
                        {
                          ConversacionId = o.ConversacionId,
                          TipoConversacion = o.TipoConversacion,
                          NombreConversacion = o.Titulo,
                          TotalMensajes = o.Conversacionmensajes.Select(x => x.MensajeId).Distinct().Count(),
                          FechaCreacion = o.FechaHoraCreacion
                        }).OrderByDescending(x => x.FechaCreacion).ToListAsync();
      foreach (var conversacion in response)
      {
        conversacion.DiferenciaTiempo = ObtenerDiferenciaTiempo(conversacion.FechaCreacion);
        if (conversacion.TipoConversacion == 1)
        {
          conversacion.Abreviatura = "R";
          conversacion.NombreTipo = "Ruta";
          conversacion.Estilo = "info";
        }
        else if (conversacion.TipoConversacion == 2)
        {
          conversacion.Abreviatura = "O";
          conversacion.NombreTipo = "Operador";
          conversacion.Estilo = "success";
        }
        else
        {
          conversacion.Abreviatura = "G";
          conversacion.NombreTipo = "Mensaje Global";
          conversacion.Estilo = "danger";
        }
      }
      return response;
    }
    public async Task<SuVanResponse<int>> ModificarEstatus(int conversacionId, int estatus)
    {
      SuVanResponse<int> response = new();
      response.CodigoMensaje = "200";

      var entity = _context.Conversacionportals.FirstOrDefault(x => x.ConversacionId == conversacionId);
      if (entity != null)
      {
        entity.EstatusConversacion = (ulong)(estatus);
        _context.Entry(entity).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        response.Mensaje = "Se realizo la actualizacion del estatus exitosamente";
        response.Data = 1;
      }
      else
      {
        response.Mensaje = "Conversacion no encontrada";
        response.Data = 0;
      }

      return response;
    }
    public async Task<SuVanResponse<ConversacionConexionModel>> ConsultaIdConexion(int conversacion_id)
    {
      SuVanResponse<ConversacionConexionModel> response = new();
      response.CodigoMensaje = "200";


      var entity = await _context.Conversacionconexions.FirstAsync(x => x.ConversacionId == conversacion_id);
      if (entity != null)
      {
        response.CodigoMensaje = "200";
        response.Mensaje = "Información encontrada";
        response.Data = await (from o in _context.Conversacionportals
                               where o.ConversacionId == entity.ConversacionId
                               select new ConversacionConexionModel()
                               {
                                 ConversacionId = conversacion_id,
                                 NombreConversacion = o.Titulo,
                                 ConexionId = entity.ConexionId,
                                 TipoConversacion = o.TipoConversacion

                               }).FirstAsync();

      }
      else
      {
        response.CodigoMensaje = "402";
        response.Mensaje = "Información no encontrada";

      }
      return response;
    }
    public async Task<SuVanResponse<ConversacionModel>> NuevaConversacion(ConversacionModel model)
    {
      SuVanResponse<ConversacionModel> response = new();
      using (var transaction = _context.Database.BeginTransaction())
      {
        try
        {
          List<Conversacionusuario> listaUsuarios = new List<Conversacionusuario>();
          var entity = new Conversacionportal()
          {
            TipoConversacion = model.TipoConversacion,
            EmpresaId = model.EmpresaId,
            Ruta = model.RutaId,
            Operador = model.OperadorId,
            UsuarioCreacion = model.UsuarioIdCreacion,
            Titulo = model.NombreConversacion,
            EstatusConversacion = 1,
            FechaHoraCreacion = DateTime.Now
          };
          await _context.Conversacionportals.AddAsync(entity);
          await _context.SaveChangesAsync();
          if (model.TipoConversacion == 2)
          {
            listaUsuarios.Add(new Conversacionusuario()
            {
              ConversacionId = entity.ConversacionId,
              UsuarioId = model.OperadorId.Value,
              TipoUsuario = "Operador"
            });
          }
          else
          {
            listaUsuarios = (from co in _context.Corrida
                             join coa in _context.CorridaAsignacions on co.Idcorrida equals coa.CorridaIdcorrida
                             join con in _context.Conductors on coa.ConductorIdconductor equals con.Idconductor
                             where co.EmpresaIdempresa == model.EmpresaId &&
                             (!model.RutaId.HasValue || co.RutaIdruta == model.RutaId) &&
                             (!model.OperadorId.HasValue || coa.ConductorIdconductor == model.OperadorId) &&
                             con.Validado == 1 && con.Activo == 1
                             select con).GroupBy(x => x.Idconductor, (g, k) => new Conversacionusuario()
                             {
                               ConversacionId = entity.ConversacionId,
                               UsuarioId = g,
                               TipoUsuario = "Operador"

                             }).ToList();

          }


          await _context.Conversacionusuarios.AddRangeAsync(listaUsuarios);
          await _context.SaveChangesAsync();
          await transaction.CommitAsync();

          response.CodigoMensaje = "200";
          response.Mensaje = "Información guardada con exito";
          response.Data = await (from o in _context.Conversacionportals.Include(x => x.Conversacionusuarios)
                                 where o.ConversacionId == entity.ConversacionId
                                 select new ConversacionModel()
                                 {
                                   ConversacionId = o.ConversacionId,
                                   TipoConversacion = o.TipoConversacion,
                                   NombreConversacion = o.Titulo,
                                   ConversacionCerrada = o.EstatusConversacion == 0,
                                   UsuarioCreacion = (from u in _context.Admins
                                                      where u.Idadmin == o.UsuarioCreacion
                                                      select u.Nombre).First(),
                                   FechaCreacion = o.FechaHoraCreacion,
                                   Usuarios = (from t in _context.Conductors.Include(n => n.FotoConductor)
                                               join z in o.Conversacionusuarios on t.Idconductor equals z.UsuarioId
                                               select new UsuariosConversacion()
                                               {
                                                 UsuarioId = t.Idconductor,
                                                 NombreUsuario = t.Nombre ?? string.Empty,
                                                 ImagenPerfil = t.FotoConductor != null ? t.FotoConductor.Imagen : string.Empty,
                                               }).ToList()



                                 }).FirstAsync();

        }
        catch (Exception ex)
        {
          response.CodigoMensaje = "500";
          response.Mensaje = ex.Message;
          await transaction.RollbackAsync();
        }

      }
      return response;
    }
    public async Task RegistraMensaje(int conversacionId, int userId, int tipoUsuario, string mensaje)
    {
      var ltUsuarios = _context.Conversacionusuarios.Where(con => con.ConversacionId == conversacionId).ToList();
      var entityMessage = new Mensaje()
      {
        Comentario = mensaje,
        UsuarioId = userId,
        TipoUsuario = tipoUsuario,
        FechaHoraCreacion = DateTime.Now,
      };

      _context.Mensajes.Add(entityMessage);
      _context.SaveChanges();
      var ltMensajes = ltUsuarios.Select(x => new Conversacionmensaje()
      {
        ConversacionId = conversacionId,
        MensajeId = entityMessage.MensajeId,
        UsuarioId = x.UsuarioId,
      });
      _context.Conversacionmensajes.AddRange(ltMensajes);
      await _context.SaveChangesAsync();

      #region NOTIFICACIONES PUSH
      //********************************************************SECCION PARA COLOCAR TODO EL CORE DE NOTIFICACIONES PUSH**************************//
      var vIds = ltUsuarios.Select(x => x.UsuarioId).ToArray();
      var lConductores = _context.Conductors.Where(z => vIds.Contains(z.Idconductor)).ToList();

      lConductores.ForEach(x =>
      {
        // Se omite la notificacion al operador que envio el mensaje
        if ((x.FirebaseId ?? "") != string.Empty && x.Idconductor != userId)
        {
          Models.Notification.DataChat dato = new()
          {
            comando = "chat",
            conversacion_id = conversacionId
          };

          _notificacionPushService.EnvioNotificacion(x.FirebaseId, dato, "Mensajeria SUVAN", mensaje, "OpenChat");
        }
      });
      #endregion
    }
    public async Task RegistraActualizaConexion(int conversacionId, string conexionId)
    {
      var conversacionConexion = _context.Conversacionconexions.Where(x => x.ConversacionId == conversacionId).FirstOrDefault();
      if (conversacionConexion != null)
      {
        conversacionConexion.ConexionId = conexionId;
        _context.Entry<Conversacionconexion>(conversacionConexion).State = EntityState.Modified;
      }
      else
      {
        var entityConexion = new Conversacionconexion()
        {
          ConversacionId = conversacionId,
          ConexionId = conexionId,
          TokenAcceso = Guid.NewGuid().ToString(),
          EstatusConexion = 1,
          FechaHoraCreacion = DateTime.Now,
        };
        _context.Conversacionconexions.Add(entityConexion);
      }
      await _context.SaveChangesAsync();

    }
    public async Task<SuVanResponse<TipoConversacion>> ObtenerTipoConversacion(int tipo, int? rutaId, int? operadorId)
    {
      SuVanResponse<TipoConversacion> response = new();
      switch (tipo)
      {
        case 1:
          var entidadRuta = await _context.Ruta.FirstAsync(x => x.Idruta == rutaId.Value);
          response.Data = new TipoConversacion()
          {
            Abreviatura = "R",
            NombreTipo = "Ruta",
            Nombre = entidadRuta?.Nombre,
            Estilo = "info"
          };
          break;
        case 2:
          var entidadConductor = _context.Conductors.First(x => x.Idconductor == operadorId.Value);
          response.Data = new TipoConversacion()
          {
            Abreviatura = "O",
            NombreTipo = "Operador",
            Nombre = $"{entidadConductor.Nombre}",
            Estilo = "success"
          };
          break;
        default:
          response.Data = new TipoConversacion()
          {
            Abreviatura = "G",
            NombreTipo = "Global",
            Nombre = "Mensaje Global",
            Estilo = "danger"
          };
          break;
      }
      return response;
    }
    public async Task<SuVanResponse<ConversacionModel>> ObtenerConversacion(MensajeConversacion model)
    {
      SuVanResponse<ConversacionModel> response = new();
      try
      {
        if (model.ConversacionId.HasValue)
        {
          int vPrimerUsuario = await (from uc in _context.Conversacionusuarios
                                      where uc.ConversacionId == model.ConversacionId
                                      select uc.UsuarioId).FirstOrDefaultAsync();

          response.Data = await (from o in _context.Conversacionportals.Include(x => x.Conversacionmensajes)
                                 where o.ConversacionId == model.ConversacionId
                                 select new ConversacionModel()
                                 {
                                   ConversacionId = o.ConversacionId,
                                   Estatus = (int)o.EstatusConversacion,
                                   EmpresaId = o.EmpresaId,
                                   OperadorId = o.Operador,
                                   RutaId = o.Ruta,
                                   TipoConversacion = o.TipoConversacion,
                                   NombreConversacion = o.Titulo,
                                   ConversacionCerrada = o.EstatusConversacion == 0,
                                   UsuarioCreacion = (from u in _context.Admins
                                                      where u.Idadmin == o.UsuarioCreacion
                                                      select u.Nombre).First(),
                                   UsuarioIdCreacion = o.UsuarioCreacion,
                                   FechaCreacion = o.FechaHoraCreacion,
                                   Mensajes = (from z in o.Conversacionmensajes
                                               join msg in _context.Mensajes on z.MensajeId equals msg.MensajeId
                                               where z.UsuarioId == vPrimerUsuario
                                               select new MensajeModel()
                                               {
                                                 MensajeId = msg.MensajeId,
                                                 Comentario = msg.Comentario ?? "",
                                                 UsuarioId = msg.UsuarioId,
                                                 NombreUsuario = (msg.TipoUsuario == 0 ?
                                                        (from u in _context.Conductors
                                                         where u.Idconductor == msg.UsuarioId
                                                         select u.Nombre).First()
                                                        : (from u in _context.Admins
                                                           where u.Idadmin == msg.UsuarioId
                                                           select u.Nombre).First()
                                                   ),
                                                 FechaCreacion = msg.FechaHoraCreacion.Value,
                                               }).OrderBy(l => l.FechaCreacion).ToList()
                                 }).FirstAsync();
        }
        else
        {
          var entityConversacion = (from o in _context.Conversacionportals
                                    where o.EmpresaId == model.EmpresaId &&
                                    o.TipoConversacion == model.TipoConversacion && (o.Ruta == model.RutaId && o.Operador == model.OperadorId && o.EstatusConversacion == 1 && o.UsuarioCreacion == model.UsuarioIdCreacion)
                                    select o).FirstOrDefault();

          if (entityConversacion != null)
          {
            response.Data = await (from o in _context.Conversacionportals.Include(x => x.Conversacionusuarios)
                                   where o.ConversacionId == entityConversacion.ConversacionId
                                   select new ConversacionModel()
                                   {
                                     ConversacionId = o.ConversacionId,
                                     Estatus = (int)o.EstatusConversacion,
                                     TipoConversacion = o.TipoConversacion,
                                     NombreConversacion = o.Titulo,
                                     ConversacionCerrada = o.EstatusConversacion == 0,
                                     OperadorId = o.Operador,
                                     EmpresaId = o.EmpresaId,
                                     RutaId = o.Ruta,
                                     UsuarioIdCreacion = o.UsuarioCreacion,
                                     Mensajes = (from z in o.Conversacionmensajes
                                                 join msg in _context.Mensajes on z.MensajeId equals msg.MensajeId
                                                 select new MensajeModel()
                                                 {
                                                   MensajeId = msg.MensajeId,
                                                   Comentario = msg.Comentario,
                                                   UsuarioId = msg.UsuarioId,
                                                   FechaCreacion = msg.FechaHoraCreacion.Value,
                                                 }).OrderBy(l => l.FechaCreacion).Distinct().ToList(),
                                     UsuarioCreacion = (from u in _context.Admins
                                                        where u.Idadmin == o.UsuarioCreacion
                                                        select u.Nombre).First(),
                                     FechaCreacion = o.FechaHoraCreacion,
                                     Usuarios = (from t in _context.Conductors.Include(n => n.FotoConductor)
                                                 join z in o.Conversacionusuarios on t.Idconductor equals z.UsuarioId
                                                 select new UsuariosConversacion()
                                                 {
                                                   UsuarioId = t.Idconductor,
                                                   NombreUsuario = t.Nombre ?? string.Empty,
                                                   ImagenPerfil = t.FotoConductor != null ? t.FotoConductor.Imagen : string.Empty,
                                                 }).ToList()
                                   }).FirstAsync();
          }
          else
          {
            var tipoConversacion = await this.ObtenerTipoConversacion(model.TipoConversacion, model.RutaId, model.OperadorId);
            response.Data = new ConversacionModel()
            {
              NombreConversacion = tipoConversacion.Data.Nombre,
              TipoConversacion = model.TipoConversacion,
              EmpresaId = model.EmpresaId,
              RutaId = model.RutaId,
              OperadorId = model.OperadorId,
              UsuarioIdCreacion = model.UsuarioIdCreacion.Value
            };
          }
        }
      }
      catch (Exception ex)
      {

      }


      return response;
    }
    private string ObtenerDiferenciaTiempo(DateTime fechaComparar)
    {
      var diferencia = DateTime.Now.Subtract(fechaComparar);
      if (diferencia.TotalSeconds < 60)
      {
        return string.Format("Hace {0} segundos", Math.Round(diferencia.TotalSeconds));
      }
      else if (diferencia.TotalMinutes < 60)
      {
        return string.Format("Hace {0} minutos", Math.Round(diferencia.TotalMinutes));
      }
      else if (diferencia.TotalHours < 23)
      {
        return string.Format("Hace {0} hrs", Math.Round(diferencia.TotalHours));
      }
      else
      {
        return fechaComparar.ToString("dd/MM/yyyy hh:mm");
      }

    }

  }
}

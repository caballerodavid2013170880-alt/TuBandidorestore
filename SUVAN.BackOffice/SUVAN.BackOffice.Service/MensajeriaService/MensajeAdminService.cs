using Microsoft.EntityFrameworkCore;
using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.Mensajeria;
using SUVAN.BackOffice.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Service.MensajeriaService
{
  public class MensajeAdminService : IMensajeAdminService
  {
    private readonly SuvanDbContext context;

    public MensajeAdminService(SuvanDbContext context)
    {
      this.context = context;
    }


    private async Task<List<ConversacionModel>> GetConversacion(int adminId)
    {
      var result = await (from o in context.Conversacionportals.Include(x => x.Conversacionusuarios)
                          where o.UsuarioCreacion == adminId && o.EstatusConversacion == 1
                          select new ConversacionModel()
                          {
                            ConversacionId = o.ConversacionId,
                            TipoConversacion = o.TipoConversacion,
                            EmpresaId = o.EmpresaId,
                            RutaId = o.Ruta,
                            OperadorId = o.Operador,
                            NombreConversacion = o.Titulo,
                            ConversacionCerrada = o.EstatusConversacion == 0,
                            UsuarioCreacion = (from u in context.Admins
                                               where u.Idadmin == adminId
                                               select u.Nombre).First(),
                            FechaCreacion = o.FechaHoraCreacion
                          }).OrderByDescending(x => x.FechaCreacion)
                          .ToListAsync();

      return result;
    }


    public async Task<List<MensajesAdminViewModel>> GetMessage(int adminId)
    {
      List<MensajesAdminViewModel> resultadoMensajes = new();
      var queryDate = DateTime.Today;

      var conversaciones = await GetConversacion(adminId);
      var conversacionIds = conversaciones.Select(x => x.ConversacionId).ToList();
      var mensajeslist = await (from o in context.Conversacionportals.Include(x => x.Conversacionusuarios)
                                where conversacionIds.Contains(o.ConversacionId)
                                select new ConversacionModel()
                                {
                                  ConversacionId = o.ConversacionId,
                                  TipoConversacion = o.TipoConversacion,
                                  NombreConversacion = o.Titulo,
                                  ConversacionCerrada = o.EstatusConversacion == 0,
                                  OperadorId = o.Operador,
                                  EmpresaId = o.EmpresaId,
                                  RutaId = o.Ruta,
                                  UsuarioIdCreacion = o.UsuarioCreacion,
                                  Mensajes = (from z in o.Conversacionmensajes
                                              join msg in context.Mensajes on z.MensajeId equals msg.MensajeId
                                              where msg.FechaHoraCreacion!.Value.Date == queryDate
                                              && msg.TipoUsuario == 0
                                              && z.Estatus == 0
                                              select new MensajeModel()
                                              {
                                                MensajeId = msg.MensajeId,
                                                Comentario = msg.Comentario!,
                                                UsuarioId = msg.UsuarioId,
                                                FechaCreacion = msg.FechaHoraCreacion!.Value,
                                              }).OrderByDescending(l => l.FechaCreacion).Distinct().ToList(),
                                  FechaCreacion = o.FechaHoraCreacion,

                                }).ToListAsync();

      // de mensajeslist obten solo el mensaje mas reciente por usuario de Mensaje


      resultadoMensajes = mensajeslist.SelectMany(x => x.Mensajes)
        .Select(x => new MensajesAdminViewModel()
        {
          MensajeId = x.MensajeId,
          FechaMensaje = x.FechaCreacion,
          Fecha = ObtenerDiferenciaTiempo(x.FechaCreacion),
          Comentario = x.Comentario,
          Usuario = (from u in context.Conductors
                     where u.Idconductor == x.UsuarioId
                     select u.Nombre).First()
        }).ToList();

      // obtene solo el menaje mas reciente de cada usuario de result
      var resultGroup = resultadoMensajes.GroupBy(x => x.Usuario)
        .Select(x => x.OrderByDescending(y => y.FechaMensaje).First())
        .ToList();

      return resultGroup;

    }

    public async Task<bool> MarcarMensajesLeidos(int conversacionId)
    {
      var mensajes = await context.Conversacionmensajes
        .Where(x => x.ConversacionId == conversacionId && x.Estatus == 0)
        .ToListAsync();

      if (mensajes.Any())
      {
        mensajes.ForEach(x => x.Estatus = 1);
        await context.SaveChangesAsync();
      }

      return true;

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

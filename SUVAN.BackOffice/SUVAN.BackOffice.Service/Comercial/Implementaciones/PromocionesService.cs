using Microsoft.EntityFrameworkCore;
using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.ViewModel.Comercial;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SUVAN.BackOffice.Service.Comercial
{
  public class PromocionesService : IPromocionesService
  {
    private readonly SuvanDbContext _context;
    public PromocionesService(SuvanDbContext context)
    {
      _context = context;
    }
    public async Task<List<PromocionesViewModel>> ConsultaPromocionesAsync(int empresaId)
    {
      List<PromocionesViewModel> model = await (from o in _context.Promocions
                                                .Include(x => x.TipodescuentoIdtipodescuentoNavigation)
                                                .Include(x => x.TipopromocionIdtipopromocionNavigation)
                                                where o.EmpresaIdempresa == empresaId
                                                select new PromocionesViewModel()
                                                {
                                                  PromocionId = o.Idpromocion,
                                                  Nombre = o.Nombre,
                                                  MontoDescuento = o.Cantidad,
                                                  DescripcionDescuento = o.TipodescuentoIdtipodescuento == 1 ? string.Format("{0:C}", o.Cantidad) : (o.Cantidad.Value / 100).ToString("P", CultureInfo.InvariantCulture),
                                                  AplicaPara = o.TipopromocionIdtipopromocionNavigation.Nombre,
                                                  FechaInicio = o.Vigenciadesde,
                                                  FechaFin = o.Vigenciahasta
                                                }).ToListAsync();
      return model;
    }
    public async Task<PromocionesViewModel> ConsultaPromocionAsync(int? promocionId)
    {
      PromocionesViewModel model = new();
      if (promocionId.HasValue)
      {
        model = await (from o in _context.Promocions
                       .Include(x => x.TipodescuentoIdtipodescuentoNavigation)
                       .Include(x => x.TipopromocionIdtipopromocionNavigation)
                       .Include(x => x.PromocionRuta)
                       .Include(x => x.PromocionHorarios)
                       where o.Idpromocion == promocionId
                       select new PromocionesViewModel()
                       {
                         PromocionId = o.Idpromocion,
                         TipoDescuentoId = o.TipodescuentoIdtipodescuento,
                         TipoPromocionId = o.TipopromocionIdtipopromocion,
                         RutasEmpresa = o.PromocionRuta.Select(x => x.RutaIdruta).ToArray(),
                         Nombre = o.Nombre,
                         MontoDescuento = o.Cantidad,
                         DescripcionDescuento = o.TipopromocionIdtipopromocion == 1 ? string.Format("{0:C}", o.Cantidad) : (o.Cantidad.Value / 100).ToString("P", CultureInfo.InvariantCulture),
                         AplicaPara = o.TipopromocionIdtipopromocionNavigation.Nombre,
                         FechaInicio = o.Vigenciadesde,
                         FechaFin = o.Vigenciahasta,
                         Horarios = (from z in o.PromocionHorarios
                                     select new HorarioViewModel()
                                     {
                                       HoraInicio = z.Horadesde,
                                       HoraFin = z.Horahasta

                                     }).ToArray(),
                         CorridasRutas = _context.PromocionCorrida.Where(x => x.PromocionIdpromocion == promocionId.Value).Select(x => x.CorridaIdcorrida).ToArray(),
                       }).FirstAsync();
      }

      if (promocionId.HasValue && model.TipoPromocionId == 4)
      {
        model.RutasEmpresa = (from y in _context.Corrida
                              join yy in _context.PromocionCorrida.Where(xx => xx.PromocionIdpromocion == promocionId) on y.Idcorrida equals yy.CorridaIdcorrida
                              select y.RutaIdruta).Distinct().ToArray();
      }

      return model;
    }

    public async Task<PromocionesViewModel> EliminaPromocionAsync(PromocionesViewModel model)
    {
      using (var transaction = _context.Database.BeginTransaction())
      {
        try
        {
          var ltPromocionRuta = _context.PromocionRuta.Where(x => x.PromocionIdpromocion == model.PromocionId);
          _context.PromocionRuta.RemoveRange(ltPromocionRuta);

          var ltPromocionCorrida = _context.PromocionCorrida.Where(x => x.PromocionIdpromocion == model.PromocionId);
          _context.PromocionCorrida.RemoveRange(ltPromocionCorrida);

          var ltPromocionHorario = _context.PromocionHorarios.Where(x => x.PromocionIdpromocion == model.PromocionId);
          _context.PromocionHorarios.RemoveRange(ltPromocionHorario);



          var entity = _context.Promocions.First(x => x.Idpromocion == model.PromocionId);
          _context.Promocions.Entry(entity).State = EntityState.Deleted;
          await _context.SaveChangesAsync();
          await transaction.CommitAsync();
        }
        catch (Exception)
        {
          await transaction.RollbackAsync();
        }
      }


      await _context.SaveChangesAsync();
      return model;
    }

    public async Task<PromocionesViewModel> GeneraPromocionAsync(PromocionesViewModel model)
    {
      using (var context = _context)
      {
        using (var transaction = context.Database.BeginTransaction())
        {
          try
          {
            var promocion = new Promocion()
            {
              Idpromocion = model.PromocionId,
              Nombre = model.Nombre,
              Vigenciadesde = model.FechaInicio,
              Vigenciahasta = model.FechaFin,
              TipopromocionIdtipopromocion = model.TipoPromocionId,
              Cantidad = model.MontoDescuento,
              TipodescuentoIdtipodescuento = model.TipoDescuentoId,
              EmpresaIdempresa = model.EmpresaId,

            };
            context.Promocions.Entry(promocion).State = model.PromocionId == 0 ? EntityState.Added : EntityState.Modified;
            await context.SaveChangesAsync();

            if (model.RutasEmpresa.Length > 0 && model.TipoPromocionId == 2)
            {
              foreach (var rutaId in model.RutasEmpresa)
              {
                context.PromocionRuta.Add(new PromocionRutum()
                {
                  PromocionIdpromocion = promocion.Idpromocion,
                  RutaIdruta = rutaId,
                  Activo = 1,
                  Fecharegistro = DateTime.Today

                });

              }
            }
            if (model.CorridasRutas.Length > 0 && model.TipoPromocionId == 4)
            {
              foreach (var corridaId in model.CorridasRutas)
              {
                context.PromocionCorrida.Add(new PromocionCorridum()
                {
                  PromocionIdpromocion = promocion.Idpromocion,
                  CorridaIdcorrida = corridaId
                });

              }
            }
            if (model.Horarios.Length > 0 && model.TipoPromocionId == 3)
            {
              foreach (var horario in model.Horarios)
              {
                context.PromocionHorarios.Add(new PromocionHorario()
                {
                  PromocionIdpromocion = promocion.Idpromocion,
                  Horadesde = horario.HoraInicio,
                  Horahasta = horario.HoraFin
                });

              }

            }

            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            model.PromocionId = promocion.Idpromocion;
          }
          catch (Exception)
          {
            await transaction.RollbackAsync();
          }

        }

      }
      return model;
    }
    public async Task<List<TipoDescuentoViewModel>> TipoDescuentoAsync()
    {
      var result = await (from o in _context.Tipodescuentos
                          select new TipoDescuentoViewModel()
                          {
                            TipoDescuendoId = o.Idtipodescuento,
                            NombreDescuento = o.Nombre
                          }).ToListAsync();
      return result;
    }
    public async Task<List<TipoPromocionViewModel>> TipoPromocionAsync()
    {
      var result = await (from o in _context.Tipopromocions
                          select new TipoPromocionViewModel()
                          {
                            TipoPromocionId = o.Idtipopromocion,
                            NombrePromocion = o.Nombre

                          }).ToListAsync();
      return result;
    }

    public async Task<UsuarioEmpresaModel> ConsultaEmpresaUsuarioAsync(int userId)
    {
      UsuarioEmpresaModel model = await (from o in _context.AdminEmpresas.Include(x => x.EmpresaIdempresaNavigation)
                                         where o.AdminIdadmin == userId && o.Principal == 1
                                         select new UsuarioEmpresaModel()
                                         {
                                           EmpresaId = o.EmpresaIdempresaNavigation.Idempresa,
                                           NombreEmpresa = o.EmpresaIdempresaNavigation.Nombre
                                         }).FirstAsync();
      return model;
    }

    public async Task<List<RutaEmpresaViewModel>> ConsultaRutaCorridaAsync(int empresaId)
    {
      List<RutaEmpresaViewModel> result = await (from o in _context.Ruta.Include(x => x.Corrida)
                                                 where o.EmpresaIdempresa == empresaId
                                                 select new RutaEmpresaViewModel()
                                                 {
                                                   RutaId = o.Idruta,
                                                   NombreRuta = o.Nombre,
                                                   Corridas = o.Corrida.Select(x => new CorridaViewModel()
                                                   {
                                                     CorridaId = x.Idcorrida,
                                                     HoraInicio = x.HoraInicio,
                                                     HoraFin = x.HoraFin
                                                   }).ToList()


                                                 }).ToListAsync();
      return result;
    }
  }
}

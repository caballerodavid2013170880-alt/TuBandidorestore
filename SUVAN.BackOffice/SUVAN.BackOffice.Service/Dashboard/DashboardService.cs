using Microsoft.EntityFrameworkCore;
using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.ViewModel.Configuracion;
using SUVAN.BackOffice.Models.ViewModel.Dashboard;
using SUVAN.BackOffice.Utilities.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Service.Dashboard
{
  public class DashboardService : IDashboardService

  {
    private readonly SuvanDbContext context;

    public DashboardService(SuvanDbContext context)
    {
      this.context = context;
    }


    public async Task<List<RutasViewModel>> GetRutasHorarios(int empresaId)
    {
      var rutas2 = await (from r in context.Ruta
                          where r.EmpresaIdempresa == empresaId
                          select new RutasViewModel
                          {
                            RutaId = r.Idruta,
                            Nombre = r.Nombre!,
                            Corridas = r.Corrida.Select(y => new CorridasRutaViewModel
                            {
                              CorridaId = y.Idcorrida,
                              Horas = $"{y.HoraInicio:hh\\:mm} - {y.HoraFin:hh\\:mm}",

                            }).ToList()
                          })
                      .ToListAsync();

      var Rutas = rutas2.GroupBy(x => new { x.RutaId, x.Nombre })
         .Select(x => new RutasViewModel
         {
           RutaId = x.Key.RutaId,
           Nombre = x.Key.Nombre,
           Corridas = x.SelectMany(y => y.Corridas).Distinct().ToList()
         }).ToList();

      return Rutas;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="model"></param>
    /// <param name="empresaId"></param>
    /// <returns></returns>
    public async Task<ViajeIngresosOcupacionDashboard> GetViajeOcupacionIngreso(ViajeIngresosOcupacionFiltro model, int empresaId)
    {

      ViajeIngresosOcupacionDashboard result = new();

      DateTime fechaInicio = Convert.ToDateTime(model.Fecha.Split("-")[0].ConvertirFechaServer());
      DateTime fechaFin = Convert.ToDateTime(model.Fecha.Split("-")[1].ConvertirFechaServer());

      var query = await (from v in context.Viajes
                         where v.EmpresaIdempresa == empresaId
                         &&
                         v.Fechaviaje >= fechaInicio && v.Fechaviaje <= fechaFin.AddDays(1).AddSeconds(-1)
                         && (model.RutaId == string.Empty || v.CorridaAsignacionIdcorridaAsignacionNavigation!.CorridaIdcorridaNavigation.RutaIdruta == Convert.ToInt32(model.RutaId))
                         && (model.HorarioId == string.Empty || v.CorridaAsignacionIdcorridaAsignacionNavigation!.CorridaIdcorridaNavigation.Idcorrida == Convert.ToInt32(model.HorarioId))
                         group v by new
                         {
                           Año = model.Periodo == "a" ? v.Fechaviaje!.Value.Year : 0,
                           Mes = model.Periodo == "m" ? v.Fechaviaje!.Value.Month : 0,
                           Día = model.Periodo == "d" ? v.Fechaviaje!.Value.Day : 0
                         } into g
                         select new ViajeIngresosOcupacion
                         {
                           Periodo = model.Periodo == "a" ? g.Key.Año.ToString() :
                                       model.Periodo == "m" ? $"{g.Key.Año}-{g.Key.Mes}" :
                                       $"{g.Key.Día}",
                           CantidadUsuarios = (model.Indicador == "o" ? g.Select(v => v.UsuarioIdusuario).Count() :
                           model.Indicador == "i" ? g.Sum(v => v.CostoFinalTotal ?? 0)
                           : 0)
                         }).ToListAsync();



      var tableQuery = await (from v in context.Viajes
                              where v.EmpresaIdempresa == empresaId
                                && v.Fechaviaje >= fechaInicio && v.Fechaviaje <= fechaFin.AddDays(1).AddSeconds(-1)
                                 && (model.RutaId == string.Empty || v.CorridaAsignacionIdcorridaAsignacionNavigation!.CorridaIdcorridaNavigation.RutaIdruta == Convert.ToInt32(model.RutaId))
                                 && (model.HorarioId == string.Empty || v.CorridaAsignacionIdcorridaAsignacionNavigation!.CorridaIdcorridaNavigation.Idcorrida == Convert.ToInt32(model.HorarioId))

                              //group v by new
                              //{
                              //  Año = model.Periodo == "a" ? v.Fechaviaje!.Value.Year : 0,
                              //  Mes = model.Periodo == "m" ? v.Fechaviaje!.Value.Month : 0,
                              //  Día = model.Periodo == "d" ? v.Fechaviaje!.Value.Day : 0
                              //} into g
                              select new ViajeIngresosOcupacionTable
                              {
                                FechaViaje = Convert.ToDateTime(v.Fechaviaje).ToShortDateString(),
                                Boleto = v.Boleto!,
                                Costo = v.CostoFinalTotal
                              }).ToListAsync();

      // agrupar por fecha sumando la cantidad de boletos y la sumatoria de los costos
      var tableGroup = tableQuery.GroupBy(x => x.FechaViaje)
        .Select(x => new ViajeIngresosOcupacionTable
        {
          FechaViaje = x.Key,
          CantidadBoleto = x.Count(),
          SumatoriaCostoFinal = x.Sum(v => v.Costo ?? 0)
        }).ToList();

      result.Chart = query;
      result.Table = tableGroup;

      return result;
    }

  }
}

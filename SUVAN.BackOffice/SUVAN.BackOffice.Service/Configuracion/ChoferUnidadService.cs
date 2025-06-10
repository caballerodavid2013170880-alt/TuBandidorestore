using FacturacionPegaso;
using Microsoft.EntityFrameworkCore;
using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.ViewModel.Configuracion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Service.Configuracion
{
  public class ChoferUnidadService : IChoferUnidadService
  {
    private readonly SuvanDbContext context;

    public ChoferUnidadService(SuvanDbContext context)
    {
      this.context = context;
    }

    /// <summary>
    /// obtener el listado de rutas
    /// </summary>
    /// <param name="empresaId"></param>
    /// <returns></returns>
    public async Task<ChoferUnidadViewModel> GetChoferUnidadViewModel(int empresaId)
    {
      ChoferUnidadViewModel choferUnidadViewModel = await GetChoferUnidad(empresaId);

      return choferUnidadViewModel;
    }

    private async Task<ChoferUnidadViewModel> GetChoferUnidad(int empresaId)
    {
      // convierte la consulta de rutas a linq 

      var rutas2 = await (from r in context.Ruta
                          where r.EmpresaIdempresa == empresaId
                          select new RutasViewModel
                          {
                            RutaId = r.Idruta,
                            Nombre = r.Nombre!,
                            Corridas = r.Corrida.Select(y => new CorridasRutaViewModel
                            {
                              CorridaId = y.Idcorrida,
                              Horas = $"{y.HoraInicio:HH\\:mm} - {y.HoraFin:HH\\:mm}",
                              DiasInactivos = ConvertDiasToIndex(y.CorridaDia.Where(x => x.Activo == 0).Select(x => x.DiasIddias).ToList()),
                              FechasSeleccionadas = y.CorridaAsignacions.Select(x => Convert.ToDateTime(x.Fecha).ToString("yyyy-MM-dd")).Distinct().ToList()
                            }).ToList()
                          })
                      .ToListAsync();

      var conductores = await context.Conductors
        .Where(x => x.EmpresaIdempresa == empresaId && x.Activo == 1)
        .OrderBy(x => x.Nombre)
        .ToListAsync();

      var unidades = await context.Vehiculos
        .Include(x => x.TipovehiculoIdtipovehiculoNavigation)
        .Where(x => x.EmpresaIdempresa == empresaId && x.Activo == 1)
        .ToListAsync();

      var choferUnidadViewModel = new ChoferUnidadViewModel
      {
        Rutas = rutas2.GroupBy(x => new { x.RutaId, x.Nombre })
          .Select(x => new RutasViewModel
          {
            RutaId = x.Key.RutaId,
            Nombre = x.Key.Nombre,
            Corridas = x.SelectMany(y => y.Corridas).Distinct().ToList()
          }).ToList(),
        Conductores = conductores.Select(x => new ConductorViewModel
        {
          ChoferId = x.Idconductor,
          Nombre = x.Nombre!
        }).ToList(),
        Vehiculos = unidades.Select(x => new VehiculoVewModel
        {
          UnidadId = x.IdVehiculo,
          Descripcion = $"{x.TipovehiculoIdtipovehiculoNavigation.Nombre} - {x.Placas}",
        }).ToList()
      };

      choferUnidadViewModel.RutasJson = Newtonsoft.Json.JsonConvert.SerializeObject(choferUnidadViewModel.Rutas);
      return choferUnidadViewModel;
    }

    /// <summary>
    /// Consulta los choferes asignados a una unidad y una corrida
    /// </summary>
    /// <param name="model"></param>
    /// <param name="empresaId"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<ChoferUnidadAgregarViewModel> ConsultarChoferesUnidad(ChoferUnidadAgregarViewModel model, int empresaId)
    {
      ChoferUnidadAgregarViewModel result = new();

      List<ChoferUnidad> choferUnidad = new();

      var corridaAsignacion = await context.CorridaAsignacions
        .Include(x => x.ConductorIdconductorNavigation)
        .Include(x => x.VehiculoIdvehiculoNavigation)
        .ThenInclude(x => x.TipovehiculoIdtipovehiculoNavigation)
        .Where(x => x.CorridaIdcorrida == model.CorridaId && model.Fechas.Contains((DateTime)x.Fecha!))
        .ToListAsync();

      if (corridaAsignacion.Any())
      {
        choferUnidad = corridaAsignacion.Select(x => new ChoferUnidad
        {
          conductorId = x.ConductorIdconductor,
          conductor = x.ConductorIdconductorNavigation.Nombre!,
          vehiculoId = x.VehiculoIdvehiculo,
          vehiculo = $"{x.VehiculoIdvehiculoNavigation.TipovehiculoIdtipovehiculoNavigation.Nombre} - {x.VehiculoIdvehiculoNavigation.Placas}",
          Consulta = true
        }).ToList();

        // remover conductores que esten repetidos 
        choferUnidad = choferUnidad.GroupBy(x => x.conductorId)
          .Select(x => x.First())
          .ToList();
      }

      result.RutaId = model.RutaId;
      result.CorridaId = model.CorridaId;
      result.Conductores = choferUnidad;

      return result;
    }


    /// <summary>
    /// Agrega un chofer a una unidad y una corrida
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<bool> AgregarChoferUnidad(ChoferUnidadAgregarViewModel model)
    {
      if (model is null)
      {
        throw new Exception("Informacion invalida");
      }

      if (model.RutaId == 0 || model.CorridaId == 0)
      {
        throw new Exception("Informacion invalida");
      }

      var corrida = await context.Corrida
        .FirstOrDefaultAsync(x => x.Idcorrida == model.CorridaId);

      if (corrida is null)
      {
        throw new Exception("No se encontro la corrida");
      }

      // remove conductores de Consulta true de model
      model.Conductores = model.Conductores.Where(x => !x.Consulta).ToList();

      await ValidacionesDeAsignacion(model, corrida);

      if (model.Fechas.Any())
      {
        foreach (var conductor in model.Conductores)
        {
          //await EliminaCorridaAsignacion(model.CorridaId, conductor.conductorId);


          foreach (var fechaAsignacion in model.Fechas)
          {

            var existeConductor = await context.CorridaAsignacions
              .Where(x => x.CorridaIdcorrida == model.CorridaId
              && (x.ConductorIdconductor == conductor.conductorId || x.VehiculoIdvehiculo == conductor.vehiculoId)
              && x.Fecha == fechaAsignacion)
              .FirstOrDefaultAsync();

            if (existeConductor is not null)
            {
              existeConductor.VehiculoIdvehiculo = conductor.vehiculoId;
              existeConductor.ConductorIdconductor = conductor.conductorId;
              existeConductor.Fecha = fechaAsignacion;
            }
            else
            {
              context.CorridaAsignacions.Add(new CorridaAsignacion
              {
                CorridaIdcorrida = model.CorridaId,
                ConductorIdconductor = conductor.conductorId,
                VehiculoIdvehiculo = conductor.vehiculoId,
                Fecha = fechaAsignacion,
                EstatusviajeIdestatusviaje = 1
              });
            }


          }


        }

        await context.SaveChangesAsync();



        await AgrearCorridaASignacionParada(model);

      }

      return true;
    }


    /// <summary>
    /// asigna las paradas de una corrida a una asignacion de corrida
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    private async Task AgrearCorridaASignacionParada(ChoferUnidadAgregarViewModel model)
    {
      var rutaparada = await context.RutaParada
        .Where(x => x.RutaIdruta == model.RutaId)
        .ToListAsync();

      var corridaAsignacion = await context.CorridaAsignacions
        .Include(x => x.VehiculoIdvehiculoNavigation)
        .ThenInclude(x => x.TipovehiculoIdtipovehiculoNavigation)
        .Where(x => x.CorridaIdcorrida == model.CorridaId)
        .ToListAsync();

      foreach (var item in corridaAsignacion)
      {

        foreach (var parada in rutaparada)
        {
          var existeParada = await context.CorridaAsignacionParada
            .Where(x => x.CorridaAsignacionIdcorridaAsignacion == item.IdcorridaAsignacion
                       && x.ParadaIdparada == parada.ParadaIdparada)
            .FirstOrDefaultAsync();

          if (existeParada is null)
          {
            context.CorridaAsignacionParada.Add(new CorridaAsignacionParadum
            {
              CorridaAsignacionIdcorridaAsignacion = item.IdcorridaAsignacion,
              ParadaIdparada = parada.ParadaIdparada,
              Espacios = item.VehiculoIdvehiculoNavigation.TipovehiculoIdtipovehiculoNavigation.Asientos,
              EstatusestacionIdestatusestacion = 0,
              Suben = 0,
              Bajan = 0,
              Subieron = 0,
              Bajaron = 0
            });
          }
          else
          {
            existeParada.Espacios = item.VehiculoIdvehiculoNavigation.TipovehiculoIdtipovehiculoNavigation.Asientos;
          }
        }

      }

      await context.SaveChangesAsync();

    }

    /// <summary>
    /// Valida que la asignacion de un chofer a una unidad y una corrida sea valida
    /// </summary>
    /// <param name="choferUnidadAgregarViewModel"></param>
    /// <param name="corrida"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    private async Task ValidacionesDeAsignacion(ChoferUnidadAgregarViewModel choferUnidadAgregarViewModel, Corridum? corrida)
    {
      // validar que la unidad dentro del listado de ChoferUnidad no este asignada a otra corrida en el mismo horario en todas las fechas seleccionadas tomando encuenta el listado de conductores en ChoferUnidad
      var unidadesAsignadas = await context.CorridaAsignacions
        .Include(x => x.CorridaIdcorridaNavigation)
        .Include(x => x.VehiculoIdvehiculoNavigation)
        .Where(x => (choferUnidadAgregarViewModel.Conductores.Select(y => y.vehiculoId).Contains(x.VehiculoIdvehiculo)
                        || choferUnidadAgregarViewModel.Conductores.Select(y => y.conductorId).Contains(x.ConductorIdconductor))
                                        && choferUnidadAgregarViewModel.Fechas.Contains((DateTime)x.Fecha!)
                                                        && x.CorridaIdcorridaNavigation.HoraInicio == corrida!.HoraInicio
                                                                        && x.CorridaIdcorridaNavigation.HoraFin == corrida.HoraFin
                                                                                        && x.CorridaIdcorridaNavigation.Idcorrida != corrida.Idcorrida)
        .ToListAsync();



      if (unidadesAsignadas.Any())
      {
        ConstruyeValidacionException(unidadesAsignadas);
      }

      // validar que el chofer en el listado de ChoferUnidad no este asignado a otra corrida en el mismo horario
      var choferesAsignados = await context.CorridaAsignacions
        .Include(x => x.CorridaIdcorridaNavigation)
        .Include(x => x.ConductorIdconductorNavigation)
        .Where(x => choferUnidadAgregarViewModel.Conductores.Select(y => y.conductorId).Contains(x.ConductorIdconductor)
                               && choferUnidadAgregarViewModel.Fechas.Contains((DateTime)x.Fecha!)
                                                                      && x.CorridaIdcorridaNavigation.HoraInicio == corrida!.HoraInicio
                                                                                                                             && x.CorridaIdcorridaNavigation.HoraFin == corrida.HoraFin
                                                                                                                                                                                                    && x.CorridaIdcorridaNavigation.Idcorrida != corrida.Idcorrida)
        .ToListAsync();

      if (choferesAsignados.Any())
      {
        var mensaje = new System.Text.StringBuilder();
        foreach (var chofer in choferesAsignados)
        {
          mensaje.AppendLine($"El operador {chofer.ConductorIdconductorNavigation.Nombre} ya esta asignado a otra ruta en la fecha {chofer.Fecha} en el mismo horario");
        }

        throw new Exception(mensaje.ToString());


      }

      // validar que la unidad y chofer no este asignado en el rango de horario inicio fin en la misma fecha
      var asignacionHorario = await context.CorridaAsignacions
        .Include(x => x.CorridaIdcorridaNavigation)
        .Include(x => x.VehiculoIdvehiculoNavigation)
        .Include(x => x.ConductorIdconductorNavigation)
        .Where(x => (choferUnidadAgregarViewModel.Conductores.Select(y => y.vehiculoId).Contains(x.VehiculoIdvehiculo)
                                      || choferUnidadAgregarViewModel.Conductores.Select(y => y.conductorId).Contains(x.ConductorIdconductor))
                                                                    && choferUnidadAgregarViewModel.Fechas.Contains((DateTime)x.Fecha!)
                                                                                                  && (x.CorridaIdcorridaNavigation.HoraInicio <= corrida!.HoraInicio
                                                                                                                                && x.CorridaIdcorridaNavigation.HoraFin >= corrida.HoraInicio))
        .ToListAsync();

      if (asignacionHorario.Any())
      {
        var mensaje = new System.Text.StringBuilder();
        foreach (var asignacion in asignacionHorario)
        {
          mensaje.AppendLine($"El vehiculo {asignacion.VehiculoIdvehiculoNavigation.Placas} y el operador {asignacion.ConductorIdconductorNavigation.Nombre} ya esta asignado a otra ruta en el rango de horario {asignacion.CorridaIdcorridaNavigation.HoraInicio:hh\\:mm} - {asignacion.CorridaIdcorridaNavigation.HoraFin:hh\\:mm} en la fecha {Convert.ToDateTime(asignacion.Fecha).ToShortDateString()}");
        }

        throw new Exception(mensaje.ToString());
      }


      // validar que la unidad y chofer no este asignado en el rango de horario inicio fin en la misma fecha
      //var asignacionFecha = await context.CorridaAsignacions
      //  .Include(x => x.CorridaIdcorridaNavigation)
      //  .Include(x => x.VehiculoIdvehiculoNavigation)
      //  .Include(x => x.ConductorIdconductorNavigation)
      //  .Where(x => (choferUnidadAgregarViewModel.Conductores.Select(y => y.vehiculoId).Contains(x.VehiculoIdvehiculo)
      //                                || choferUnidadAgregarViewModel.Conductores.Select(y => y.conductorId).Contains(x.ConductorIdconductor))
      //                                                              && choferUnidadAgregarViewModel.Fechas.Contains((DateTime)x.Fecha!))
      //  .ToListAsync();

      //if (asignacionFecha.Any())
      //{
      //  var mensaje = new System.Text.StringBuilder();
      //  foreach (var asignacion in asignacionFecha)
      //  {
      //    mensaje.AppendLine($"El vehiculo {asignacion.VehiculoIdvehiculoNavigation.Placas} y el operador {asignacion.ConductorIdconductorNavigation.Nombre} ya esta asignado en la fecha {Convert.ToDateTime(asignacion.Fecha).ToShortDateString()}");
      //  }

      //  throw new Exception(mensaje.ToString());
      //}

    }

    private static void ConstruyeValidacionException(List<CorridaAsignacion> unidadesAsignadas)
    {
      var mensaje = new System.Text.StringBuilder();
      foreach (var unidad in unidadesAsignadas)
      {
        mensaje.AppendLine($"El vehiculo {unidad.VehiculoIdvehiculoNavigation.Placas} ya esta asignado a otro operador en la fecha {unidad.Fecha}");
      }

      throw new Exception(mensaje.ToString());
    }

    /// <summary>
    /// elimina la asignacion de un chofer a una unidad y una corrida que no este asignado ya a un viaje
    /// </summary>
    /// <param name="idCorrida"></param>
    /// <returns></returns>
    private async Task EliminaCorridaAsignacion(int idCorrida, int idConductor)
    {
      var corridaAsignacion = await context.CorridaAsignacions
        .Where(x => x.CorridaIdcorrida == idCorrida && x.ConductorIdconductor == idConductor)
        .ToListAsync();

      if (corridaAsignacion.Any())
      {
        List<Viaje> ListViajes = new();

        foreach (var corrida in corridaAsignacion)
        {
          var viajesResult = await context.Viajes
                          .Where(x => x.CorridaAsignacionIdcorridaAsignacion == corrida.IdcorridaAsignacion)
                                       .ToListAsync();
          if (viajesResult.Any())
            ListViajes.AddRange(viajesResult);

        }


        // elimina corida asignacion por idCorrida y idConductor y que no exista en ListViajes
        var corridaAsignacionEliminar = await context.CorridaAsignacions
          .Where(x => x.CorridaIdcorrida == idCorrida && x.ConductorIdconductor == idConductor
            && !ListViajes.Select(x => x.CorridaAsignacionIdcorridaAsignacion).Contains(x.IdcorridaAsignacion))
          .ExecuteDeleteAsync();

      }

    }

    /// <summary>
    /// Convierte los dias de la corrida a un listado de indices
    /// </summary>
    /// <param name="dias"></param>
    /// <returns></returns>
    private static List<int> ConvertDiasToIndex(List<string> dias)
    {
      List<int> result = new();
      foreach (string diasId in dias)
      {
        switch (diasId)
        {
          case "Lu":
            result.Add(1);
            break;
          case "Ma":
            result.Add(2);
            break;
          case "Mi":
            result.Add(3);
            break;
          case "Ju":
            result.Add(4);
            break;
          case "Vi":
            result.Add(5);
            break;
          case "Sa":
            result.Add(6);
            break;
          case "Do":
            result.Add(0);
            break;
        }
      }

      return result;

    }

    /// <summary>
    /// Obtiene el reporte de choferes asignados a una unidad y una corrida
    /// </summary>
    /// <param name="model"></param>
    /// <param name="empresaId"></param>
    /// <returns></returns>
    public async Task<ChoferUnidadViewModel> GetReporte(ChoferUnidadViewModel model, int empresaId)
    {
      ChoferUnidadViewModel result = await GetChoferUnidad(empresaId);
      result.RutaId = model.RutaId;
      result.IdHoSeleccionado = model.HorarioId;
      // obtiene listado de corridaasignacion por RutaId y HorarioId
      var corridaAsignacion = await context.CorridaAsignacions
        .Include(x => x.ConductorIdconductorNavigation)
        .Include(x => x.VehiculoIdvehiculoNavigation)
        .ThenInclude(x => x.TipovehiculoIdtipovehiculoNavigation)
        .Include(x => x.CorridaIdcorridaNavigation)
        .Where(x => x.CorridaIdcorridaNavigation.RutaIdruta == model.RutaId
        && x.CorridaIdcorridaNavigation.EmpresaIdempresa == empresaId
                           && (model.HorarioId == null || x.CorridaIdcorridaNavigation.Idcorrida == model.HorarioId))
        .ToListAsync();

      var corridaAsignacionList = corridaAsignacion.Select(x => new CorridaAsignacionViewModel
      {
        Conductor = x.ConductorIdconductorNavigation.Nombre!,
        Vehiculo = $"{x.VehiculoIdvehiculoNavigation.TipovehiculoIdtipovehiculoNavigation.Nombre} - {x.VehiculoIdvehiculoNavigation.Placas}",
        Horario = $"{x.CorridaIdcorridaNavigation.HoraInicio:HH\\:mm} - {x.CorridaIdcorridaNavigation.HoraFin:HH\\:mm}",
        Fechas = corridaAsignacion.Where(y => y.ConductorIdconductor == x.ConductorIdconductor && y.VehiculoIdvehiculo == x.VehiculoIdvehiculo).Select(y => (DateTime)y.Fecha!).ToList()
      }).ToList();

      // remover elementos repetidos en corridaAsignacionList
      corridaAsignacionList = corridaAsignacionList.GroupBy(x => new { x.Conductor, x.Vehiculo, x.Horario })
        .Select(x => new CorridaAsignacionViewModel
        {
          Conductor = x.Key.Conductor,
          Vehiculo = x.Key.Vehiculo,
          Horario = x.Key.Horario,
          Fechas = x.SelectMany(y => y.Fechas).Distinct().OrderByDescending(x => x).ToList()
        }).ToList();

      result.CorridaAsignacion = corridaAsignacionList;

      return result;
    }

    /// <summary>
    /// obtiene el listado de conductores y unidades para reasignar
    /// </summary>
    /// <param name="empresaId"></param>
    /// <returns></returns>
    public async Task<ReasignarChoferViewMolde> GetReasignarChoferUnidad(int empresaId)
    {
      var reasignar = new ReasignarChoferViewMolde();

      reasignar.Conductores = await GetConductoresActivo(empresaId);
      reasignar.Vehiculos = await GetVehiculosActivo(empresaId);

      return reasignar;

    }


    /// <summary>
    /// obtiene las de choferes o unidades
    /// </summary>
    /// <param name="model"></param>
    /// <param name="empresaId"></param>
    /// <returns></returns>
    public async Task<ReasignarChoferViewMolde> BuscarAsignaciones(ConsultaReasingacionViewModel model, int empresaId)
    {
      var result = new ReasignarChoferViewMolde();
      result.Vehiculos = await GetVehiculosActivo(empresaId);
      result.Conductores = await GetConductoresActivo(empresaId);
      result.ConductorId = model.ConductorId;
      result.VehiculoId = model.VehiculoId;

      result.CorridaReasingacion = await context.CorridaAsignacions
        .Include(x => x.ConductorIdconductorNavigation)
        .Include(x => x.VehiculoIdvehiculoNavigation)
        .Include(x => x.CorridaIdcorridaNavigation)
        .Where(x => (model.ConductorId == 0 || x.ConductorIdconductor == model.ConductorId)
                    && (model.VehiculoId == 0 || x.VehiculoIdvehiculo == model.VehiculoId)
                    && x.CorridaIdcorridaNavigation.EmpresaIdempresa == empresaId
                    && x.Fecha >= DateTime.Today.AddDays(-1))
        .OrderBy(x => x.Fecha)
        .ThenBy(x => x.CorridaIdcorridaNavigation.HoraInicio)
        .Select(x => new CorridaReasingacionViewModel
        {
          CorridaAsignacionId = x.IdcorridaAsignacion,
          RutaId = x.CorridaIdcorridaNavigation.RutaIdruta,
          NombreRuta = x.CorridaIdcorridaNavigation.RutaIdrutaNavigation.Nombre!,
          Conductor = x.ConductorIdconductorNavigation.Nombre!,
          ConductorId = x.ConductorIdconductor,
          Vehiculo = $"{x.VehiculoIdvehiculoNavigation.TipovehiculoIdtipovehiculoNavigation.Nombre} - {x.VehiculoIdvehiculoNavigation.Placas}",
          VehiculoId = x.VehiculoIdvehiculo,
          HorarioId = x.CorridaIdcorrida,
          Horario = $"{x.CorridaIdcorridaNavigation.HoraInicio:HH\\:mm} - {x.CorridaIdcorridaNavigation.HoraFin:HH\\:mm}",
          Fecha = (DateTime)x.Fecha!
        })
        .OrderByDescending(x => x.Fecha)
        .ToListAsync();

      return result;

    }

    /// <summary>
    /// acutaliza la reasingacion del chofer o unidad
    /// </summary>
    /// <param name="model"></param>
    /// <param name="empresaId"></param>
    /// <returns></returns>
    public async Task GuardarReasignacion(ReasignarChoferViewModel model, int empresaId)
    {



      foreach (var item in model.detalle)
      {
        var corridaAsignacion = await context.CorridaAsignacions
         .Include(x => x.CorridaIdcorridaNavigation)
         .Where(x => x.IdcorridaAsignacion == item.corridaAsingnacionId)
         .FirstOrDefaultAsync();



        var asignacionHorario = await context.CorridaAsignacions
          .Include(x => x.CorridaIdcorridaNavigation)
          .Include(x => x.VehiculoIdvehiculoNavigation)
          .Include(x => x.ConductorIdconductorNavigation)
          .Where(x => (((model.vehiculoReasignarId == 0 || model.vehiculoReasignarId == x.VehiculoIdvehiculo)
                                        && (model.conductorReasignarId == 0 || model.conductorReasignarId == x.ConductorIdconductor))
                                                                      && corridaAsignacion!.Fecha == x.Fecha
                                                                      && (x.CorridaIdcorridaNavigation.HoraInicio <= corridaAsignacion!.CorridaIdcorridaNavigation.HoraInicio
                                                                                                                                  && x.CorridaIdcorridaNavigation.HoraFin >= corridaAsignacion.CorridaIdcorridaNavigation.HoraInicio))
                                                                                                                                  && x.IdcorridaAsignacion != item.corridaAsingnacionId)
          .ToListAsync();

        if (asignacionHorario.Any())
        {
          var mensaje = new System.Text.StringBuilder();
          foreach (var asignacion in asignacionHorario)
          {
            mensaje.AppendLine($"El vehiculo {asignacion.VehiculoIdvehiculoNavigation.Placas} y el operador {asignacion.ConductorIdconductorNavigation.Nombre} ya esta asignado en el rango de horario {asignacion.CorridaIdcorridaNavigation.HoraInicio:hh\\:mm} - {asignacion.CorridaIdcorridaNavigation.HoraFin:hh\\:mm} en la fecha {Convert.ToDateTime(asignacion.Fecha).ToShortDateString()}");
          }

          throw new Exception(mensaje.ToString());
        }



        if (corridaAsignacion is not null)
        {
          if (model.conductorReasignarId != 0)
          {
            corridaAsignacion.ConductorIdconductor = model.conductorReasignarId;

          }
          if (model.vehiculoReasignarId != 0)
          {
            corridaAsignacion.VehiculoIdvehiculo = model.vehiculoReasignarId;

          }
          await context.SaveChangesAsync();
        }
      }
    }


    public async Task EliminarAsignacionChoferUnidad(ReasignarChoferViewModel model, int empresaId)
    {
      //validar si la corrida asignacion tiene viajes asignados
      await validacionSiExistenViajes(model);

      foreach (var item in model.detalle)
      {
        var corridaAsignacion = await context.CorridaAsignacions
         .Where(x => x.IdcorridaAsignacion == item.corridaAsingnacionId)
         .FirstOrDefaultAsync();

        if (corridaAsignacion is not null)
        {
          context.CorridaAsignacions.Remove(corridaAsignacion);
          await context.SaveChangesAsync();
        }
      }
    }

    /// <summary>
    /// valida si existen viajes en las corridas a eliminar
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    private async Task validacionSiExistenViajes(ReasignarChoferViewModel model)
    {
      var mensaje = new System.Text.StringBuilder();
      foreach (var item in model.detalle)
      {
        var corridaAsignacion = await context.CorridaAsignacions
         .Where(x => x.IdcorridaAsignacion == item.corridaAsingnacionId)
         .FirstOrDefaultAsync();

        if (corridaAsignacion is not null)
        {
          var viajes = await context.Viajes
            .Where(x => x.CorridaAsignacionIdcorridaAsignacion == corridaAsignacion.IdcorridaAsignacion)
            .ToListAsync();

          if (viajes.Any())
          {
            mensaje.AppendLine($"No se puede eliminar la asignacion ya que tiene viaje asignado Fecha: ${Convert.ToDateTime(corridaAsignacion.Fecha).ToShortDateString()}");
          }
        }
      }

      if (mensaje.ToString().Length > 0)
      {
        throw new Exception(mensaje.ToString());
      }
    }

    private async Task<List<VehiculoVewModel>> GetVehiculosActivo(int empresaId)
    {
      List<VehiculoVewModel> vehiculos = new();
      vehiculos = await context.Vehiculos
        .Include(x => x.TipovehiculoIdtipovehiculoNavigation)
        .Where(x => x.EmpresaIdempresa == empresaId && x.Activo == 1)
        .OrderBy(x => x.TipovehiculoIdtipovehiculoNavigation.Nombre)
        .Select(x => new VehiculoVewModel
        {
          UnidadId = x.IdVehiculo,
          Descripcion = $"{x.TipovehiculoIdtipovehiculoNavigation.Nombre} - {x.Placas}",
        })
        .ToListAsync();
      return vehiculos;
    }

    private async Task<List<ConductorViewModel>> GetConductoresActivo(int empresaId)
    {
      List<ConductorViewModel> conductores = new();
      conductores = await context.Conductors
       .Where(x => x.EmpresaIdempresa == empresaId && x.Activo == 1)
       .OrderBy(x => x.Nombre)
       .Select(x => new ConductorViewModel
       {
         ChoferId = x.Idconductor,
         Nombre = x.Nombre!
       })
       .ToListAsync();
      return conductores;
    }



  }
}

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
  public class CorridasService : ICorridasService
  {
    private readonly SuvanDbContext context;

    public CorridasService(SuvanDbContext context)
    {
      this.context = context;
    }

    /// <summary>
    /// obtener el listado de corridas de la base de datos por empresa
    /// </summary>
    /// <param name="empresaId"></param>
    /// <returns></returns>
    public async Task<List<Rutum>> GetCorridas(int empresaId)
    {
      var corridas = await context.Ruta
        .Include(x => x.Corrida)
        .ThenInclude(x => x.CorridaDia)
        .ThenInclude(x => x.DiasIddiasNavigation)
        .Where(x => x.EmpresaIdempresa == empresaId)
        .ToListAsync();

      //var corridas = await (from v in context.Ruta
      //                      where v.EmpresaIdempresa == empresaId
      //                      select v
      //                      ).Include(x => x.Corrida)
      //                      .ThenInclude(x => x.CorridaDia)
      //                      .ThenInclude(x => x.DiasIddiasNavigation).ToListAsync();


      return corridas!;
    }

    /// <summary>
    /// obtiene la configuracion de corrida de la ruta seleccionada
    /// </summary>
    /// <param name="id"></param>
    /// <param name="empresaId"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<AgregarCorridaViewModel> GetCorridasViewModel(int id, int empresaId)
    {

      var ruta = await context.Ruta
        .FirstOrDefaultAsync(x => x.Idruta == id && x.EmpresaIdempresa == empresaId);

      if (ruta is null)
      {
        throw new Exception("No se encontró la ruta");
      }

      var corridas = await context.Corrida
        .Include(x => x.CorridaDia)
        .Where(x => x.RutaIdruta == ruta.Idruta && x.EmpresaIdempresa == empresaId)
        .ToListAsync();


      var model = new AgregarCorridaViewModel
      {
        RutaId = ruta.Idruta,
        NombreRuta = ruta.Nombre!,
      };


      if (corridas.Any())
      {
        var dias = await context.Dias
          .OrderBy(x => x.Orden)
          .ToListAsync();

        model.Corridas = corridas.Select(x => new CorridaViewModel
        {
          CorridaId = x.Idcorrida,
          Inicio = (TimeOnly)x.HoraInicio!,
          Fin = (TimeOnly)x.HoraFin!,
          Dias = dias.Select(y => new DiasViewModel
          {
            DiaId = y.Iddias,
            Nombre = y.Nombre!,
            Orden = (int)y.Orden!,
            Activo = x.CorridaDia.Any(z => z.DiasIddias == y.Iddias && z.Activo == 1)
          }).ToList()

        }).ToList();


      }
      else
      {

        model.Corridas = new List<CorridaViewModel>
        {
          await AddNuevaCorrida()
        };


      }


      return model;

    }

    /// <summary>
    /// obtiene una nueva corrida
    /// </summary>
    /// <returns></returns>
    public async Task<CorridaViewModel> AddNuevaCorrida()
    {
      var dias = await context.Dias
           .OrderBy(x => x.Orden)
           .ToListAsync();

      var model = new CorridaViewModel
      {
        CorridaId = 0,
        Inicio = new TimeOnly(0, 0, 0),
        Fin = new TimeOnly(0, 0, 0),
      };

      model.Dias = dias.Select(x => new DiasViewModel
      {
        DiaId = x.Iddias,
        Nombre = x.Nombre!,
        Orden = (int)x.Orden!,
        Activo = false
      }).ToList();

      return model;
    }


    /// <summary>
    /// guarda la configuracion de las corridas
    /// </summary>
    /// <param name="model"></param>
    /// <param name="empresaId"></param>
    /// <returns></returns>
    public async Task<bool> AgregarCorrida(AgregarCorridaViewModel model, int empresaId)
    {
      if (model.Corridas.Any())
      {
        await EliminaCorrida(model.RutaId, empresaId);
        foreach (var corrida in model.Corridas)
        {

          var existeCorrida = await context.Corrida
            .AnyAsync(x => x.RutaIdruta == model.RutaId && x.HoraInicio == corrida.Inicio && x.HoraFin == corrida.Fin);

          if (!existeCorrida)
          {
            Corridum corridaEntity = new()
            {
              RutaIdruta = model.RutaId,
              HoraInicio = corrida.Inicio,
              HoraFin = corrida.Fin,
              EmpresaIdempresa = empresaId
            };

            context.Corrida.Add(corridaEntity);
            await context.SaveChangesAsync();

            foreach (var dia in corrida.Dias)
            {
              CorridaDia corridaDia = new()
              {
                CorridaIdcorrida = corridaEntity.Idcorrida,
                DiasIddias = dia.DiaId,
                Activo = (ulong?)(dia.Activo ? 1 : 0)
              };

              context.CorridaDias.Add(corridaDia);
            }
            await context.SaveChangesAsync();
          }
          else
          {
            // si ya existe validar la diferencia de dias y actualizar
            var corridaEntity = await context.Corrida
               .FirstOrDefaultAsync(x => x.RutaIdruta == model.RutaId && x.HoraInicio == corrida.Inicio && x.HoraFin == corrida.Fin);


            if (corridaEntity is not null)
            {
              var corridaDias = await context.CorridaDias
                .Where(x => x.CorridaIdcorrida == corridaEntity.Idcorrida)
                .ToListAsync();

              foreach (var dia in corrida.Dias)
              {
                var corridaDia = corridaDias.FirstOrDefault(x => x.DiasIddias == dia.DiaId);

                if (corridaDia is not null)
                {
                  corridaDia.Activo = (ulong?)(dia.Activo ? 1 : 0);
                }
                else
                {
                  CorridaDia corridaDiaEntity = new()
                  {
                    CorridaIdcorrida = corridaEntity.Idcorrida,
                    DiasIddias = dia.DiaId,
                    Activo = (ulong?)(dia.Activo ? 1 : 0)
                  };

                  context.CorridaDias.Add(corridaDiaEntity);
                }
              }

              await context.SaveChangesAsync();
            }
          }

        }


      }


      return true;
    }

    /// <summary>
    /// elimina la configuracion de la corrida seleccionada
    /// </summary>
    /// <param name="rutaId"></param>
    /// <param name="empresaId"></param>
    /// <returns></returns>
    public async Task<bool> EliminaCorrida(int rutaId, int empresaId)
    {
      //var corridas = await context.Corrida
      //  .Where(x => x.RutaIdruta == rutaId && x.EmpresaIdempresa == empresaId)
      //  .ToListAsync();

      var corridas = await (from v in context.Corrida
                            where v.RutaIdruta == rutaId && v.EmpresaIdempresa == empresaId
                            && !v.CorridaAsignacions.Any()
                            select v
                            ).ToListAsync();


      if (corridas.Any())
      {
        foreach (var corrida in corridas)
        {
          var corridaDias = await context.CorridaDias
            .Where(x => x.CorridaIdcorrida == corrida.Idcorrida)
            .ExecuteDeleteAsync();

          await context.CorridaParada
            .Where(x => x.CorridaIdcorrida == corrida.Idcorrida)
            .ExecuteDeleteAsync();

          await context.Corrida
           .Where(x => x.Idcorrida == corrida.Idcorrida && x.EmpresaIdempresa == empresaId)
           .ExecuteDeleteAsync();

        }


        await context.SaveChangesAsync();

        return true;
      }

      return false;


    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="corridaId"></param>
    /// <param name="empresaId"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<bool> EliminarHorarioCorrida(int corridaId, int empresaId)
    {
      //var corridas = await context.Corrida
      //  .Where(x => x.RutaIdruta == rutaId && x.EmpresaIdempresa == empresaId)
      //  .ToListAsync();

      var corridas = await (from v in context.Corrida
                            where v.Idcorrida == corridaId && v.EmpresaIdempresa == empresaId
                            && !v.CorridaAsignacions.Any()
                            select v
                            ).ToListAsync();


      if (corridas.Any())
      {
        foreach (var corrida in corridas)
        {
          var corridaDias = await context.CorridaDias
            .Where(x => x.CorridaIdcorrida == corrida.Idcorrida)
            .ExecuteDeleteAsync();

          await context.CorridaParada
            .Where(x => x.CorridaIdcorrida == corrida.Idcorrida)
            .ExecuteDeleteAsync();

        }


        await context.Corrida
        .Where(x => x.Idcorrida == corridaId && x.EmpresaIdempresa == empresaId)
        .ExecuteDeleteAsync();


        await context.SaveChangesAsync();

        return true;
      }
      else
      {
        throw new Exception("No se puede eliminar la corrida ya que tiene asignaciones");
      }




    }

    /// <summary>
    ///  obtiene el detalle de la ruta seleccionada
    /// </summary>
    /// <param name="corridaId"></param>
    /// <param name="empresaId"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<DetalleRutaViewModel> GetDetalleRutaViewModel(int corridaId, int empresaId)
    {
      var corrida = await context.Corrida
        .Include(x => x.RutaIdrutaNavigation)
        .FirstOrDefaultAsync(x => x.Idcorrida == corridaId && x.EmpresaIdempresa == empresaId);


      if (corrida is null)
      {
        throw new Exception("No se encontró la corrida");
      }
      var tiempoEspera = await context.Variableempresas
                .FirstOrDefaultAsync(x => x.EmpresaIdempresa == empresaId && x.VariableIdvariable == 9);
      int tiempoEsperaMinutos = 0;
      if (tiempoEspera != null)
      {
        tiempoEsperaMinutos = int.Parse(tiempoEspera.Valor!);
      }
      var puntos = await context.RutaParada
        .Include(x => x.ParadaIdparadaNavigation)
        .Where(x => x.RutaIdruta == corrida.RutaIdrutaNavigation.Idruta)
        .OrderBy(x => x.Orden)
        .ToListAsync();

      var corridaParada = context.CorridaParada
        .Where(x => x.CorridaIdcorrida == corrida.Idcorrida)
        .ToList();


      var model = new DetalleRutaViewModel();

      model.CorridaId = corrida.Idcorrida;
      model.Estaciones = puntos.Select(x => new EstacionViewModel
      {
        ParadaId = x.ParadaIdparada,
        NombreEstacion = x.ParadaIdparadaNavigation.Nombre!,
        Tiempo = (int)(x.Tiemposeg == null || x.Tiemposeg == 0 ? 0
                : ((x.Tiemposeg / 60) + tiempoEsperaMinutos))
      }).ToList();

      foreach (var estacion in model.Estaciones)
      {
        var corridaParadaEntity = corridaParada.FirstOrDefault(x => x.ParadaIdparada == estacion.ParadaId);

        if (corridaParadaEntity is not null)
        {
          estacion.Horario = (TimeOnly)corridaParadaEntity.Hora!;
        }
      }

      model.CantidadEstaciones = model.Estaciones.Count;

      // colocar el horario de la corrida inicio a la primera estacion y el fin a la ultima estacion
      if (!corridaParada.Any())
      {
        if (model.Estaciones.Any())
        {
          model.Estaciones.First().Horario = (TimeOnly)corrida.HoraInicio!;
          model.Estaciones.Last().Horario = (TimeOnly)corrida.HoraFin!;
        }
      }

      return model;
    }

    /// <summary>
    /// guarda el detalle de la corrida seleccionada
    /// </summary>
    /// <param name="model"></param>
    /// <param name="empresaId"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<bool> AgregarDetalleRuta(DetalleRutaViewModel model, int empresaId)
    {
      var corrida = await context.Corrida
        .Include(x => x.RutaIdrutaNavigation)
        .FirstOrDefaultAsync(x => x.Idcorrida == model.CorridaId && x.EmpresaIdempresa == empresaId);

      if (corrida is null)
      {
        throw new Exception("No se encontró la corrida");
      }


      // validar que los horarios de las estaciones sean mayor que el anterior y menor que el siguiente
      ValidarHorarios(model);


      // elimina los horarios de la corrida seleccionada
      var corridaParada = await context.CorridaParada
        .Where(x => x.CorridaIdcorrida == corrida.Idcorrida)
        .ExecuteDeleteAsync();


      foreach (var estacion in model.Estaciones)
      {
        CorridaParadum corridaParadaEntity = new()
        {
          CorridaIdcorrida = corrida.Idcorrida,
          ParadaIdparada = estacion.ParadaId,
          Hora = estacion.Horario,
          Fecharegistro = DateTime.UtcNow,
          Activo = 1
        };

        context.CorridaParada.Add(corridaParadaEntity);
      }


      await context.SaveChangesAsync();

      return true;
    }

    /// <summary>
    /// valida los horarios de las estaciones de la corrida
    /// </summary>
    /// <param name="model"></param>
    /// <exception cref="Exception"></exception>
    private void ValidarHorarios(DetalleRutaViewModel model)
    {
      for (int i = 0; i < model.Estaciones.Count; i++)
      {
        if (i > 0)
        {
          if (model.Estaciones[i].Horario < model.Estaciones[i - 1].Horario)
          {
            throw new Exception($"El horario de la estación {model.Estaciones[i].NombreEstacion} debe ser mayor que el horario de la estación {model.Estaciones[i - 1].NombreEstacion}");
          }
        }

        if (i < model.Estaciones.Count - 1)
        {
          if (model.Estaciones[i].Horario > model.Estaciones[i + 1].Horario)
          {
            throw new Exception($"El horario de la estación {model.Estaciones[i].NombreEstacion} debe ser menor que el horario de la estación {model.Estaciones[i + 1].NombreEstacion}");
          }
        }
      }
    }


    /// <summary>
    /// Elimina toda la corrida de la ruta seleccionada
    /// </summary>
    /// <param name="rutaId"></param>
    /// <param name="empresaId"></param>
    /// <returns></returns>
    public async Task<bool> EliminaTodaCorrida(int rutaId, int empresaId)
    {

      var corridas = await (from v in context.Corrida
                            where v.RutaIdruta == rutaId && v.EmpresaIdempresa == empresaId
                            select v
                            ).ToListAsync();


      if (corridas.Any())
      {
        foreach (var corrida in corridas)
        {
          var asignacion = await context.CorridaAsignacions
            .Where(x => x.CorridaIdcorrida == corrida.Idcorrida)
            .ExecuteDeleteAsync();

          var corridaDias = await context.CorridaDias
            .Where(x => x.CorridaIdcorrida == corrida.Idcorrida)
            .ExecuteDeleteAsync();

          await context.CorridaParada
            .Where(x => x.CorridaIdcorrida == corrida.Idcorrida)
            .ExecuteDeleteAsync();

          await context.Corrida
           .Where(x => x.Idcorrida == corrida.Idcorrida && x.EmpresaIdempresa == empresaId)
           .ExecuteDeleteAsync();

        }


        await context.SaveChangesAsync();

        return true;
      }

      return false;


    }
  }
}

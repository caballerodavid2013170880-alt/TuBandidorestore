using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.ViewModel.Configuracion;
using System.Linq;
using static SUVAN.BackOffice.Models.StoredsProcedures.ModelsStoredsProcedures;
using static SUVAN.BackOffice.Models.UnlimintPay.Pago.RespuestaValidacionPagoWS;

namespace SUVAN.BackOffice.Service.Configuracion
{
  public class RutasService : IRutasService
  {
    private readonly SuvanDbContext context;
    private readonly ICorridasService corridasService;

    public RutasService(SuvanDbContext context, ICorridasService corridasService)
    {
      this.context = context;
      this.corridasService = corridasService;
    }


    /// <summary>
    /// Obtiene la lista de rutas desde la base de datos.
    /// </summary>
    /// <returns>Lista de rutas.</returns>
    public async Task<List<Rutum>> ObtenerRutas(int empresaId)
    {
      return await context.Ruta
        .Where(x => x.EmpresaIdempresa == empresaId)
        .ToListAsync();
    }


    /// <summary>
    /// Obtiene el ViewModel para la ruta específica.
    /// </summary>
    /// <param name="id">Identificador de la ruta.</param>
    /// <returns>ViewModel de la ruta.</returns>
    public async Task<AgregarRutaViewModel> GetRutaViewModel(int id, int empresaId)
    {
      var ruta = await context.Ruta
        .FirstOrDefaultAsync(x => x.Idruta == id);

      if (ruta == null)
        return new AgregarRutaViewModel();

      var model = new AgregarRutaViewModel
      {
        RutaId = ruta.Idruta,
        Nombre = ruta.Nombre!,
        Activo = ruta.Activo == 1
      };

      List<PuntosViewModel> puntosViewModel = new();

      var rutaParadas = await context.RutaParada
         .Include(x => x.ParadaIdparadaNavigation)
         .Where(x => x.RutaIdruta == ruta.Idruta)
         .OrderBy(x => x.Orden)
         .ToListAsync();

      foreach (var rutaParada in rutaParadas)
      {


        PuntosViewModel punto = new()
        {
          id = rutaParada.ParadaIdparada,
          label = rutaParada.ParadaIdparadaNavigation.Nombre!,
          tipo = rutaParada.TipoestacionIdtipoestacion ?? 0,
          tiempo = rutaParada.Tiemposeg ?? 0,
          order = (int)rutaParada.Orden!
        };

        var puntosVirtuales = await context.Puntovirtuals
          .Where(x => x.RutaParadaRutaIdruta == rutaParada.RutaIdruta
                               && x.RutaParadaParadaIdparada == rutaParada.ParadaIdparada)
          .OrderBy(x => x.Orden)
          .ToListAsync();

        punto.coordinatesArray = puntosVirtuales.Select(x => new CoordinatesViewModel
        {
          lat = (decimal)x.Latitud!,
          lng = (decimal)x.Longitud!
        }).ToList();

        puntosViewModel.Add(punto);
      }

      model.PuntosJson = Newtonsoft.Json.JsonConvert.SerializeObject(puntosViewModel);

      return model;
    }

    /// <summary>
    /// Agrega o actualiza una ruta en la base de datos.
    /// </summary>
    /// <param name="model">ViewModel de la ruta a agregar o actualizar.</param>
    /// <returns>Indica si la operación fue exitosa.</returns>
    /// <exception cref="Exception">Excepción lanzada en caso de errores.</exception>
    public async Task<bool> AgregarRuta(AgregarRutaViewModel model, int empresaId)
    {
      Rutum ruta = new();

      if (model.RutaId > 0)
      {
        ruta = await context.Ruta.FirstOrDefaultAsync(x => x.Idruta == model.RutaId);
        if (ruta == null)
        {
          throw new Exception("No se encontro la ruta");
        }

        await ValidarViajesRuta(model.RutaId);
      }

      var empresaRuta = await context.Ruta
        .FirstOrDefaultAsync(x => (x.Nombre == model.Nombre && x.EmpresaIdempresa == empresaId) && x.Idruta != model.RutaId);

      if (empresaRuta != null)
      {
        throw new Exception("Ya existe una ruta con el mismo nombre");
      }

      ruta.Nombre = model.Nombre.Trim();
      ruta.Activo = (ulong?)(model.Activo ? 1 : 0);
      ruta.Fecharegistro = DateTime.UtcNow;
      ruta.EmpresaIdempresa = empresaId;
      ruta.Distanciamts = model.Distancia;
      ruta.Googlemapsruta = model.GoogleMapsRuta;

      if (model.RutaId > 0)
      {
        context.Ruta.Update(ruta);
      }
      else
      {
        ruta.TipotarifaIdtipotarifa = 1;
        context.Ruta.Add(ruta);
      }

      await context.SaveChangesAsync();

      await GuardaDetalleRuta(model.PuntosJson, ruta.Idruta);

      return true;
    }

    private async Task ValidarViajesRuta(int rutaId)
    {

      var result = await (from v in context.Viajes
                          join ca in context.CorridaAsignacions
                             on v.CorridaAsignacionIdcorridaAsignacion equals ca.IdcorridaAsignacion into caGroup
                          from ca in caGroup.DefaultIfEmpty()
                          join c in context.Corrida
                             on ca.CorridaIdcorrida equals c.Idcorrida into cGroup
                          from c in cGroup.DefaultIfEmpty()
                          join r in context.Ruta
                             on c.RutaIdruta equals r.Idruta into rGroup
                          from r in rGroup.DefaultIfEmpty()
                          where r.Idruta == rutaId &&
                          (v.EstatusviajeIdestatusviaje == 1 || v.EstatusviajeIdestatusviaje == 2)
                          && v.Fechaviaje >= DateTime.Today.AddSeconds(-1)

                          select v).AnyAsync();

      if (result)
      {
        throw new Exception("No se puede modificar la ruta, ya que tiene viajes en espera o en curso");
      }


    }

    /// <summary>
    /// valida si la ruta tiene viajes asignados
    /// </summary>
    /// <param name="rutaId"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    private async Task ValidarTodosViajesRuta(int rutaId)
    {

      var result = await (from v in context.Viajes
                          join ca in context.CorridaAsignacions
                             on v.CorridaAsignacionIdcorridaAsignacion equals ca.IdcorridaAsignacion into caGroup
                          from ca in caGroup.DefaultIfEmpty()
                          join c in context.Corrida
                             on ca.CorridaIdcorrida equals c.Idcorrida into cGroup
                          from c in cGroup.DefaultIfEmpty()
                          join r in context.Ruta
                             on c.RutaIdruta equals r.Idruta into rGroup
                          from r in rGroup.DefaultIfEmpty()
                          where r.Idruta == rutaId
                          select v).AnyAsync();

      if (result)
      {
        throw new Exception("No se puede Eliminar la ruta, ya que tiene viajes");
      }


    }

    /// <summary>
    /// Guarda los detalles de la ruta, incluidos los puntos virtuales.
    /// </summary>
    /// <param name="puntosJson">Datos JSON de los puntos virtuales.</param>
    /// <param name="idruta">Identificador de la ruta.</param>
    /// <returns>Task.</returns>
    private async Task GuardaDetalleRuta(string puntosJson, int idruta)
    {
      // elimina los puntos virtuales configurados de una ruta
      await EliminaDetalleRutaPuntoVirtual(idruta);

      var puntos = Newtonsoft.Json.JsonConvert.DeserializeObject<List<PuntosViewModel>>(puntosJson);

      if (puntos != null && puntos.Any())
      {
        foreach (var punto in puntos)
        {
          var rutaParada = new RutaParadum
          {
            Activo = 1,
            Fecharegistro = DateTime.UtcNow,
            Orden = punto.order,
            ParadaIdparada = punto.id,
            TipoestacionIdtipoestacion = punto.tipo,
            Tiemposeg = punto.tiempo,
            RutaIdruta = idruta
          };

          context.RutaParada.Add(rutaParada);
          //await context.SaveChangesAsync();

          var ordenPuntoVirtual = 1;
          foreach (var puntoVirtual in punto.coordinatesArray)
          {
            var puntoVirtualEntity = new Puntovirtual
            {
              Latitud = puntoVirtual.lat,
              Longitud = puntoVirtual.lng,
              RutaParadaRutaIdruta = rutaParada.RutaIdruta,
              RutaParadaParadaIdparada = rutaParada.ParadaIdparada,
              Fecharegistro = DateTime.UtcNow,
              Activo = 1,
              Orden = ordenPuntoVirtual
            };

            context.Puntovirtuals.Add(puntoVirtualEntity);

            ordenPuntoVirtual++;
          }

        }

        await context.SaveChangesAsync();
      }

    }
    /// <summary>
    /// Elimina los puntos virtuales configurados de una ruta.
    /// </summary>
    /// <param name="idruta">Identificador de la ruta.</param>
    /// <returns>Task.</returns>
    private async Task EliminaDetalleRutaPuntoVirtual(int idruta)
    {
      // Elimina los puntos virtuales asociados a la ruta.
      var puntosVirtuales = await context.Puntovirtuals
       .Where(x => x.RutaParadaRutaIdruta == idruta).ExecuteDeleteAsync();

      // Elimina las relaciones ruta-parada asociadas a la ruta.
      var rutaParadas = await context.RutaParada
        .Where(x => x.RutaIdruta == idruta).ExecuteDeleteAsync();

    }

    /// <summary>
    /// obtine las rutas sin configurar
    /// </summary>
    /// <param name="empresaId"></param>
    /// <returns></returns>
    public async Task<List<ModelRutaConfiguracion>> GetRutasSinConfigurar(int empresaId)
    {
      List<ModelRutaConfiguracion> rutas = new();
      string query = String.Format("CALL sp_s_ConfiguracionRuta({0});", empresaId);
      rutas = await context.Set<ModelRutaConfiguracion>().FromSqlRaw(query).ToListAsync();
      return rutas;

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="empresaId"></param>
    /// <returns></returns>
    public async Task<List<ModelRutaConfiguracion>> GetRutasSinConfigurarSoloRed(int empresaId)
    {
      
            List<ModelRutaConfiguracion> rutas = new();
      string query = String.Format("CALL sp_s_ConfiguracionRuta({0});", empresaId);
      rutas = await context.Set<ModelRutaConfiguracion>().FromSqlRaw(query).ToListAsync();
      // devuelve solo las rutas que tienen al menos un false en las columnas de ModelRutaConfiguracion
      rutas = rutas.Where(x => x.corrida == false || x.estacion == false || x.asignacion == false || (x.tarifaEsc == false && x.tarifaGen == false)).ToList();

      return rutas;

    }

    /// <summary>
    /// Elimina la ruta de la base de datos
    /// </summary>
    /// <param name="rutaId"></param>
    /// <param name="empresaId"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<bool> EliminarRuta(int rutaId, int empresaId)
    {
      var ruta = await context.Ruta.FirstOrDefaultAsync(x => x.Idruta == rutaId && x.EmpresaIdempresa == empresaId);

      if (ruta is null)
      {
        throw new Exception("No se encontro la ruta");
      }

      await ValidarTodosViajesRuta(rutaId);

      await corridasService.EliminaTodaCorrida(rutaId, empresaId);

      await EliminaDetalleRutaPuntoVirtual(rutaId);

      await EliminarTarifas(rutaId, empresaId);

      //context.Ruta.Remove(ruta);

      // Desactivar temporamente el seguimiento de entidades relacionadas
      context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;


      var delete = await context.Ruta
        .Where(x => x.Idruta == rutaId && x.EmpresaIdempresa == empresaId)
        .ExecuteDeleteAsync();

      await context.SaveChangesAsync();

      // Volver a activar el seguimiento de entidades relacionadas
      context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.TrackAll;
      return true;
    }

    private async Task EliminarTarifas(int rutaId, int empresaId)
    {
      var tarifas = await context.TarifaGenerals
        .Where(x => x.RutaIdruta == rutaId)
        .ExecuteDeleteAsync();

      var tarifaEscalonada = await context.TarifaEscalonada
        .Where(x => x.RutaIdruta == rutaId)
        .ExecuteDeleteAsync();

      await context.SaveChangesAsync();
    }
  }
}

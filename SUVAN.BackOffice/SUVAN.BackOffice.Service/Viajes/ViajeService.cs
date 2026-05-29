using Microsoft.EntityFrameworkCore;
using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.ManejoRespuesta;
using SUVAN.BackOffice.Models.Viajes;
using SUVAN.BackOffice.Models.ViewModel.Comercial;
using SUVAN.BackOffice.Models.ViewModel.Enums;
using SUVAN.BackOffice.Service.Notificaciones;
using SUVAN.BackOffice.Service.Pago;
using System.Data;
using System.Globalization;
using static SUVAN.BackOffice.Models.StoredsProcedures.ModelsStoredsProcedures;

namespace SUVAN.BackOffice.Service.Viajes
{
  public class ViajeService : IViajeService
  {
    private readonly SuvanDbContext _context;

    private readonly IPagoService _pagoService;
    private readonly INotificacionPushService notificacionPushService;
    //private readonly SemaphoreSlim _semaphore;// Permitir solo una operación a la vez

    public ViajeService(SuvanDbContext context, IPagoService pagoService, INotificacionPushService notificacionPushService)
    {
      _context = context;
      _pagoService = pagoService;
      this.notificacionPushService = notificacionPushService;
      //this._semaphore = semaphore;
    }

    public async Task<SuVanResponse<List<ViajeServicioResponse>>> BuscaServicio(int userId, string emailUser, ViajeServicioRequest data)
    {
      SuVanResponse<List<ViajeServicioResponse>> response = new();
      List<ViajeServicioResponse> listaServicios = new List<ViajeServicioResponse>();

      string variabledistanciamts = await (from a in _context.Variables join b in _context.Variableglobals on a.Idvariable equals b.VariableIdvariable where a.Idvariable == Convert.ToInt32(EnumVariable.KMD) select b.Valor).FirstOrDefaultAsync();
      int distanciamts = variabledistanciamts != null ? Convert.ToInt32(variabledistanciamts) : 15;
      distanciamts = distanciamts * 1000; //SE CONVIERTE KM A MTS

      List<ModelstpBuscaServicio> lstBS = new List<ModelstpBuscaServicio>();

      string query = String.Format("CALL stpBuscaServicio({0},{1},{2},{3},{4});", data.LatitudInicial, data.LongitudInicial, data.LatitudFinal, data.LongitudFinal, distanciamts);
      lstBS = _context.Set<ModelstpBuscaServicio>().FromSqlRaw(query).ToList<ModelstpBuscaServicio>();

      foreach (var item in lstBS)
      {
        //if (listaServicios.Where(x => x.RutaId == item.Idruta).FirstOrDefault() != null) { continue; }

        ViajeServicioResponse viaje = new ViajeServicioResponse();
        viaje.RutaId = item.Idruta;
        viaje.RutaNombre = item.Ruta;

        viaje.EstacionInicial = item.Paradainicial;
        viaje.EstacionFinal = item.Paradafinal;

        viaje.EstacionAbordaje = item.Paradainicial;
        viaje.EstacionAbordajeId = item.Idparadainicial;

        viaje.EstacionIntermedia = item.Paradainicial;
        viaje.EstacionDescenso = item.Paradafinal;
        viaje.EstacionDescensoId = item.Idparadafinal;

        decimal costo = await _pagoService.ObtenTarifa(userId, emailUser, item.Idruta, null, viaje.EstacionAbordajeId, viaje.EstacionDescensoId, string.Empty, false);

        viaje.Costo = String.Format("${0} MXN", costo);
        viaje.CostoPromocion = String.Format("${0} MXN", costo);
        listaServicios.Add(viaje);
      }

      response.Data = listaServicios;
      response.CodigoMensaje = "200";
      response.Mensaje = response.Data.ToArray().Length == 0 ? "No se encontraron servicios" : "Solicitud exitosa";

      return response;
    }

    public async Task<SuVanResponse<List<ViajeDisponibilidadResponse>>> BuscaDisponibilidad(int userId, string emailUser, ViajeDisponibilidadRequest data)
    {
      SuVanResponse<List<ViajeDisponibilidadResponse>> response = new();
      List<ViajeDisponibilidadResponse> listaViajes = new List<ViajeDisponibilidadResponse>();

      //data.Fecha = string.Format("{0} {1}:{2}", data.Fecha, DateTime.Now.Hour, DateTime.Now.Minute);


      DateTime fechaConsulta;
      DateTime.TryParseExact(data.Fecha,
                      "dd-MM-yyyy",
                      CultureInfo.InvariantCulture,
                      DateTimeStyles.None,
                      out fechaConsulta);

      // obtiene las corridas que son de la fecha de consulta y que no esten finalizadas
      var corridasResult = await (from ruta in _context.Ruta
                                  join corrida in _context.Corrida on ruta.Idruta equals corrida.RutaIdruta
                                  join corridaAsignacion in _context.CorridaAsignacions on corrida.Idcorrida equals corridaAsignacion.CorridaIdcorrida
                                  join vehiculo in _context.Vehiculos on corridaAsignacion.VehiculoIdvehiculo equals vehiculo.IdVehiculo
                                  join tipoVehiculo in _context.Tipovehiculos on vehiculo.TipovehiculoIdtipovehiculo equals tipoVehiculo.Idtipovehiculo
                                  where ruta.Idruta == data.RutaId
                                  && corridaAsignacion.Fecha == fechaConsulta
                                  && (corridaAsignacion.EstatusviajeIdestatusviaje == Convert.ToInt32(EnumEstatusViaje.EN_CURSO)
                                  || corridaAsignacion.EstatusviajeIdestatusviaje == Convert.ToInt32(EnumEstatusViaje.EN_ESPERA))
                                  select new { ruta, corridaAsignacion, tipoVehiculo }).ToListAsync();

      var rutaParadasResult = await (from a in _context.RutaParada
                                     where a.RutaIdruta == data.RutaId
                                     && a.Activo == Convert.ToUInt64(true)
                                     select a).OrderBy(x => x.Orden).ToListAsync();


      List<CorridaAsignacion> corridasAgregadas = new List<CorridaAsignacion>();
      foreach (var corrida in corridasResult)
      {
        int asientosPorVehiculo = corrida.tipoVehiculo.Asientos ?? 0;
        int asientosDisponibles = 0;
        if (corrida.ruta.TipotarifaIdtipotarifa == Convert.ToInt32(EnumTipoTarifa.FIJA))
        {
          asientosDisponibles = await (from a in _context.CorridaAsignacionParada
                                       where a.CorridaAsignacionIdcorridaAsignacion == corrida.corridaAsignacion.IdcorridaAsignacion
                                       select a).MinAsync(x => x.Espacios) ?? 0;
        }
        else
        {
          var estacionesIntermedias = await ObtenEstacionesIntermedias(rutaParadasResult, data.EstacionAbordajeId, data.EstacionDescensoId);

          asientosDisponibles = await (from a in _context.CorridaAsignacionParada
                                       where a.CorridaAsignacionIdcorridaAsignacion == corrida.corridaAsignacion.IdcorridaAsignacion
                                       && estacionesIntermedias.Contains(a.ParadaIdparada)
                                       select a).MinAsync(x => x.Espacios) ?? 0;
        }


        DateTime hoy = DateTime.Now;

        var reservados = await (from v in _context.Viajes
                                where v.CorridaAsignacionIdcorridaAsignacion == corrida.corridaAsignacion.IdcorridaAsignacion
                                && v.EstatusviajeIdestatusviaje == 0 && v.Vigenciareserva > hoy
                                select v.Numeropasajeros).CountAsync();

        if ((asientosDisponibles - reservados) < data.Pasajeros) { continue; }

        //VALIDAMOS SI YA EXISTE LA CORRIDA INCLUIDA EN LA DISPONIBILIDAD Y MANTENEMOS SOLO LA QUE TIENE MENOS ASIENTOS DISPONIBLES
        var existeCorrida = corridasAgregadas.Where(x => x.CorridaIdcorrida == corrida.corridaAsignacion.CorridaIdcorrida).FirstOrDefault();

        if (existeCorrida != null)
        {
          var viajeDisponible = listaViajes.Where(x => x.CorridaId == existeCorrida.IdcorridaAsignacion).FirstOrDefault();
          if (viajeDisponible.AsientosDisponibles > asientosDisponibles)
          {
            listaViajes.Remove(viajeDisponible);
          }
          else
          {
            continue;
          }
        }


        if (fechaConsulta.Date == DateTime.Now.Date)
        {
          var corr = await (from co in _context.Corrida
                            where co.Idcorrida == corrida.corridaAsignacion.CorridaIdcorrida
                            select co).FirstOrDefaultAsync();
          if (corr.HoraInicio.Value < TimeOnly.FromDateTime(DateTime.Now.AddHours(0)))
          {
            continue;
          }
        }


        var estacionAbordajeResult = await (from corridaParada in _context.CorridaParada
                                            where corridaParada.ParadaIdparada == data.EstacionAbordajeId
                                            && corridaParada.CorridaIdcorrida == corrida.corridaAsignacion.CorridaIdcorrida
                                            select corridaParada).Include(x => x.ParadaIdparadaNavigation).FirstOrDefaultAsync();

        var estacionDescensoResult = await (from corridaParada in _context.CorridaParada
                                            where corridaParada.ParadaIdparada == data.EstacionDescensoId
                                            && corridaParada.CorridaIdcorrida == corrida.corridaAsignacion.CorridaIdcorrida
                                            select corridaParada).Include(x => x.ParadaIdparadaNavigation).FirstOrDefaultAsync();

        ViajeDisponibilidadResponse viaje = new ViajeDisponibilidadResponse();
        viaje.CorridaId = corrida.corridaAsignacion.IdcorridaAsignacion;
        viaje.RutaId = corrida.ruta.Idruta;
        viaje.RutaNombre = corrida.ruta.Nombre;

        viaje.EstacionAbordajeId = estacionAbordajeResult.ParadaIdparadaNavigation.Idparada;
        viaje.EstacionAbordaje = estacionAbordajeResult.ParadaIdparadaNavigation.Nombre;
        viaje.HoraAbordaje = estacionAbordajeResult.Hora.Value.ToString("HH:mm");

        viaje.EstacionDescenso = estacionDescensoResult.ParadaIdparadaNavigation.Nombre;
        viaje.EstacionDescensoId = estacionDescensoResult.ParadaIdparadaNavigation.Idparada;
        viaje.HoraDescenso = estacionDescensoResult.Hora.Value.ToString("HH:mm");

        viaje.AsientosPorVehiculo = asientosPorVehiculo;
        viaje.AsientosDisponibles = asientosDisponibles;
        viaje.CostoPromocion = null;

        decimal costo = await _pagoService.ObtenTarifa(userId, emailUser, corrida.ruta.Idruta, corrida.corridaAsignacion.CorridaIdcorrida, viaje.EstacionAbordajeId, viaje.EstacionDescensoId, string.Empty, false);
        decimal costoPromocion = await _pagoService.AplicaPromocion(costo, userId, emailUser, corrida.ruta.Idruta, corrida.corridaAsignacion.CorridaIdcorrida, string.Empty);

        if (costoPromocion == costo)
        {
          viaje.Costo = String.Format("{0:C} MXN", (costo * data.Pasajeros));
        }
        else
        {
          viaje.Costo = String.Format("{0:C} MXN", (costo * data.Pasajeros));
          viaje.CostoPromocion = String.Format("{0:C} MXN", (costoPromocion * data.Pasajeros));
        }


        if (costo == 0)
        {
          continue;
        }

        listaViajes.Add(viaje);

        //GUARDAMOS TEMPORALMENTE LAS CORRIDAS ASIGNADAS EN LA DISPONIBILIDAD





        corridasAgregadas.Add(corrida.corridaAsignacion);
      }

      response.Data = listaViajes;
      response.CodigoMensaje = "200";
      if (listaViajes.Count == 0)
      {
        response.CodigoMensaje = "400";
      }
      response.Mensaje = response.Data.ToArray().Length == 0 ? "No se encontro disponibilidad" : "Solicitud exitosa";

      return response;
    }

    public async Task<SuVanResponse<ReservacionViajeReponse>> ApartaReservacion(ReservacionViajeRequest data, int userId)
    {
      SuVanResponse<ReservacionViajeReponse> response = new();

      //Random random = new Random();
      //// Generar un retraso aleatorio entre 100 ms y 600 ms
      //int delay = random.Next(100, 601);
      //await Task.Delay(delay);

      //using (var transaction = await _context.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable))
      //{
      try
      {


        string boleto = "";

        DateTime fhVigencia = DateTime.Now.AddMinutes(5);

        var infoParada = await (from a in _context.CorridaAsignacions
                                join b in _context.Corrida on a.CorridaIdcorrida equals b.Idcorrida
                                join c in _context.CorridaParada on b.Idcorrida equals c.CorridaIdcorrida
                                join d in _context.Ruta on b.RutaIdruta equals d.Idruta
                                where a.IdcorridaAsignacion == data.CorridaId
                                && c.ParadaIdparada == data.EstacionAbordajeId
                                select new
                                {
                                  RutaId = d.Idruta,
                                  d.TipotarifaIdtipotarifa,
                                  d.EmpresaIdempresa,
                                  Fechaviaje = a.Fecha + c.Hora.Value.ToTimeSpan()
                                }).FirstOrDefaultAsync();

        if (infoParada == null)
        {
          response.CodigoMensaje = "400";
          response.Mensaje = "No se puede obtener la fecha y hora de la corrida";
          return response;
        }

        var rutaParadasResult = await (from a in _context.RutaParada
                                       where a.RutaIdruta == infoParada.RutaId
                                       && a.Activo == Convert.ToUInt64(true)
                                       select a).OrderBy(x => x.Orden).ToListAsync();

        var asientosDisponibles = 0;
        List<int> estacionesIntermedias = new List<int>();
        if (infoParada.TipotarifaIdtipotarifa == Convert.ToInt32(EnumTipoTarifa.FIJA))
        {
          asientosDisponibles = await (from a in _context.CorridaAsignacionParada
                                       where a.CorridaAsignacionIdcorridaAsignacion == data.CorridaId
                                       select a).MinAsync(x => x.Espacios) ?? 0;
        }
        else
        {
          estacionesIntermedias = await ObtenEstacionesIntermedias(rutaParadasResult, data.EstacionAbordajeId, data.EstacionDescensoId);

          asientosDisponibles = await (from a in _context.CorridaAsignacionParada
                                       where a.CorridaAsignacionIdcorridaAsignacion == data.CorridaId
                                       && estacionesIntermedias.Contains(a.ParadaIdparada)
                                       select a).MinAsync(x => x.Espacios) ?? 0;
        }

        DateTime hoy = DateTime.Now;



        // calculo de asientos reservados por parada tomando en cuenta las estaciones por el orden configurada en la corrida
        int ordenParaInicio = rutaParadasResult.FirstOrDefault(x => x.ParadaIdparada == data.EstacionAbordajeId)!.Orden ?? 0;
        int ordenParaFin = rutaParadasResult.FirstOrDefault(x => x.ParadaIdparada == data.EstacionDescensoId)!.Orden ?? 0;

        var reservados = await (from v in _context.Viajes
                                join pi in _context.RutaParada on new { RutaID = infoParada.RutaId, ParadaID = v.ParadaInicio } equals new { RutaID = infoParada.RutaId, ParadaID = pi.ParadaIdparada }
                                join pf in _context.RutaParada on new { RutaID = infoParada.RutaId, ParadaID = v.ParadaFin } equals new { RutaID = infoParada.RutaId, ParadaID = pf.ParadaIdparada }
                                where v.CorridaAsignacionIdcorridaAsignacion == data.CorridaId
                                && v.EstatusviajeIdestatusviaje == 0
                                && v.Vigenciareserva > hoy
                                && ((pi.Orden <= ordenParaInicio && pf.Orden > ordenParaInicio)
                                  || (pi.Orden < ordenParaFin && pf.Orden >= ordenParaFin)
                                  || (pi.Orden >= ordenParaInicio && pf.Orden <= ordenParaFin))
                                select v.Numeropasajeros)
                                .SumAsync();




        if ((asientosDisponibles - reservados) < data.Pasajeros)
        {
          response.Data = new ReservacionViajeReponse { ReservacionId = 0 };
          response.CodigoMensaje = "202";
          response.Mensaje = "No se puede apartar la reservación por falta de asientos";
          return response;
        }



        #region Se guarda información de viaje redondo
        int? ViajeRedondoID = null;
        if (data.agregaviajeredondo)
        {
          #region Validamos datos de entrada
          //if (data.viajeredondo.origenId <= 0)
          //{
          //    response.CodigoMensaje = "400";
          //    response.Mensaje = "Falta el parámetro origenId";
          //    return response;
          //}
          //if (data.viajeredondo.destinoId <= 0)
          //{
          //    response.CodigoMensaje = "400";
          //    response.Mensaje = "Falta el parámetro origenId";
          //    return response;
          //}
          if (data.viajeredondo.origenlatitud == 0)
          {
            response.CodigoMensaje = "400";
            response.Mensaje = "Falta el parámetro origenlatitud";
            return response;
          }
          if (data.viajeredondo.destinolongitud == 0)
          {
            response.CodigoMensaje = "400";
            response.Mensaje = "Falta el parámetro destinolongitud";
            return response;
          }
          #endregion
          Database.Entities.Viajeredondo viajeredondo = new Database.Entities.Viajeredondo()
          {
            Origennombre = data.viajeredondo.origennombre,
            //Origenid = data.viajeredondo.origenId,
            Origenlatitud = data.viajeredondo.origenlatitud,
            Origenlongitud = data.viajeredondo.origenlongitud,
            Origendireccion = data.viajeredondo.origendireccion,

            Destinonombre = data.viajeredondo.destinonombre,
            //Destinoid = data.viajeredondo.destinoId,
            Destinolatitud = data.viajeredondo.destinolatitud,
            Destinolongitud = data.viajeredondo.destinolongitud,
            Destinodireccion = data.viajeredondo.destinodireccion,

            Fecharegistro = DateTime.Now,
          };
          _context.Viajeredondos.Add(viajeredondo);
          await _context.SaveChangesAsync();
          ViajeRedondoID = viajeredondo.Idviajeredondo;
        }
        #endregion

        boleto = await Utilities.GeneraCodigos.GetGeneraCodigoAlfa();

        Viaje viaje = new Viaje()
        {
          UsuarioIdusuario = userId,
          EstatusviajeIdestatusviaje = (sbyte)(EnumEstatusViaje.RESERVANDO), //Verificar como se va a obtener el estatus
          Fechaviaje = infoParada.Fechaviaje,
          Vigenciareserva = fhVigencia,
          ParadaInicio = data.EstacionAbordajeId,
          ParadaFin = data.EstacionDescensoId,
          TransaccionIdtransaccion = null,
          CorridaAsignacionIdcorridaAsignacion = data.CorridaId,
          Numeropasajeros = data.Pasajeros,
          Boleto = boleto,
          EmpresaIdempresa = infoParada.EmpresaIdempresa,
          ViajeredondoIdviajeredondo = ViajeRedondoID
        };

        _context.Viajes.Add(viaje);
        await _context.SaveChangesAsync();

        //await transaction.CommitAsync();

        response.Data = new ReservacionViajeReponse { ReservacionId = viaje.Idviaje };
        response.CodigoMensaje = "200";
        response.Mensaje = "Reserva realizada con exito";

        return response;
      }
      catch (Exception)
      {

        //await transaction.RollbackAsync();
        response.CodigoMensaje = "400";
        response.Mensaje = $"Error al realizar la reserva";

      }
      //}
      return response;

    }

    public async Task<SuVanResponse<BoletoViajeResponse>> ObtenBoleto(BoletoViajeRequest data)
    {
      SuVanResponse<BoletoViajeResponse> response = new();
      BoletoViajeResponse? infoBoleto = await ObtenerInfoBoleto(data);

      response.Data = infoBoleto ?? new BoletoViajeResponse();
      response.CodigoMensaje = infoBoleto == null ? "400" : "200";
      response.Mensaje = infoBoleto == null ? "No se pudo obtener información del boleto" : "Solicitud exitosa";
      return response;
    }


    /// <summary>
    /// obtiene la infrmación del boleto
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    private async Task<BoletoViajeResponse?> ObtenerInfoBoleto(BoletoViajeRequest data)
    {

      //BoletoModel boleto = new BoletoModel();
      //boleto.Codigo = "VB1253";
      //boleto.QR = "VB1253";

      var infoBoleto = await (from a in _context.Viajes
                              join b in _context.CorridaAsignacions on a.CorridaAsignacionIdcorridaAsignacion equals b.IdcorridaAsignacion
                              join c in _context.Corrida on b.CorridaIdcorrida equals c.Idcorrida
                              join d in _context.Vehiculos on b.VehiculoIdvehiculo equals d.IdVehiculo
                              join e in _context.Conductors on b.ConductorIdconductor equals e.Idconductor
                              join f in _context.Parada on a.ParadaInicio equals f.Idparada
                              join g in _context.Parada on a.ParadaFin equals g.Idparada
                              join h in _context.Tipovehiculos on d.TipovehiculoIdtipovehiculo equals h.Idtipovehiculo
                              join i in _context.Transaccions on a.TransaccionIdtransaccion equals i.Idtransaccion
                              join j in _context.CalificacionConductors on a.Idviaje equals j.ViajeIdviaje into calificacion
                              where a.Idviaje == data.ReservacionId
                              select new BoletoViajeResponse
                              {
                                Fecha = a.Fechaviaje == null ? "" : a.Fechaviaje.Value.ToString("dd/MM/yyyy"),
                                Hora = a.Fechaviaje == null ? "" : a.Fechaviaje.Value.ToString("HH:mm"),
                                IdcorridaAsignacion = b.IdcorridaAsignacion,
                                CorridaId = a.CorridaAsignacionIdcorridaAsignacion ?? 0,
                                Pasajeros = a.Numeropasajeros ?? 0,
                                ConductorNombre = e.Nombre,
                                DireccionAbordaje = (f.Calle ?? "") + ("No. " + f.Numero) ?? "" + (", " + f.Colonia) ?? "" + (", " + f.Codigopostal) ?? "" + (", " + f.Municipio) ?? "" + (", " + f.Ciudad) ?? "",
                                DireccionDescenso = (g.Calle ?? "") + ("No. " + g.Numero) ?? "" + (", " + g.Colonia) ?? "" + (", " + g.Codigopostal) ?? "" + (", " + g.Municipio) ?? "" + (", " + g.Ciudad) ?? "",
                                EstacionAbordaje = f.Nombre,
                                EstacionDescenso = g.Nombre,
                                EstacionAbordajeId = f.Idparada,
                                EstacionDescensoId = g.Idparada,
                                HoraAbordaje = c.HoraInicio == null ? "" : c.HoraInicio.Value.ToString("HH:mm"),
                                HoraDescenso = c.HoraFin == null ? "" : c.HoraFin.Value.ToString("HH:mm"),
                                Monto = i.Cantidad ?? 0,
                                Calificado = calificacion.Count() > 0,
                                Calificacion = calificacion.Count() > 0 == null ? 0 : (calificacion.FirstOrDefault().Calificacion ?? 0),
                                MensajeCalificacion = calificacion.Count() > 0 ? calificacion.FirstOrDefault().Mensaje : null,
                                Facturado = a.Facturado ?? false,//factura != null,
                                EstatusViajeId = a.EstatusviajeIdestatusviaje,
                                Cancelable = true,
                                ViajeRedondo = a.ViajeredondoIdviajeredondo == null ? null : (from redondo in _context.Viajeredondos
                                                                                              where redondo.Idviajeredondo == a.ViajeredondoIdviajeredondo
                                                                                              select new Models.Viajes.Viajeredondo
                                                                                              {
                                                                                                origendireccion = redondo.Origendireccion,
                                                                                                origennombre = redondo.Origennombre,
                                                                                                //origenId = redondo.Origenid.Value,
                                                                                                origenlatitud = redondo.Origenlatitud,
                                                                                                origenlongitud = redondo.Origenlongitud,
                                                                                                destinonombre = redondo.Destinonombre,
                                                                                                destinodireccion = redondo.Destinodireccion,
                                                                                                //destinoId = redondo.Destinoid.Value,
                                                                                                destinolatitud = redondo.Destinolatitud,
                                                                                                destinolongitud = redondo.Destinolongitud
                                                                                              }).FirstOrDefault(),
                                Auto = new AutoModel
                                {
                                  Placas = d.Placas,
                                  Color = "Blanco",
                                  Descripcion = h.Nombre
                                },
                                Boleto = new BoletoModel
                                {
                                  Codigo = a.Boleto,
                                  QR = a.Boleto
                                }
                              }).FirstOrDefaultAsync();
      return infoBoleto;
    }


    private async Task<List<BoletoViajeResponse>> ObtenerListInfoBoleto(int userId, DateTime fecha)
    {

      //BoletoModel boleto = new BoletoModel();
      //boleto.Codigo = "VB1253";
      //boleto.QR = "VB1253";

      var infoBoleto = await (from a in _context.Viajes
                              join b in _context.CorridaAsignacions on a.CorridaAsignacionIdcorridaAsignacion equals b.IdcorridaAsignacion
                              join c in _context.Corrida on b.CorridaIdcorrida equals c.Idcorrida
                              join d in _context.Vehiculos on b.VehiculoIdvehiculo equals d.IdVehiculo
                              join e in _context.Conductors on b.ConductorIdconductor equals e.Idconductor
                              join f in _context.Parada on a.ParadaInicio equals f.Idparada
                              join g in _context.Parada on a.ParadaFin equals g.Idparada
                              join h in _context.Tipovehiculos on d.TipovehiculoIdtipovehiculo equals h.Idtipovehiculo
                              join i in _context.Transaccions on a.TransaccionIdtransaccion equals i.Idtransaccion
                              join j in _context.CalificacionConductors on a.Idviaje equals j.ViajeIdviaje into calificacion
                              join k in _context.Estatusviajes on a.EstatusviajeIdestatusviaje equals k.Idestatusviaje
                              where a.UsuarioIdusuario == userId
                                  && k.Idestatusviaje == Convert.ToInt32(EnumEstatusViaje.EN_ESPERA)
                                  && a.Fechaviaje >= fecha
                              select new BoletoViajeResponse
                              {
                                Fecha = a.Fechaviaje == null ? "" : a.Fechaviaje.Value.ToString("dd/MM/yyyy"),
                                Hora = a.Fechaviaje == null ? "" : a.Fechaviaje.Value.ToString("HH:mm"),
                                IdcorridaAsignacion = b.IdcorridaAsignacion,
                                CorridaId = a.CorridaAsignacionIdcorridaAsignacion ?? 0,
                                Pasajeros = a.Numeropasajeros ?? 0,
                                ConductorNombre = e.Nombre,
                                DireccionAbordaje = (f.Calle ?? "") + ("No. " + f.Numero) ?? "" + (", " + f.Colonia) ?? "" + (", " + f.Codigopostal) ?? "" + (", " + f.Municipio) ?? "" + (", " + f.Ciudad) ?? "",
                                DireccionDescenso = (g.Calle ?? "") + ("No. " + g.Numero) ?? "" + (", " + g.Colonia) ?? "" + (", " + g.Codigopostal) ?? "" + (", " + g.Municipio) ?? "" + (", " + g.Ciudad) ?? "",
                                EstacionAbordaje = f.Nombre,
                                EstacionDescenso = g.Nombre,
                                EstacionAbordajeId = f.Idparada,
                                EstacionDescensoId = g.Idparada,
                                HoraAbordaje = c.HoraInicio == null ? "" : c.HoraInicio.Value.ToString("HH:mm"),
                                HoraDescenso = c.HoraFin == null ? "" : c.HoraFin.Value.ToString("HH:mm"),
                                Monto = i.Cantidad ?? 0,
                                Calificado = calificacion.Count() > 0,
                                Calificacion = calificacion.Count() > 0 == null ? 0 : (calificacion.FirstOrDefault().Calificacion ?? 0),
                                MensajeCalificacion = calificacion.Count() > 0 ? calificacion.FirstOrDefault().Mensaje : null,
                                Facturado = a.Facturado ?? false,//factura != null,
                                EstatusViajeId = a.EstatusviajeIdestatusviaje,
                                Cancelable = true,
                                ViajeRedondo = a.ViajeredondoIdviajeredondo == null ? null : (from redondo in _context.Viajeredondos
                                                                                              where redondo.Idviajeredondo == a.ViajeredondoIdviajeredondo
                                                                                              select new Models.Viajes.Viajeredondo
                                                                                              {
                                                                                                origendireccion = redondo.Origendireccion,
                                                                                                origennombre = redondo.Origennombre,
                                                                                                //origenId = redondo.Origenid.Value,
                                                                                                origenlatitud = redondo.Origenlatitud,
                                                                                                origenlongitud = redondo.Origenlongitud,
                                                                                                destinonombre = redondo.Destinonombre,
                                                                                                destinodireccion = redondo.Destinodireccion,
                                                                                                //destinoId = redondo.Destinoid.Value,
                                                                                                destinolatitud = redondo.Destinolatitud,
                                                                                                destinolongitud = redondo.Destinolongitud
                                                                                              }).FirstOrDefault(),
                                Auto = new AutoModel
                                {
                                  Placas = d.Placas,
                                  Color = "Blanco",
                                  Descripcion = h.Nombre
                                },
                                Boleto = new BoletoModel
                                {
                                  Codigo = a.Boleto,
                                  QR = a.Boleto
                                }
                              }).Distinct().ToListAsync();
      return infoBoleto;
    }

    /// <summary>
    /// Obtiene la informacion del boleto y proximos viajes
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<SuVanResponse<List<BoletoViajeResponse>>> ObtenBoletoOffline(int userId)
    {
      SuVanResponse<List<BoletoViajeResponse>> response = new();
      DateTime fecha = DateTime.Now.AddHours(-6);

      List<BoletoViajeResponse> list = await ObtenerListInfoBoleto(userId, fecha);


      response.Data = list;
      response.CodigoMensaje = "200";
      response.Mensaje = list.Any() ? "Solicitud exitosa" : "No se encontraron boletos";
      return response;
    }


    public async Task<SuVanResponse<RecorridoViajeResponse>> ObtenRecorrido(RecorridoViajeRequest data)
    {
      SuVanResponse<RecorridoViajeResponse> response = new();
      List<EstacionModel> estaciones = new List<EstacionModel>();
      string GoogleMapsRuta = "";


      using (var ctx = _context)
      {
        var paradas = (from a in ctx.Viajes
                       join b in ctx.CorridaAsignacions on a.CorridaAsignacionIdcorridaAsignacion equals b.IdcorridaAsignacion
                       join c in ctx.Corrida on b.CorridaIdcorrida equals c.Idcorrida
                       join d in ctx.CorridaParada on c.Idcorrida equals d.CorridaIdcorrida
                       join e in ctx.Parada on d.ParadaIdparada equals e.Idparada
                       join f in ctx.RutaParada on e.Idparada equals f.ParadaIdparada
                       join g in ctx.Ruta on c.RutaIdruta equals g.Idruta
                       where a.Idviaje == data.ReservacionId
                       && c.RutaIdruta == f.RutaIdruta
                       && e.Idparada == f.ParadaIdparada
                       select new
                       {
                         e.Idparada,
                         f.RutaIdruta,
                         e.Nombre,
                         e.Latitud,
                         e.Longitud,
                         f.Orden,
                         g.Googlemapsruta

                       }).Distinct().OrderBy(x => x.Orden).ToList();


        Viaje? viaje = ctx.Viajes.SingleOrDefault(x => x.Idviaje == data.ReservacionId);


        int inicio = 999;
        int fin = 999;

        foreach (var item in paradas)
        {
          GoogleMapsRuta = item.Googlemapsruta.ToString();

          EstacionModel estacion = new EstacionModel();
          //estacion.IdEstacion = item.Idparada;
          estacion.Nombre = item.Nombre;
          estacion.Latitud = item.Latitud;
          estacion.Longitud = item.Longitud;

          if (viaje != null)
          {
            if (viaje.ParadaInicio == item.Idparada)
            {
              inicio = item.Orden ?? 0;
              estacion.ViajeUsuario = 1;
            }
            else if (viaje.ParadaFin == item.Idparada)
            {
              fin = item.Orden ?? 0;
              estacion.ViajeUsuario = 2;
            }
            else
            {
              if (item.Orden.Value > inicio && item.Orden.Value < fin)
              {
                estacion.ViajeUsuario = 3;
              }
              else
              {
                estacion.ViajeUsuario = 0;
              }
            }
          }


          estaciones.Add(estacion);

          var puntisVirtuales = ctx.Puntovirtuals.Where(x => x.RutaParadaRutaIdruta == item.RutaIdruta && x.RutaParadaParadaIdparada == item.Idparada).OrderBy(x => x.Orden).ToList();
          int contador = 1;
          if (puntisVirtuales.Count() > 0)
          {
            foreach (var item2 in puntisVirtuales)
            {
              if (contador != 1)
              {
                EstacionModel puntoVirtual = new EstacionModel();
                puntoVirtual.Nombre = String.Empty;
                puntoVirtual.Latitud = item2.Latitud;
                puntoVirtual.Longitud = item2.Longitud;

                if (estacion.ViajeUsuario != 0 && estacion.ViajeUsuario != 2)
                {
                  puntoVirtual.ViajeUsuario = 4;
                }

                estaciones.Add(puntoVirtual);
              }

              contador++;
            }
          }
        }

      }

      response.Mensaje = String.Empty;
      if (estaciones.Count() == 0)
      {
        response.Mensaje = "No se encontró recorrido";
      }
      response.Data = new RecorridoViajeResponse { GoogleMapsRuta = GoogleMapsRuta, Estaciones = estaciones };
      response.CodigoMensaje = "200";
      return response;
    }

    public async Task<SuVanResponse<List<ViajeRutaModels>>> ViajesFrecuentes(int userId)
    {
      SuVanResponse<List<ViajeRutaModels>> response = new();
      SuVanResponse<List<ViajeRutaModels>> r2 = new();

      using (var ctx = _context)
      {
        var list = (from a in ctx.Viajes
                    join b in ctx.Parada on a.ParadaInicio equals b.Idparada
                    join c in ctx.Parada on a.ParadaFin equals c.Idparada
                    join e in ctx.CorridaAsignacions on a.CorridaAsignacionIdcorridaAsignacion equals e.IdcorridaAsignacion
                    join h in ctx.Corrida on e.CorridaIdcorrida equals h.Idcorrida
                    join f in ctx.Ruta on h.RutaIdruta equals f.Idruta
                    join d in ctx.Transaccions on a.TransaccionIdtransaccion equals d.Idtransaccion
                    join g in ctx.Estatusviajes on a.EstatusviajeIdestatusviaje equals g.Idestatusviaje
                    where a.UsuarioIdusuario == userId && g.Idestatusviaje == Convert.ToInt32(EnumEstatusViaje.FINALIZADO) && a.ViajeredondoIdviajeredondo != null
                    select new ViajeRutaModels()
                    {
                      ViajeRelacion = a.ViajeredondoIdviajeredondo.Value,
                      ReservacionId = a.Idviaje,
                      RutaId = f.Idruta,
                      RutaNombre = f.Nombre,
                      GoogleMapsRuta = f.Googlemapsruta,
                      EstacionAbordaje = (b.Calle ?? "") + ((" No. " + b.Numero) ?? "") + ((", " + b.Colonia) ?? "") + ((", " + b.Codigopostal) ?? "") + ((", " + b.Municipio) ?? "") + ((", " + b.Ciudad) ?? ""),
                      EstacionDescenso = (c.Calle ?? "") + ((" No. " + c.Numero) ?? "") + ((", " + c.Colonia) ?? "") + ((", " + c.Codigopostal) ?? "") + ((", " + c.Municipio) ?? "") + ((", " + c.Ciudad) ?? ""),
                      EstacionAbordajeId = b.Idparada,
                      EstacionDescensoId = c.Idparada,
                      Costo = d.Cantidad.ToString(),
                      LatitudInicial = b.Latitud ?? 0,
                      LongitudInicial = b.Longitud ?? 0,
                      LatitudFinal = c.Latitud ?? 0,
                      LongitudFinal = c.Longitud ?? 0
                    }).ToList().OrderByDescending(o => o.ReservacionId);

        response.Data = list.GroupBy(v => new
        {
          v.ViajeRelacion,
          v.ReservacionId,
          v.RutaId,
          v.RutaNombre,
          v.GoogleMapsRuta,
          v.EstacionAbordaje,
          v.EstacionDescenso,
          v.EstacionAbordajeId,
          v.EstacionDescensoId,
          v.Costo,
          v.LatitudInicial,
          v.LongitudInicial,
          v.LatitudFinal,
          v.LongitudFinal
        })
            .Select(v => new
            {
              v.Key.ViajeRelacion,
              v.Key.ReservacionId,
              v.Key.RutaId,
              v.Key.RutaNombre,
              v.Key.GoogleMapsRuta,
              v.Key.EstacionAbordaje,
              v.Key.EstacionDescenso,
              v.Key.EstacionAbordajeId,
              v.Key.EstacionDescensoId,
              v.Key.Costo,
              v.Key.LatitudInicial,
              v.Key.LongitudInicial,
              v.Key.LatitudFinal,
              v.Key.LongitudFinal,
              Count = v.Count()
            })
            .OrderByDescending(v => v.Count)
            .ToList()
            .Select(v => new ViajeRutaModels()
            {
              ViajeRelacion = v.ViajeRelacion,
              ReservacionId = v.ReservacionId,
              RutaId = v.RutaId,
              RutaNombre = v.RutaNombre,
              GoogleMapsRuta = v.GoogleMapsRuta,
              EstacionAbordaje = v.EstacionAbordaje,
              EstacionDescenso = v.EstacionDescenso,
              EstacionAbordajeId = v.EstacionAbordajeId,
              EstacionDescensoId = v.EstacionDescensoId,
              Costo = v.Costo,
              LatitudInicial = v.LatitudInicial,
              LongitudInicial = v.LongitudInicial,
              LatitudFinal = v.LatitudFinal,
              LongitudFinal = v.LongitudFinal
            })
            .Distinct()
            .Take(2)
            .ToList();




        r2 = response;


        for (int i = 0; i < response.Data.Count; i++)
        {

          int? id = r2.Data[i].ViajeRelacion;

          var a = (from redondo in ctx.Viajeredondos
                   where redondo.Idviajeredondo == r2.Data[i].ViajeRelacion
                   select new
                   {
                     origendireccion = redondo.Origendireccion,
                     origennombre = redondo.Origennombre,
                     //origenId = redondo.Origenid.Value,
                     origenlatitud = redondo.Origenlatitud,
                     origenlongitud = redondo.Origenlongitud,
                     destinonombre = redondo.Destinonombre,
                     destinodireccion = redondo.Destinodireccion,
                     //destinoId = redondo.Destinoid.Value,
                     destinolatitud = redondo.Destinolatitud,
                     destinolongitud = redondo.Destinolongitud
                   }).FirstOrDefault();

          if (a != null)
          {
            r2.Data[i].OrigenNombre = a.origennombre;
            r2.Data[i].OrigenDireccion = a.origendireccion;
            r2.Data[i].DestinoNombre = a.destinonombre;
            r2.Data[i].DestinoDireccion = a.destinodireccion;
          }
          else
          {
            r2.Data[i].OrigenDireccion = a.origendireccion;
            r2.Data[i].DestinoDireccion = a.destinodireccion;
          }
        }


      }


      response.CodigoMensaje = "200";
      response.Mensaje = r2.Data.ToArray().Length == 0 ? "No se encontraron viajes frecuentes" : "Búsqueda exitosa";

      return response;
    }


    public async Task<SuVanResponse<List<ViajeRutaResponse>>> ViajeCurso(int userId, int numViajes)
    {

      DateTime fecha = DateTime.Now.Date;


      SuVanResponse<List<ViajeRutaResponse>> response = new();

      using (var ctx = _context)
      {
        var viaje = (from a in ctx.Viajes
                     join b in ctx.Parada on a.ParadaInicio equals b.Idparada
                     join c in ctx.Parada on a.ParadaFin equals c.Idparada
                     join h in ctx.CorridaAsignacions on a.CorridaAsignacionIdcorridaAsignacion equals h.IdcorridaAsignacion
                     join e in ctx.Corrida on h.CorridaIdcorrida equals e.Idcorrida
                     join d in ctx.CorridaParada on e.Idcorrida equals d.CorridaIdcorrida
                     join f in ctx.Ruta on e.RutaIdruta equals f.Idruta
                     where a.UsuarioIdusuario == userId
                         && a.EstatusviajeIdestatusviaje == Convert.ToInt32(EnumEstatusViaje.EN_CURSO)
                         && b.Idparada == d.ParadaIdparada
                         && h.Fecha >= fecha
                     select new ViajeRutaResponse()
                     {
                       CorridaAsignacionId = a.CorridaAsignacionIdcorridaAsignacion.Value,
                       ReservacionId = a.Idviaje,
                       RutaId = f.Idruta,
                       RutaNombre = f.Nombre,
                       GoogleMapsRuta = f.Googlemapsruta,
                       EstacionAbordaje = (b.Calle ?? "") + (("No. " + b.Numero) ?? "") + ((", " + b.Colonia) ?? "") + ((", " + b.Codigopostal) ?? "") + ((", " + b.Municipio) ?? "") + ((", " + b.Ciudad) ?? ""),
                       EstacionDescenso = (c.Calle ?? "") + (("No. " + c.Numero) ?? "") + ((", " + c.Colonia) ?? "") + ((", " + c.Codigopostal) ?? "") + ((", " + c.Municipio) ?? "") + ((", " + c.Ciudad) ?? ""),
                       EstacionAbordajeId = b.Idparada,
                       EstacionDescensoId = c.Idparada,
                       FechaAbordaje = a.Fechaviaje.Value.ToString("dd/MM/yyyy"),
                       HoraAbordaje = d.Hora.Value.ToString("HH:mm"),
                       LatitudInicial = b.Latitud ?? 0,
                       LongitudInicial = b.Longitud ?? 0,
                       LatitudFinal = c.Latitud ?? 0,
                       LongitudFinal = c.Longitud ?? 0
                     }).Distinct().ToList();
        if (numViajes == 0)
          response.Data = viaje;
        else
          response.Data = viaje;
      }
      response.CodigoMensaje = "200";
      response.Mensaje = response.Data == null ? "No se encontró viaje en curso" : "Búsqueda exitosa";

      return response;
    }

    public async Task<SuVanResponse<List<ViajeRutaResponse>>> ProximosViajes(int userId, int numViajes)
    {
      SuVanResponse<List<ViajeRutaResponse>> response = new();

      DateTime fecha = DateTime.Now.AddHours(-6);


      List<ViajeRutaResponse> list = await ObtenInformacionProximosViajes(userId, fecha);

      if (numViajes == 0)
        response.Data = list;
      else
        response.Data = list.Take(numViajes).ToList();

      response.CodigoMensaje = "200";
      response.Mensaje = response.Data.ToArray().Length == 0 ? "No se encontraron proximos viajes" : "Búsqueda exitosa";

      return response;
    }

    private async Task<List<ViajeRutaResponse>> ObtenInformacionProximosViajes(int userId, DateTime fecha)
    {
      using (var ctx = _context)
      {
        return await (from a in _context.Viajes
                      join b in ctx.Parada on a.ParadaInicio equals b.Idparada
                      join c in ctx.Parada on a.ParadaFin equals c.Idparada
                      join h in ctx.CorridaAsignacions on a.CorridaAsignacionIdcorridaAsignacion equals h.IdcorridaAsignacion
                      join e in ctx.Corrida on h.CorridaIdcorrida equals e.Idcorrida
                      join d in ctx.CorridaParada on new { X1 = e.Idcorrida, X2 = b.Idparada } equals new { X1 = d.CorridaIdcorrida, X2 = d.ParadaIdparada }
                      join f in ctx.Ruta on e.RutaIdruta equals f.Idruta
                      join g in ctx.Estatusviajes on a.EstatusviajeIdestatusviaje equals g.Idestatusviaje
                      where a.UsuarioIdusuario == userId
                          && g.Idestatusviaje == Convert.ToInt32(EnumEstatusViaje.EN_ESPERA)
                          && b.Idparada == d.ParadaIdparada
                          && a.Fechaviaje >= fecha
                      select new ViajeRutaResponse()
                      {
                        CorridaAsignacionId = a.CorridaAsignacionIdcorridaAsignacion.Value,
                        ReservacionId = a.Idviaje,
                        RutaId = f.Idruta,
                        RutaNombre = f.Nombre,
                        GoogleMapsRuta = f.Googlemapsruta,
                        EstacionAbordaje = (b.Calle ?? "") + (("No. " + b.Numero) ?? "") + ((", " + b.Colonia) ?? "") + ((", " + b.Codigopostal) ?? "") + ((", " + b.Municipio) ?? "") + ((", " + b.Ciudad) ?? ""),
                        EstacionDescenso = (c.Calle ?? "") + (("No. " + c.Numero) ?? "") + ((", " + c.Colonia) ?? "") + ((", " + c.Codigopostal) ?? "") + ((", " + c.Municipio) ?? "") + ((", " + c.Ciudad) ?? ""),
                        EstacionAbordajeId = b.Idparada,
                        EstacionDescensoId = c.Idparada,
                        FechaAbordaje = a.Fechaviaje.Value.ToString("dd/MM/yyyy"),
                        HoraAbordaje = d.Hora.Value.ToString("HH:mm"),
                        LatitudInicial = b.Latitud ?? 0,
                        LongitudInicial = b.Longitud ?? 0,
                        LatitudFinal = c.Latitud ?? 0,
                        LongitudFinal = c.Longitud ?? 0,
                        EstatusViajeId = a.EstatusviajeIdestatusviaje
                      }).Distinct().ToListAsync();
      }

    }

    public async Task<SuVanResponse<List<ViajeRutaResponse>>> ViajesAnteriores(int userId)
    {
      SuVanResponse<List<ViajeRutaResponse>> response = new();

      var status = new string[] { "Perdido", "En curso", "Finalizado" };

      using (var ctx = _context)
      {
        var list = (from a in ctx.Viajes
                    join b in ctx.Parada on a.ParadaInicio equals b.Idparada
                    join c in ctx.Parada on a.ParadaFin equals c.Idparada
                    join e in ctx.CorridaAsignacions on a.CorridaAsignacionIdcorridaAsignacion equals e.IdcorridaAsignacion
                    join h in ctx.Corrida on e.CorridaIdcorrida equals h.Idcorrida
                    join d in ctx.CorridaParada on new { X = b.Idparada, Y = e.CorridaIdcorrida } equals new { X = d.ParadaIdparada, Y = d.CorridaIdcorrida }
                    join f in ctx.Ruta on h.RutaIdruta equals f.Idruta
                    join g in ctx.Estatusviajes on a.EstatusviajeIdestatusviaje equals g.Idestatusviaje
                    where a.UsuarioIdusuario == userId
                        && status.Contains(g.Estatusviajecol)
                    select new ViajeRutaResponse()
                    {
                      CorridaAsignacionId = a.CorridaAsignacionIdcorridaAsignacion.Value,
                      ReservacionId = a.Idviaje,
                      RutaId = f.Idruta,
                      RutaNombre = f.Nombre,
                      GoogleMapsRuta = f.Googlemapsruta,
                      EstacionAbordaje = (b.Calle ?? "") + (("No. " + b.Numero) ?? "") + ((", " + b.Colonia) ?? "") + ((", " + b.Codigopostal) ?? "") + ((", " + b.Municipio) ?? "") + ((", " + b.Ciudad) ?? ""),
                      EstacionDescenso = (c.Calle ?? "") + (("No. " + c.Numero) ?? "") + ((", " + c.Colonia) ?? "") + ((", " + c.Codigopostal) ?? "") + ((", " + c.Municipio) ?? "") + ((", " + c.Ciudad) ?? ""),
                      EstacionAbordajeId = b.Idparada,
                      EstacionDescensoId = c.Idparada,
                      FechaAbordaje = a.Fechaviaje.Value.ToString("dd/MM/yyyy"),
                      FechaAbordajeDate = a.Fechaviaje.Value,
                      HoraAbordaje = d.Hora.Value.ToString("HH:mm"),
                      LatitudInicial = b.Latitud ?? 0,
                      LongitudInicial = b.Longitud ?? 0,
                      LatitudFinal = c.Latitud ?? 0,
                      LongitudFinal = c.Longitud ?? 0
                    }).Distinct().ToList().OrderByDescending(o => o.FechaAbordajeDate).ThenByDescending(o => o.HoraAbordaje).ToList();


        response.Data = list;
      }
      response.CodigoMensaje = "200";
      response.Mensaje = response.Data.ToArray().Length == 0 ? "No se encontraron viajes anteriores" : "Búsqueda exitosa";

      return response;
    }




    public async Task<SuVanResponse<List<ViajesServicioFechaResponse>>> BuscaFechasRuta(ViajeFechasRequest data)
    {

      DateTime fechaConsulta = DateTime.Now.Date;

      List<ViajesServicioFechaResponse> listaViajes = new List<ViajesServicioFechaResponse>();
      SuVanResponse<List<ViajesServicioFechaResponse>> response = new();


      // obtiene las corridas que son de la fecha de consulta y que no esten finalizadas
      var corridasResult = await (from ruta in _context.Ruta
                                  join corrida in _context.Corrida on ruta.Idruta equals corrida.RutaIdruta
                                  join corridaAsignacion in _context.CorridaAsignacions on corrida.Idcorrida equals corridaAsignacion.CorridaIdcorrida
                                  join vehiculo in _context.Vehiculos on corridaAsignacion.VehiculoIdvehiculo equals vehiculo.IdVehiculo
                                  join tipoVehiculo in _context.Tipovehiculos on vehiculo.TipovehiculoIdtipovehiculo equals tipoVehiculo.Idtipovehiculo
                                  where ruta.Idruta == data.RutaId
                                  && corridaAsignacion.Fecha >= fechaConsulta
                                  && (corridaAsignacion.EstatusviajeIdestatusviaje == Convert.ToInt32(EnumEstatusViaje.EN_CURSO)
                                  || corridaAsignacion.EstatusviajeIdestatusviaje == Convert.ToInt32(EnumEstatusViaje.EN_ESPERA))
                                  orderby corridaAsignacion.Fecha ascending
                                  select new { ruta, corridaAsignacion, tipoVehiculo }).ToListAsync();

      var rutaParadasResult = await (from a in _context.RutaParada
                                     where a.RutaIdruta == data.RutaId
                                     && a.Activo == Convert.ToUInt64(true)
                                     select a).OrderBy(x => x.Orden).ToListAsync();


      foreach (var corrida in corridasResult)
      {
        ViajesServicioFechaResponse fechas = new ViajesServicioFechaResponse();

        int asientosPorVehiculo = corrida.tipoVehiculo.Asientos ?? 0;
        int asientosDisponibles = 0;
        if (corrida.ruta.TipotarifaIdtipotarifa == Convert.ToInt32(EnumTipoTarifa.FIJA))
        {
          asientosDisponibles = await (from a in _context.CorridaAsignacionParada
                                       where a.CorridaAsignacionIdcorridaAsignacion == corrida.corridaAsignacion.IdcorridaAsignacion
                                       select a).MinAsync(x => x.Espacios) ?? 0;
        }
        else
        {
          var estacionesIntermedias = await ObtenEstacionesIntermedias(rutaParadasResult, data.EstacionAbordajeId, data.EstacionDescensoId);

          asientosDisponibles = await (from a in _context.CorridaAsignacionParada
                                       where a.CorridaAsignacionIdcorridaAsignacion == corrida.corridaAsignacion.IdcorridaAsignacion
                                       && estacionesIntermedias.Contains(a.ParadaIdparada)
                                       select a).MinAsync(x => x.Espacios) ?? 0;
        }


        DateTime hoy = DateTime.Now;

        var reservados = await (from v in _context.Viajes
                                where v.CorridaAsignacionIdcorridaAsignacion == corrida.corridaAsignacion.IdcorridaAsignacion
                                && v.EstatusviajeIdestatusviaje == 0 && v.Vigenciareserva > hoy
                                select v.Numeropasajeros).CountAsync();

        if ((asientosDisponibles - reservados) < 1) { continue; }

        var estacionAbordajeResult = await (from corridaParada in _context.CorridaParada
                                            where corridaParada.ParadaIdparada == data.EstacionAbordajeId
                                            && corridaParada.CorridaIdcorrida == corrida.corridaAsignacion.CorridaIdcorrida
                                            select corridaParada).Include(x => x.ParadaIdparadaNavigation).FirstOrDefaultAsync();


        var viajeFecha = listaViajes.Where(x => x.FechaViaje == corrida.corridaAsignacion.Fecha && estacionAbordajeResult.Hora > TimeOnly.FromDateTime(DateTime.Now)).FirstOrDefault();

                if (viajeFecha != null)
        {
          if (viajeFecha.FechaViaje == corrida.corridaAsignacion.Fecha)
          {
            listaViajes.Remove(viajeFecha);
          }
          else
          {
            continue;
          }
        }
        if (fechaConsulta == corrida.corridaAsignacion.Fecha)
        {
          if (estacionAbordajeResult.Hora > TimeOnly.FromDateTime(DateTime.Now))
          {
            fechas.FechaViaje = corrida.corridaAsignacion.Fecha;
            listaViajes.Add(fechas);
          }
        }
        else
        {
          fechas.FechaViaje = corrida.corridaAsignacion.Fecha;
          listaViajes.Add(fechas);
        }

      }

      response.Data = listaViajes;
      response.CodigoMensaje = "200";
      if (listaViajes.Count == 0)
      {
        response.CodigoMensaje = "400";
      }
      response.Mensaje = response.Data.ToArray().Length == 0 ? "No se encontro disponibilidad" : "Solicitud exitosa";

      return response;

    }

    public async Task<ObtenDatosDeViajePorIdDeVajeModel> ObtenRutaViajePorId(int ViajeId)
    {
      int RutaId = 0;
      //Validamos si existe Información para obtener la ruta
      var InformacionResult = await (from v in _context.Viajes
                                     join ca in _context.CorridaAsignacions on v.CorridaAsignacionIdcorridaAsignacion equals ca.IdcorridaAsignacion
                                     join c in _context.Corrida on ca.CorridaIdcorrida equals c.Idcorrida
                                     join r in _context.Ruta on c.RutaIdruta equals r.Idruta
                                     join rpabordaje in _context.RutaParada on r.Idruta equals rpabordaje.RutaIdruta
                                     join pabordaje in _context.Parada on rpabordaje.ParadaIdparada equals pabordaje.Idparada
                                     join rpdescenso in _context.RutaParada on r.Idruta equals rpdescenso.RutaIdruta
                                     join pdescenso in _context.Parada on rpabordaje.ParadaIdparada equals pdescenso.Idparada
                                     where v.Idviaje == ViajeId
                                     && v.EstatusviajeIdestatusviaje == Convert.ToInt32(EnumEstatusViaje.RESERVANDO) //En reservacion
                                     && r.Activo == 1
                                     && v.EmpresaIdempresa == c.EmpresaIdempresa
                                     && v.EmpresaIdempresa == r.EmpresaIdempresa
                                     ////abordaje
                                     //&& v.ParadaInicio == pabordaje.Idparada
                                     //&& pabordaje.Activo == 1
                                     ////descenso
                                     //&& v.ParadaFin == pdescenso.Idparada
                                     //&& pdescenso.Activo == 1
                                     //&& rpabordaje.Activo ==1
                                     //&& rpdescenso.Activo==1
                                     select new ObtenDatosDeViajePorIdDeVajeModel
                                     {
                                       CorridaAsignacionId = ca.IdcorridaAsignacion,
                                       CorridaId = c.Idcorrida,
                                       RutaId = c.RutaIdruta,
                                       EmpresaId = (int)r.EmpresaIdempresa,
                                       TipoTarifaId = (int)r.TipotarifaIdtipotarifa,
                                       ParadaInicio = v.ParadaInicio,
                                       ParadaFin = v.ParadaFin
                                     }).FirstOrDefaultAsync();

      if (InformacionResult == null)
      {
        return null;
      }
      else
      {
        return InformacionResult;
      }
    }

    public async Task<Viaje> ObtenInfoViaje(int ViajeId)
    {
      var Viaje = await _context.Viajes.Where(x => x.Idviaje == ViajeId).FirstOrDefaultAsync();

      return Viaje!;
    }

    public async Task<SuVanResponse<RecorridoViajeResponse>> ObtenRecorridoOperador(RecorridoViajeOperadorRequest data)
    {
      SuVanResponse<RecorridoViajeResponse> response = new();
      List<EstacionModel> estaciones = new List<EstacionModel>();
      string GoogleMapsRuta = "";


      using (var ctx = _context)
      {
        var paradas = await (from a in ctx.CorridaAsignacions
                             join b in ctx.Corrida on a.CorridaIdcorrida equals b.Idcorrida
                             join c in ctx.CorridaParada on b.Idcorrida equals c.CorridaIdcorrida
                             join d in ctx.Parada on c.ParadaIdparada equals d.Idparada
                             join e in ctx.RutaParada on d.Idparada equals e.ParadaIdparada
                             join f in ctx.Ruta on b.RutaIdruta equals f.Idruta
                             where a.IdcorridaAsignacion == data.CorridaAsignacionId
                             select new
                             {
                               d.Idparada,
                               e.RutaIdruta,
                               d.Nombre,
                               d.Latitud,
                               d.Longitud,
                               e.Orden,
                               f.Googlemapsruta
                             }).Distinct().OrderBy(x => x.Orden).ToListAsync();

        foreach (var item in paradas)
        {
          GoogleMapsRuta = item.Googlemapsruta.ToString();

          EstacionModel estacion = new EstacionModel();
          estacion.Nombre = item.Nombre;
          estacion.Latitud = item.Latitud;
          estacion.Longitud = item.Longitud;
          estaciones.Add(estacion);

          var puntisVirtuales = await ctx.Puntovirtuals.Where(x => x.RutaParadaRutaIdruta == item.RutaIdruta && x.RutaParadaParadaIdparada == item.Idparada).OrderBy(x => x.Orden).ToListAsync();
          int contador = 1;
          if (puntisVirtuales.Count() > 0)
          {
            foreach (var item2 in puntisVirtuales)
            {
              if (contador != 1)
              {
                EstacionModel puntoVirtual = new EstacionModel();
                puntoVirtual.Nombre = String.Empty;
                puntoVirtual.Latitud = item2.Latitud;
                puntoVirtual.Longitud = item2.Longitud;
                estaciones.Add(puntoVirtual);
              }
              contador++;
            }
          }
        }

      }

      response.Mensaje = String.Empty;
      response.CodigoMensaje = "200";

      if (estaciones.Count() == 0)
      {
        response.CodigoMensaje = "400";
        response.Mensaje = "No se encontró recorrido";
      }
      response.Data = new RecorridoViajeResponse { GoogleMapsRuta = GoogleMapsRuta, Estaciones = estaciones };
      return response;
    }


    public async Task<SuVanResponse<CancelacionViaje>> Cancelacion(int userId, CancelaViajeRequest data)
    {
      decimal SaldoFinal = 0;
      SuVanResponse<CancelacionViaje> response = new();
      CancelacionViaje? result = new CancelacionViaje();

      var InformacionResult = await (from v in _context.Viajes
                                     join t in _context.Transaccions on v.TransaccionIdtransaccion equals t.Idtransaccion
                                     where v.Idviaje == data.ReservacionId
                                     && v.EstatusviajeIdestatusviaje != (sbyte)(EnumEstatusViaje.CANCELADO)
                                     select new
                                     {
                                       v.EstatusviajeIdestatusviaje,
                                       t.TipotransaccionIdtipotransaccion,
                                       t.EstatustransaccionIdestatustransaccion,
                                       t.Cantidad
                                     }).FirstOrDefaultAsync();

      if (InformacionResult == null)
      {
        response.CodigoMensaje = "400";
        response.Mensaje = "No se encontro información";
        return response;
      }


      if (InformacionResult.EstatusviajeIdestatusviaje != 1)
      {
        response.CodigoMensaje = "400";
        response.Mensaje = "El viaje ya no puede cancelarse";
        return response;

      }


      Viaje? ViajeEntity = await _context.Viajes.FirstOrDefaultAsync(x => x.Idviaje == data.ReservacionId);

      // valida que la corrida a la que pertenece el viaje no este finalizada
      var corridaAsignacion = await _context.CorridaAsignacions
       .Include(x => x.CorridaIdcorridaNavigation)
       .FirstOrDefaultAsync(x => x.IdcorridaAsignacion == ViajeEntity!.CorridaAsignacionIdcorridaAsignacion);

      if (corridaAsignacion!.EstatusviajeIdestatusviaje == (int)EnumEstatusViaje.FINALIZADO)
      {

        ViajeEntity!.EstatusviajeIdestatusviaje = (sbyte)(EnumEstatusViaje.PERDIDO);
        _context.SaveChanges();

        await _pagoService.ActualizaOcupacion(ViajeEntity.Idviaje, true);

        response.CodigoMensaje = "400";
        response.Mensaje = "El viaje no se puede cancelar por que ya finalizo la corrida";
        return response;
      }



      // valida si la fecha actual es mayor a la fecha del viaje, si se requiere cancelar el viaje vencido se marca como perdido
      if (ViajeEntity != null && DateTime.Now > ViajeEntity.Fechaviaje)
      {

        ViajeEntity.EstatusviajeIdestatusviaje = (sbyte)(EnumEstatusViaje.PERDIDO);
        _context.SaveChanges();

        await _pagoService.ActualizaOcupacion(ViajeEntity.Idviaje, true);

        response.CodigoMensaje = "400";
        response.Mensaje = "El viaje ya no puede cancelarse";
        return response;

      }


      //Actualizamos estatus del Viaje
      if (ViajeEntity != null)
      {
        ViajeEntity.EstatusviajeIdestatusviaje = (sbyte)(EnumEstatusViaje.CANCELADO);
        _context.SaveChanges();

        await _pagoService.ActualizaOcupacion(ViajeEntity.Idviaje, true);
      }

      //Validamos si el Viaje esta Pagado el saldo lo Asignamos al monedero
      if (InformacionResult.EstatustransaccionIdestatustransaccion == Convert.ToInt32(EnumEstatusPago.COMPLETED) || InformacionResult.EstatustransaccionIdestatustransaccion == Convert.ToInt32(EnumEstatusPago.AUTHORIZED))
      {
        #region Actualizamos saldo de Monedero
        //Actualizamos Saldo de Monedero
        SaldoFinal = (decimal)await ActualizaSaldoMonedero(userId, (decimal)InformacionResult.Cantidad, ViajeEntity!);
        #endregion
      }
      else
      {
        //Si no esta pago solo devolvemos el saldo disponible del monedero
        Database.Entities.Monedero? MonederoEntity = await _context.Monederos.Where(x => x.UsuarioIdusuario == userId).FirstOrDefaultAsync();
        if (MonederoEntity != null)
        {
          SaldoFinal = (decimal)MonederoEntity.Saldo;
        }
      }

      #region Guardamos en tabla de Log datos de la camcelacion
      var entity = new Database.Entities.Logcancelacionviaje()
      {
        ViajeIdviaje = data.ReservacionId,
        Saldoabonadaamonedero = SaldoFinal,
        Fecharegistro = DateTime.Now
      };
      _context.Logcancelacionviajes.Add(entity);
      await _context.SaveChangesAsync();
      #endregion


      result.SaldoMonedero = SaldoFinal;
      response.CodigoMensaje = "200";
      response.Mensaje = "Cancelación exitosa";
      response.Data = result;
      return response;
    }

    private async Task<List<int>> ObtenEstacionesIntermedias(List<RutaParadum> paradasRuta, int estacionAbordajeId, int estacionDescensoId)
    {
      int ordenEstacionAbordaje = (from a in paradasRuta
                                   where a.ParadaIdparada == estacionAbordajeId
                                   select a.Orden).FirstOrDefault() ?? 0;

      int ordenEstacionDescenso = (from a in paradasRuta
                                   where a.ParadaIdparada == estacionDescensoId
                                   select a.Orden).FirstOrDefault() ?? 0;

      var rutaParadasEntreEstacionesResult = (from a in paradasRuta
                                              where a.Orden >= ordenEstacionAbordaje
                                              && a.Orden < ordenEstacionDescenso
                                              select a.ParadaIdparada).ToList();

      return rutaParadasEntreEstacionesResult;
    }

    public async Task<decimal?> ActualizaSaldoMonedero(int userId, decimal Cantidad, Viaje viaje)
    {
      decimal SaldoFinal = 0;
      decimal SaldoMonederoDisponible = 0;

      decimal reembolso = await CalculaReembolso(Cantidad, viaje);

      //Actualizamos Saldo
      Database.Entities.Monedero? MonederoEntity = await _context.Monederos.Where(x => x.UsuarioIdusuario == userId).FirstOrDefaultAsync();
      if (MonederoEntity == null)
      {
        SaldoFinal = reembolso;
        //Insertamos Saldo
        var MonederoEntityInsert = new Database.Entities.Monedero()
        {
          UsuarioIdusuario = userId,
          Saldo = SaldoFinal
        };
        _context.Monederos.Add(MonederoEntityInsert);
      }
      else
      {
        SaldoMonederoDisponible = MonederoEntity.Saldo ?? 0;
        SaldoFinal = SaldoMonederoDisponible + reembolso;
        MonederoEntity.Saldo = SaldoFinal;//Actualizamos Saldo del Monedero
        _context.Monederos.Entry(MonederoEntity);
      }
      await _context.SaveChangesAsync();

      await NotificacionCancelacionUsuario(userId, viaje, reembolso);

      return SaldoFinal;
    }

    /// <summary>
    /// notifiacion de cancelacion a usuario
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="viaje"></param>
    /// <param name="reembolso"></param>
    /// <returns></returns>
    private async Task NotificacionCancelacionUsuario(int userId, Viaje viaje, decimal reembolso)
    {
      var usuario = await _context.Usuarios.FirstOrDefaultAsync(x => x.Idusuario == userId);

      if (usuario is not null && usuario.FirebaseId is not null)
      {
        Models.Notification.Data dato = new()
        {
          comando = "casifin_viaje",
          reserva_id = viaje.Idviaje,
          corrida_asignacion_id = (int)viaje.CorridaAsignacionIdcorridaAsignacion!
        };

        await notificacionPushService.EnvioNotificacion(usuario.FirebaseId,
          dato,
          "Viaje Cancelado",
          $"Por la cancelación que realizaste a tu viaje programado, el monto que ha sido reembolsado a tu monedero es {reembolso:C}.",
          string.Empty);
      }
    }

    /// <summary>
    /// calcula el reembolso por cancelacion segun la politica de la empresa
    /// </summary>
    /// <param name="cantidad"></param>
    /// <param name="empresaId"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    private async Task<decimal> CalculaReembolso(decimal cantidad, Viaje viaje)
    {
      decimal reembolso = 0;

      var politicas = await (from x in _context.Politicascompensacions
                             where x.EmpresaIdempresa == viaje.EmpresaIdempresa
                             && x.Tipocancelacion == (int)EnumTipoPoliticaCancelacion.Cliente
                             && x.Tipopolitica == (int)EnumTipoPolitica.Cancelacion
                             select new ViajePoliticaRangoPorcentaje
                             {
                               Porcentaje = (decimal)x.Porcentajecompensacion!,
                               RangoTiempo = x.Tipotiempo == (int)EnumTipoTiempoPoliticaCancelacion.Dias ? ((decimal)x.Rangotiempo! * 24) : (decimal)x.Rangotiempo!,
                             }
                             ).ToListAsync();



      // si no hay politicas de reembolso, se devuelve la cantidad completa
      if (!politicas.Any())
      {
        return cantidad;
      }

      // si hay politicas de reembolso, se calcula el reembolso segun las politicas configuradas restando el porcentaje que aplique segun la fecha actual y la fecha del viaje contra la propiedad de rango tiempo


      var rangoTiempoProvided = (decimal)(DateTime.Now - viaje.Fechaviaje!).Value.TotalHours;

      var porcentajeCompensacion = politicas
           .OrderBy(r => r.RangoTiempo)
           .FirstOrDefault(r => r.RangoTiempo >= rangoTiempoProvided);



      var porcentaje = porcentajeCompensacion ?? politicas.OrderBy(r => r.RangoTiempo).First();


      reembolso = (decimal)(cantidad - (cantidad * porcentaje.Porcentaje / 100)!);



      return reembolso;

    }
  }
}

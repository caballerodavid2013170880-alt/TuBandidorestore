using System;
using Microsoft.EntityFrameworkCore;
using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.Configuracion.Tarifas;
using SUVAN.BackOffice.Models.Facturacion;
using SUVAN.BackOffice.Models.ViewModel.Comercial;
using SUVAN.BackOffice.Models.ViewModel.Configuracion;
using SUVAN.BackOffice.Models.ViewModel.Enums;
using SUVAN.BackOffice.Service.Notificaciones;
using SUVAN.BackOffice.Service.Pago;

namespace SUVAN.BackOffice.Service.Comercial
{
  public class ViajesService : IViajesService
  {
    private readonly SuvanDbContext _context;
    private readonly INotificacionPushService notificacionPushService;

    public ViajesService(SuvanDbContext context,
      INotificacionPushService notificacionPushService
      )
    {
      _context = context;
      this.notificacionPushService = notificacionPushService;
    }

    public async Task<List<ObtenerViajesViewModel>> ObtenerViajes(int empresaId)
    {
      var fecha = DateTime.Now;
      var result = await (from ca in _context.CorridaAsignacions
                          join v in _context.Viajes on ca.IdcorridaAsignacion equals v.CorridaAsignacionIdcorridaAsignacion
                          join c in _context.Conductors on ca.ConductorIdconductor equals c.Idconductor
                          join p in _context.Parada on v.ParadaInicio equals p.Idparada
                          join p2 in _context.Parada on v.ParadaFin equals p2.Idparada
                          join co in _context.Corrida on ca.CorridaIdcorrida equals co.Idcorrida
                          join r in _context.Ruta on co.RutaIdruta equals r.Idruta
                          where !(new int[] { Convert.ToInt32(EnumEstatusViaje.CANCELADO) }).Contains(v.EstatusviajeIdestatusviaje)
                          && !(new int[] { Convert.ToInt32(EnumEstatusViaje.RESERVANDO) }).Contains(v.EstatusviajeIdestatusviaje)
                          && v.EmpresaIdempresa == empresaId
                          && v.Fechaviaje <= fecha
                          group v by new
                          {
                            ca.IdcorridaAsignacion,
                            ca.CorridaIdcorrida,
                            ca.ConductorIdconductor,
                            ca.VehiculoIdvehiculo,
                            ca.Fecha,
                            ca.EstatusviajeIdestatusviaje,
                            c.Nombre,
                            NombreRuta = r.Nombre
                          } into grupo
                          orderby grupo.Key.Fecha descending
                          select new ObtenerViajesViewModel
                          {
                            IdCorridaAsignacion = grupo.Key.IdcorridaAsignacion,
                            CorridaIdCorrida = grupo.Key.CorridaIdcorrida,
                            ConductorIdConductor = grupo.Key.ConductorIdconductor,
                            VehiculoIdVehiculo = grupo.Key.VehiculoIdvehiculo,
                            Fecha = grupo.Min(x => x.Fechaviaje),
                            EstatusViajeIdEstatusViaje = grupo.Key.EstatusviajeIdestatusviaje,
                            ConductorNombre = grupo.Key.Nombre,
                            Nombre = grupo.Key.NombreRuta,
                            TotalPasajeros = grupo.Sum(x => x.Numeropasajeros)
                          }).ToListAsync();
      return result;
    }

    public async Task<List<ObtenerViajesViewModel>> ObtenerViajesProximos(int empresaId)
    {
      var fecha = DateTime.Now;
      var resul = await (from ca in _context.CorridaAsignacions
                         join v in _context.Viajes on ca.IdcorridaAsignacion equals v.CorridaAsignacionIdcorridaAsignacion
                         join c in _context.Conductors on ca.ConductorIdconductor equals c.Idconductor
                         join p in _context.Parada on v.ParadaInicio equals p.Idparada
                         join p2 in _context.Parada on v.ParadaFin equals p2.Idparada
                         join co in _context.Corrida on ca.CorridaIdcorrida equals co.Idcorrida
                         join r in _context.Ruta on co.RutaIdruta equals r.Idruta
                         where (new int[] { Convert.ToInt32(EnumEstatusViaje.EN_ESPERA) }).Contains(v.EstatusviajeIdestatusviaje)
                         && v.EmpresaIdempresa == empresaId
                         && v.Fechaviaje > fecha
                         group v by new
                         {
                           ca.IdcorridaAsignacion,
                           ca.CorridaIdcorrida,
                           ca.ConductorIdconductor,
                           ca.VehiculoIdvehiculo,
                           ca.Fecha,
                           ca.EstatusviajeIdestatusviaje,
                           c.Nombre,
                           NombreRuta = r.Nombre
                         } into grupo
                         orderby grupo.Key.IdcorridaAsignacion descending
                         select new ObtenerViajesViewModel
                         {
                           IdCorridaAsignacion = grupo.Key.IdcorridaAsignacion,
                           CorridaIdCorrida = grupo.Key.CorridaIdcorrida,
                           ConductorIdConductor = grupo.Key.ConductorIdconductor,
                           VehiculoIdVehiculo = grupo.Key.VehiculoIdvehiculo,
                           Fecha = grupo.Min(x => x.Fechaviaje),
                           EstatusViajeIdEstatusViaje = grupo.Key.EstatusviajeIdestatusviaje,
                           ConductorNombre = grupo.Key.Nombre,
                           Nombre = grupo.Key.NombreRuta,
                           TotalPasajeros = grupo.Sum(x => x.Numeropasajeros)
                         }).ToListAsync();
      return resul;
    }

    public async Task<List<ObtenerViajesViewModel>> ObtenerViajesCancelados(int empresaId)
    {
      var resul = await (from ca in _context.CorridaAsignacions
                         join v in _context.Viajes on ca.IdcorridaAsignacion equals v.CorridaAsignacionIdcorridaAsignacion
                         join c in _context.Conductors on ca.ConductorIdconductor equals c.Idconductor
                         join p in _context.Parada on v.ParadaInicio equals p.Idparada
                         join p2 in _context.Parada on v.ParadaFin equals p2.Idparada
                         join co in _context.Corrida on ca.CorridaIdcorrida equals co.Idcorrida
                         join r in _context.Ruta on co.RutaIdruta equals r.Idruta
                         where v.EmpresaIdempresa == empresaId
                         && v.EstatusviajeIdestatusviaje == Convert.ToInt32(EnumEstatusViaje.CANCELADO)
                         group v by new
                         {
                           ca.IdcorridaAsignacion,
                           ca.CorridaIdcorrida,
                           ca.ConductorIdconductor,
                           ca.VehiculoIdvehiculo,
                           ca.Fecha,
                           ca.EstatusviajeIdestatusviaje,
                           c.Nombre,
                           NombreRuta = r.Nombre
                         } into grupo
                         orderby grupo.Key.IdcorridaAsignacion descending
                         select new ObtenerViajesViewModel
                         {
                           IdCorridaAsignacion = grupo.Key.IdcorridaAsignacion,
                           CorridaIdCorrida = grupo.Key.CorridaIdcorrida,
                           ConductorIdConductor = grupo.Key.ConductorIdconductor,
                           VehiculoIdVehiculo = grupo.Key.VehiculoIdvehiculo,
                           Fecha = grupo.Min(x => x.Fechaviaje),
                           EstatusViajeIdEstatusViaje = grupo.Key.EstatusviajeIdestatusviaje,
                           ConductorNombre = grupo.Key.Nombre,
                           Nombre = grupo.Key.NombreRuta,
                           TotalPasajeros = grupo.Sum(x => x.Numeropasajeros)
                         }).ToListAsync();
      return resul;
    }

    public async Task<ObtenerDetalleViajesViewModel> ObtenerDetalleViajes(int? corridaId)
    {
      ObtenerDetalleViajesViewModel model = new();

      model = await (from ca in _context.CorridaAsignacions
                     where ca.IdcorridaAsignacion == corridaId
                     let primeraParada = (from v in _context.Viajes
                                          where v.CorridaAsignacionIdcorridaAsignacion == ca.IdcorridaAsignacion
                                          orderby v.ParadaInicioNavigation
                                          select v.ParadaInicioNavigation.Nombre).FirstOrDefault()
                     let ultimaParada = (from v in _context.Viajes
                                         where v.CorridaAsignacionIdcorridaAsignacion == ca.IdcorridaAsignacion
                                         orderby v.ParadaFinNavigation descending
                                         select v.ParadaFinNavigation.Nombre).FirstOrDefault()
                     select new ObtenerDetalleViajesViewModel()
                     {
                       ViajeId = ca.IdcorridaAsignacion,
                       Viaje = primeraParada + " - " + ultimaParada,
                       Operador = ca.ConductorIdconductorNavigation.Nombre,
                       Ruta = ca.CorridaIdcorridaNavigation.RutaIdrutaNavigation.Nombre,
                       Usuarios = ca.Viajes.Sum(v => v.Numeropasajeros) ?? 0,
                       IdRuta = ca.CorridaIdcorridaNavigation.RutaIdrutaNavigation.Idruta,
                       estatusId = (int)ca.EstatusviajeIdestatusviaje,
                       Fechaviaje = ca.Viajes.Min(v => v.Fechaviaje)
                     }).FirstOrDefaultAsync();

      List<ParadaRutasViewModel> paradaRutasViewModel = new();

      var ruta = await _context.Ruta
      .FirstOrDefaultAsync(x => x.Idruta == model.IdRuta);

      var rutaParadas = await _context.RutaParada
      .Include(x => x.ParadaIdparadaNavigation)
      .Where(rp => rp.RutaIdruta == ruta.Idruta)
      .OrderBy(x => x.Orden)
      .ToListAsync();
      foreach (var rutaParada in rutaParadas)
      {
        ParadaRutasViewModel parada = new()
        {
          RuataIdRuta = rutaParada.ParadaIdparada,
          ParadaNombre = rutaParada.ParadaIdparadaNavigation.Nombre!,
          order = (int)rutaParada.Orden!
        };
        paradaRutasViewModel.Add(parada);

      }
      model.Estaciones = paradaRutasViewModel;

      return model;
    }

    public async Task<List<ObtenerViajesUsuarioViewModel>> ObtenerViajesUsuario(int empresaId)
    {
      var fecha = DateTime.Now;
      var resul = await (from o in _context.Viajes
                         .Include(x => x.ParadaInicioNavigation)
                         .Include(x => x.ParadaFinNavigation)
                         where o.CorridaAsignacionIdcorridaAsignacionNavigation != null
                         && !(new int[] { Convert.ToInt32(EnumEstatusViaje.CANCELADO) }).Contains(o.EstatusviajeIdestatusviaje)
                         && !(new int[] { Convert.ToInt32(EnumEstatusViaje.RESERVANDO) }).Contains(o.EstatusviajeIdestatusviaje)
                         && o.EmpresaIdempresa == empresaId
                         && o.Fechaviaje <= fecha
                         orderby o.Fechaviaje descending
                         select new ObtenerViajesUsuarioViewModel()
                         {
                           ViajeId = o.Idviaje,
                           Viaje = o.ParadaInicioNavigation.Nombre + " - " + o.ParadaFinNavigation.Nombre,
                           Nombre = o.UsuarioIdusuarioNavigation.Nombre,
                           Codigo = o.Boleto,
                           Pasajeros = o.Numeropasajeros,
                           Fechaviaje = o.Fechaviaje
                         }).ToListAsync();
      return resul;
    }

    public async Task<List<ObtenerViajesUsuarioViewModel>> ObtenerViajesProximosUsuario(int empresaId)
    {
      var fecha = DateTime.Now;
      var resul = await (from o in _context.Viajes
                         .Include(x => x.ParadaInicioNavigation)
                         .Include(x => x.ParadaFinNavigation)
                         where o.CorridaAsignacionIdcorridaAsignacionNavigation != null
                         && (new int[] { Convert.ToInt32(EnumEstatusViaje.EN_ESPERA) }).Contains(o.EstatusviajeIdestatusviaje)
                         && o.EmpresaIdempresa == empresaId
                         && o.Fechaviaje > fecha
                         orderby o.Idviaje descending
                         select new ObtenerViajesUsuarioViewModel()
                         {
                           ViajeId = o.Idviaje,
                           Viaje = o.ParadaInicioNavigation.Nombre + " - " + o.ParadaFinNavigation.Nombre,
                           Nombre = o.UsuarioIdusuarioNavigation.Nombre,
                           Codigo = o.Boleto,
                           Pasajeros = o.Numeropasajeros,
                           Fechaviaje = o.Fechaviaje
                         }).ToListAsync();
      return resul;
    }

    public async Task<List<ObtenerViajesUsuarioViewModel>> ObtenerViajesCanceladosUsuario(int empresaId)
    {
      var resul = await (from o in _context.Viajes
                         .Include(x => x.ParadaInicioNavigation)
                         .Include(x => x.ParadaFinNavigation)
                         where o.CorridaAsignacionIdcorridaAsignacionNavigation != null
                         && o.EstatusviajeIdestatusviaje == Convert.ToInt32(EnumEstatusViaje.CANCELADO)
                         && o.EmpresaIdempresa == empresaId
                         orderby o.Idviaje descending
                         select new ObtenerViajesUsuarioViewModel()
                         {
                           ViajeId = o.Idviaje,
                           Viaje = o.ParadaInicioNavigation.Nombre + " - " + o.ParadaFinNavigation.Nombre,
                           Nombre = o.UsuarioIdusuarioNavigation.Nombre,
                           Codigo = o.Boleto,
                           Pasajeros = o.Numeropasajeros,
                           Fechaviaje = o.Fechaviaje
                         }).ToListAsync();
      return resul;
    }

    public async Task<ObtenerDetalleViajesViewModel> ObtenerDetalleViajesUsuario(int? viajeId)
    {
      ObtenerDetalleViajesViewModel model = new();
      model = await (from o in _context.Viajes
                    .Include(x => x.ParadaInicioNavigation)
                    .Include(x => x.ParadaFinNavigation)
                    .Include(x => x.CorridaAsignacionIdcorridaAsignacionNavigation)
                     where o.Idviaje == viajeId
                     select new ObtenerDetalleViajesViewModel()
                     {
                       ViajeId = o.Idviaje,
                       Viaje = o.ParadaInicioNavigation.Nombre + " - " + o.ParadaFinNavigation.Nombre,
                       Operador = o.CorridaAsignacionIdcorridaAsignacionNavigation.ConductorIdconductorNavigation.Nombre,
                       Ruta = o.CorridaAsignacionIdcorridaAsignacionNavigation.CorridaIdcorridaNavigation.RutaIdrutaNavigation.Nombre,
                       Usuarios = o.Numeropasajeros,
                       IdRuta = o.CorridaAsignacionIdcorridaAsignacionNavigation.CorridaIdcorridaNavigation.RutaIdrutaNavigation.Idruta,
                       CorridaAsignacionId = o.CorridaAsignacionIdcorridaAsignacion,
                       estatusId = o.EstatusviajeIdestatusviaje,
                       Fechaviaje = o.Fechaviaje
                     }).FirstAsync();

      List<ParadaRutasViewModel> paradaRutasViewModel = new();

      var ruta = await _context.Ruta
      .FirstOrDefaultAsync(x => x.Idruta == model.IdRuta);

      var rutaParadas = await _context.RutaParada
      .Include(x => x.ParadaIdparadaNavigation)
      .Where(rp => rp.RutaIdruta == ruta.Idruta)
      .OrderBy(x => x.Orden)
      .ToListAsync();
      foreach (var rutaParada in rutaParadas)
      {
        ParadaRutasViewModel parada = new()
        {
          RuataIdRuta = rutaParada.ParadaIdparada,
          ParadaNombre = rutaParada.ParadaIdparadaNavigation.Nombre!,
          order = (int)rutaParada.Orden!
        };
        paradaRutasViewModel.Add(parada);

      }
      model.Estaciones = paradaRutasViewModel;

      return model;
    }

    private async Task ActualizarParadaSubenBajan(Viaje viaje)
    {
      CorridaAsignacionParadum? capInicio = await _context.CorridaAsignacionParada.FirstOrDefaultAsync(x =>
                                                                        x.CorridaAsignacionIdcorridaAsignacion == viaje.CorridaAsignacionIdcorridaAsignacion
                                                                     && x.ParadaIdparada == viaje.ParadaInicio);

      if (capInicio is not null)
      {
        capInicio.Suben -= viaje.Numeropasajeros;
        capInicio.Espacios = capInicio.Espacios + viaje.Numeropasajeros;
        _context.CorridaAsignacionParada.Update(capInicio);
        await _context.SaveChangesAsync();
      }

      CorridaAsignacionParadum? capTermino = await _context.CorridaAsignacionParada.FirstOrDefaultAsync(x =>
                                                      x.CorridaAsignacionIdcorridaAsignacion == viaje.CorridaAsignacionIdcorridaAsignacion
                                                   && x.ParadaIdparada == viaje.ParadaFin);

      if (capTermino is not null)
      {
        capTermino.Bajan -= viaje.Numeropasajeros;

        _context.CorridaAsignacionParada.Update(capTermino);
        await _context.SaveChangesAsync();
      }
    }



    public async Task<string> CancelaViaje(int viajeId, int corridaAsignacionId, int empresaId)
    {
      // decidir si es cancelacion por usuario o por corrida asignacion


      if (viajeId > 0 && corridaAsignacionId == 0)
      {
        // cancela toda la corrida
        await CancelarYNotificarCorrida(viajeId, empresaId);
      }
      else
      {
        // cancela solo un viaje
        await CancelarYNotificarViaje(viajeId, corridaAsignacionId, empresaId);
      }


      //var viaje = _context.Viajes.FirstOrDefault(x => x.Idviaje == viajeId);

      //if (viaje != null)
      //{
      //  viaje.EstatusviajeIdestatusviaje = 5;
      //  _context.Viajes.Update(viaje);
      //  await _context.SaveChangesAsync();
      //}
      ////await _context.SaveChangesAsync();
      //await NotificarViajeCancelado(corridaAsignacionId, empresaId);
      return "El viaje se cancelo correctamente";
    }

    private async Task<bool> CancelarYNotificarViaje(int viajeId, int corridaAsignacionId, int empresaId)
    {
      var viaje = await _context.Viajes
        .FirstOrDefaultAsync(x => x.Idviaje == viajeId);

      if (viaje is null)
        throw new Exception("No se encontro el viaje");

      if (viaje.EstatusviajeIdestatusviaje != (int)EnumEstatusViaje.EN_CURSO && viaje.EstatusviajeIdestatusviaje != (int)EnumEstatusViaje.EN_ESPERA)
        throw new Exception("El viaje no se puede cancelar por que no esta en curso o en espera");

      var corridaAsignacion = await _context.CorridaAsignacions
        .Include(x => x.CorridaIdcorridaNavigation)
        .FirstOrDefaultAsync(x => x.IdcorridaAsignacion == viaje.CorridaAsignacionIdcorridaAsignacion);

      if (corridaAsignacion!.EstatusviajeIdestatusviaje == (int)EnumEstatusViaje.FINALIZADO)
        throw new Exception("El viaje no se puede cancelar por que ya finalizo");

      //validar estacion
      if (corridaAsignacion.IdestacionActual is not null)
      {
        var isValid = await ValidarParaActualVsParadaFinVaije(viaje, corridaAsignacion);
        if (!isValid)
          throw new Exception("El viaje no se puede cancelar por que ya paso la parada actual");

      }

      // actualiza el estatus del viaje
      viaje.EstatusviajeIdestatusviaje = (int)EnumEstatusViaje.CANCELADO;
      _context.Viajes.Update(viaje);
      await _context.SaveChangesAsync();


      await ActualizarParadaSubenBajan(viaje);

      var viajesId = new List<int>
      {
        viaje.Idviaje
      };
      //notifica
      await NotificarViajeCancelado(viajesId, empresaId);


      return true;
    }

    private async Task<bool> ValidarParaActualVsParadaFinVaije(Viaje viaje, CorridaAsignacion corridaAsignacion)
    {
      //valida la estacion del viaje sea mayor igual a la actual de la corrida parada por el orden deruta parada
      var estaciones = await _context.RutaParada
        .Where(x => x.RutaIdruta == corridaAsignacion.CorridaIdcorridaNavigation.RutaIdruta)
        .OrderBy(x => x.Orden)
        .ToListAsync();

      var paradaFin = viaje.ParadaFin;
      var paradaCurrent = corridaAsignacion.IdestacionActual;
      //validar que la parada actual sea menor a la parada fin del viaje
      if (estaciones.FindIndex(x => x.ParadaIdparada == paradaCurrent) >= estaciones.FindIndex(x => x.ParadaIdparada == paradaFin))
        return false;

      return true;
    }

    private async Task<bool> CancelarYNotificarCorrida(int corridaAsignacionId, int empresaId)
    {
      var corridaAsignacion = await _context.CorridaAsignacions
        .Include(x => x.CorridaIdcorridaNavigation)
        .FirstOrDefaultAsync(x => x.IdcorridaAsignacion == corridaAsignacionId);

      if (corridaAsignacion!.EstatusviajeIdestatusviaje == (int)EnumEstatusViaje.FINALIZADO)
        throw new Exception("El viaje no se puede cancelar por que ya finalizo");


      var viajes = await _context.Viajes
        .Where(x => x.CorridaAsignacionIdcorridaAsignacion == corridaAsignacionId
        && (x.EstatusviajeIdestatusviaje == (int)EnumEstatusViaje.EN_ESPERA
        || x.EstatusviajeIdestatusviaje == (int)EnumEstatusViaje.EN_CURSO
        || x.EstatusviajeIdestatusviaje == (int)EnumEstatusViaje.RESERVANDO))
        .ToListAsync();

      var viajesId = new List<int>();
      foreach (var viaje in viajes)
      {
        if (viaje.EstatusviajeIdestatusviaje == (int)EnumEstatusViaje.EN_ESPERA || viaje.EstatusviajeIdestatusviaje == (int)EnumEstatusViaje.EN_CURSO)
        {
          var viajeIsValid = await ValidarParaActualVsParadaFinVaije(viaje, corridaAsignacion);

          if (viajeIsValid)
            viajesId.Add(viaje.Idviaje);
        }

        // actualiza el estatus del viaje
        viaje.EstatusviajeIdestatusviaje = (int)EnumEstatusViaje.CANCELADO;
        _context.Viajes.Update(viaje);
        await _context.SaveChangesAsync();

        await ActualizarParadaSubenBajan(viaje);
      }

      // Actualiza la corrida a cancelad 
      corridaAsignacion.EstatusviajeIdestatusviaje = (int)EnumEstatusViaje.CANCELADO;
      _context.CorridaAsignacions.Update(corridaAsignacion);
      await _context.SaveChangesAsync();

      // notifica a los usuarios del viaje
      await NotificarViajeCancelado(viajesId, empresaId);


      return true;
    }


    /// <summary>
    /// metodo que notifica del viaje cancelado a los usuarios que tenian reservacion
    /// </summary>
    /// <param name="corridaAsignacionId"></param>
    /// <param name="empresaId"></param>
    /// <returns></returns>
    public async Task NotificarViajeCancelado(List<int> viajesId, int empresaId)
    {
      var viajes = await (from c in _context.Viajes
                          where viajesId.Contains(c.Idviaje)
                          select new ViajeCancelacionViewModel()
                          {
                            CorridaAsignacionId = (int)c.CorridaAsignacionIdcorridaAsignacion!,
                            ViajeId = c.Idviaje,
                            TrnasaccionId = (int)(c.TransaccionIdtransaccion ?? 0),
                            UsuarioId = c.UsuarioIdusuario,
                            UsuarioFirebaseId = c.UsuarioIdusuarioNavigation.FirebaseId!,
                            Cantidad = (decimal)(c.TransaccionIdtransaccionNavigation!.Cantidad ?? 0),
                            EstatusTransaccion = c.TransaccionIdtransaccionNavigation!.EstatustransaccionIdestatustransaccion
                          })
                          .ToListAsync();

      foreach (var viaje in viajes)
      {

        if (viaje.UsuarioFirebaseId is not null &&
          (viaje.EstatusTransaccion == Convert.ToInt32(EnumEstatusPago.COMPLETED) || viaje.EstatusTransaccion == Convert.ToInt32(EnumEstatusPago.AUTHORIZED)))
        {
          // hace el calculo del reembolso
          decimal cantidadReembolso = await CalculaReembolso(viaje.Cantidad, empresaId);

          //actualiza monedero

          await updateMonedero(viaje.UsuarioId, cantidadReembolso);

          // guarda transaccion
          Transaccion entity = await GuardaTransaccion(viaje.UsuarioId, 2, 2, cantidadReembolso);

          Models.Notification.Data dato = new()
          {
            comando = "casifin_viaje",
            reserva_id = viaje.ViajeId,
            corrida_asignacion_id = viaje.CorridaAsignacionId
          };

          await notificacionPushService.EnvioNotificacion(viaje.UsuarioFirebaseId,
            dato,
            "Viaje Cancelado",
            $"Lamentamos informarte que tu viaje programado ha sido cancelado, el monto del boleto {cantidadReembolso:C} ha sido reembolsado a tu monedero.",
            string.Empty);
        }

      }
    }

    private async Task<Transaccion> GuardaTransaccion(int userId, int MetodoPagoId, int TipoTransacionId, decimal Monto)
    {
      var entity = new Database.Entities.Transaccion()
      {
        MetodopagoIdmetodopago = MetodoPagoId,
        Terminacion = null,
        Cantidad = Monto,
        TipotransaccionIdtipotransaccion = TipoTransacionId,
        UsuarioIdusuario = userId,
        EstatustransaccionIdestatustransaccion = Convert.ToInt32(EnumEstatusPago.AUTHORIZED),
        Fecharegistro = DateTime.Now,
      };
      _context.Transaccions.Add(entity);
      await _context.SaveChangesAsync();
      return entity;
    }


    /// <summary>
    /// calcula la sumatoria del reembolso
    /// </summary>
    /// <param name="cantidad"></param>
    /// <param name="empresaId"></param>
    /// <returns></returns>
    private async Task<decimal> CalculaReembolso(decimal cantidad, int empresaId)
    {
      decimal reembolso = 0;

      var politicas = await _context.Politicascompensacions
        .Where(x => x.EmpresaIdempresa == empresaId
          && x.Tipocancelacion == (int)EnumTipoPoliticaCancelacion.Empresa
          && x.Tipopolitica == (int)EnumTipoPolitica.Compensacion
          && x.Activa == 1)
        .ToListAsync();

      if (politicas.Any())
      {
        var politica = politicas.FirstOrDefault();

        reembolso = cantidad + (decimal)(cantidad * (politica!.Porcentajecompensacion / 100))!;

        return reembolso;

      }


      return cantidad;
    }

    /// <summary>
    /// Actualiza el monto de nonedero del usuario
    /// </summary>
    /// <param name="usuarioId"></param>
    /// <param name="cantidad"></param>
    /// <returns></returns>
    private async Task updateMonedero(int usuarioId, decimal cantidad)
    {
      var monedero = await _context.Monederos
        .FirstOrDefaultAsync(x => x.UsuarioIdusuario == usuarioId);

      if (monedero != null)
      {
        monedero.Saldo = monedero.Saldo + cantidad;
        _context.Monederos.Update(monedero);
        await _context.SaveChangesAsync();
      }
      else
      {
        monedero = new Database.Entities.Monedero();
        monedero.UsuarioIdusuario = usuarioId;
        monedero.Saldo = cantidad;
        _context.Monederos.Add(monedero);
        await _context.SaveChangesAsync();

      }
    }

  }
}


using Microsoft.EntityFrameworkCore;
using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.Corrida;
using SUVAN.BackOffice.Models.ManejoRespuesta;
using SUVAN.BackOffice.Models.ViewModel.Enums;
using static SUVAN.BackOffice.Models.StoredsProcedures.ModelsStoredsProcedures;
using SUVAN.BackOffice.Service.Notificaciones;


namespace SUVAN.BackOffice.Service.Corrida
{
  public class CorridaService : ICorridaService
  {
    private readonly SuvanDbContext _context;
    private readonly INotificacionPushService _notificacionPushService;
    public CorridaService(SuvanDbContext context, INotificacionPushService pNotificacionPushService)
    {
      _context = context;
      _notificacionPushService = pNotificacionPushService;
    }

    public async Task<SuVanResponse<CorridaEstacionesResponse>> Estaciones(int idCorrida)
    {
      SuVanResponse<CorridaEstacionesResponse> response = new();

      CorridaEstacionesResponse ceResponse = new CorridaEstacionesResponse();
      ceResponse.Idcorrida = idCorrida;

      using (var ctx = _context)
      {
        ceResponse = (from a in ctx.CorridaAsignacions
                      join b in ctx.Corrida on a.CorridaIdcorrida equals b.Idcorrida
                      join c in ctx.Ruta on b.RutaIdruta equals c.Idruta
                      where a.IdcorridaAsignacion == idCorrida
                      select new CorridaEstacionesResponse()
                      {
                        Idcorrida = a.IdcorridaAsignacion,
                        Idruta = c.Idruta,
                        Ruta = c.Nombre,
                        Fecha = a.Fecha,
                        HoraInicio = b.HoraInicio,
                        HoraFin = b.HoraFin,
                        ParadaInicio = (
                          (from a in ctx.RutaParada
                           join b in ctx.Parada on a.ParadaIdparada equals b.Idparada
                           orderby a.Orden
                           select new CorridaEstacion()
                           {
                             Idruta = a.RutaIdruta,
                             Idparada = b.Idparada,
                             Nombre = b.Nombre,
                             Direccion = (b.Calle ?? "") + (("No. " + b.Numero) ?? "") + ((", " + b.Colonia) ?? "") + ((", " + b.Codigopostal) ?? "") + ((", " + b.Municipio) ?? "") + ((", " + b.Ciudad) ?? ""),
                             Longitud = b.Longitud,
                             Latitud = b.Latitud,
                             Orden = a.Orden
                           }).Where(x => x.Idruta == c.Idruta).First()
                        ),
                        ParadaTermino = (
                          (from a in ctx.RutaParada
                           join b in ctx.Parada on a.ParadaIdparada equals b.Idparada
                           orderby a.Orden descending
                           select new CorridaEstacion()
                           {
                             Idruta = a.RutaIdruta,
                             Idparada = b.Idparada,
                             Nombre = b.Nombre,
                             Direccion = (b.Calle ?? "") + (("No. " + b.Numero) ?? "") + ((", " + b.Colonia) ?? "") + ((", " + b.Codigopostal) ?? "") + ((", " + b.Municipio) ?? "") + ((", " + b.Ciudad) ?? ""),
                             Longitud = b.Longitud,
                             Latitud = b.Latitud,
                             Orden = a.Orden
                           }).Where(x => x.Idruta == c.Idruta).First()
                        ),
                        Pasajeros = (
                          (from z in ctx.Viajes
                           join y in _context.Estatusviajes on z.EstatusviajeIdestatusviaje equals y.Idestatusviaje
                           where z.CorridaAsignacionIdcorridaAsignacion == a.IdcorridaAsignacion
                           && (y.Idestatusviaje == Convert.ToInt32(EnumEstatusViaje.EN_ESPERA) ||
                            y.Idestatusviaje == Convert.ToInt32(EnumEstatusViaje.EN_CURSO) ||
                            y.Idestatusviaje == Convert.ToInt32(EnumEstatusViaje.FINALIZADO)
                            || y.Idestatusviaje == Convert.ToInt32(EnumEstatusViaje.PERDIDO)
                            )
                           select z.Numeropasajeros).Sum() ?? 0
                        )
                      }).FirstOrDefault();

        var actual = (from a in ctx.CorridaAsignacions
                      join b in ctx.CorridaAsignacionParada on a.IdcorridaAsignacion equals b.CorridaAsignacionIdcorridaAsignacion
                      join c in ctx.Parada on b.ParadaIdparada equals c.Idparada
                      join d in ctx.Corrida on a.CorridaIdcorrida equals d.Idcorrida
                      join e in ctx.RutaParada on d.RutaIdruta equals e.RutaIdruta
                      where a.IdcorridaAsignacion == idCorrida
                       && (b.EstatusestacionIdestatusestacion == 0 || b.EstatusestacionIdestatusestacion == 1)
                       && b.ParadaIdparada == e.ParadaIdparada
                      orderby e.Orden
                      select new CorridaEstacion()
                      {
                        Idruta = e.RutaIdruta,
                        Idparada = c.Idparada,
                        Nombre = c.Nombre,
                        Direccion = (c.Calle ?? "") + (("No. " + c.Numero) ?? "") + ((", " + c.Colonia) ?? "") + ((", " + c.Codigopostal) ?? "") + ((", " + c.Municipio) ?? "") + ((", " + c.Ciudad) ?? ""),
                        Latitud = c.Latitud ?? 0,
                        Longitud = c.Longitud ?? 0,
                        IdEstatus = b.EstatusestacionIdestatusestacion,
                        Orden = e.Orden,
                        Abordan = 0,
                        Bajan = 0,
                        SiguienteAbordan = 0,
                        SiguienteBajan = 0
                      }).FirstOrDefault();


        if (actual != null)
        {
          CorridaAsignacion ca = _context.CorridaAsignacions.Where(x => x.IdcorridaAsignacion == idCorrida).FirstOrDefault();
          if (ca != null)
          {
            ca.IdestacionActual = actual.Idparada;
            _context.CorridaAsignacions.Update(ca);
            await _context.SaveChangesAsync();
          }
        }



        List<CorridaEstacion> listapuntos = new List<CorridaEstacion>();

        var list = (from a in ctx.CorridaAsignacions
                    join b in ctx.CorridaAsignacionParada on a.IdcorridaAsignacion equals b.CorridaAsignacionIdcorridaAsignacion
                    join c in ctx.Parada on b.ParadaIdparada equals c.Idparada
                    join d in ctx.Corrida on a.CorridaIdcorrida equals d.Idcorrida
                    join e in ctx.RutaParada on d.RutaIdruta equals e.RutaIdruta
                    where a.IdcorridaAsignacion == idCorrida
                       && b.ParadaIdparada == e.ParadaIdparada
                    orderby e.Orden
                    select new CorridaEstacion()
                    {
                      Idruta = e.RutaIdruta,
                      Idparada = c.Idparada,
                      Nombre = c.Nombre,
                      Direccion = (c.Calle ?? "") + (("No. " + c.Numero) ?? "") + ((", " + c.Colonia) ?? "") + ((", " + c.Codigopostal) ?? "") + ((", " + c.Municipio) ?? "") + ((", " + c.Ciudad) ?? ""),
                      Latitud = c.Latitud ?? 0,
                      Longitud = c.Longitud ?? 0,
                      IdEstatus = b.EstatusestacionIdestatusestacion,
                      Orden = e.Orden,
                      EsEstacion = true
                    }).Distinct().OrderBy(x => x.Orden).ToList();


        if (actual != null)
        {
          if (actual.Idparada == ceResponse.ParadaTermino.Idparada)
          {
            actual.Ultima = true;
          }
          else
          {
            actual.Ultima = false;
          }


          var corridaactual = (from z in ctx.CorridaAsignacionParada
                               where z.CorridaAsignacionIdcorridaAsignacion == idCorrida
                               && z.ParadaIdparada == actual.Idparada
                               select new PasajerosSubenBajan()
                               {
                                 Suben = z.Suben ?? 0,
                                 Bajan = z.Bajan ?? 0
                               }
                  ).Distinct().FirstOrDefault();


          actual.SiguienteAbordan = corridaactual.Suben;
          actual.SiguienteBajan = corridaactual.Bajan;



          /*
		  bool SIG = false;
					foreach (CorridaEstacion est in list)
					{

						/*  FUNCION PERO YA NO SE USA
						if (SIG)
						{
							var corridas = (from z in ctx.CorridaAsignacionParada
											where z.CorridaAsignacionIdcorridaAsignacion == idCorrida
											&& z.ParadaIdparada == est.Idparada
											select new PasajerosSubenBajan()
											{
												Suben = z.Suben ?? 0,
												Bajan = z.Bajan ?? 0
											}
							 ).Distinct().FirstOrDefault();


							actual.SiguienteAbordan = corridas.Suben;
							actual.SiguienteBajan = corridas.Bajan;

							SIG = false;
						}



						if (est.Idparada == actual.Idparada)
						{
							SIG = true;
						}
					}
					 */
        }

        ceResponse.Actual = actual;




        foreach (CorridaEstacion est in list)
        {

          listapuntos.Add(est);

          var puntisVirtuales = ctx.Puntovirtuals.Where(x => x.RutaParadaRutaIdruta == est.Idruta && x.RutaParadaParadaIdparada == est.Idparada).OrderBy(x => x.Orden).ToList();

          int contador = 1;
          if (puntisVirtuales.Count() > 0)
          {
            foreach (var item2 in puntisVirtuales)
            {
              if (contador != 1)
              {
                CorridaEstacion puntoVirtual = new CorridaEstacion();
                puntoVirtual.Nombre = String.Empty;
                puntoVirtual.Latitud = item2.Latitud;
                puntoVirtual.Longitud = item2.Longitud;
                puntoVirtual.EsEstacion = false;
                listapuntos.Add(puntoVirtual);
              }
              contador++;
            }
          }

        }




        ceResponse.Estaciones = listapuntos;

      }
      response.Data = ceResponse;
      response.CodigoMensaje = "200";
      response.Mensaje = "Búsqueda exitosa";

      return response;
    }

    public async Task<SuVanResponse<CorridaParadaResponse>> ComenzarAbordaje(ComenzarAbordajeRequest data, int ConductorId)
    {

      SuVanResponse<CorridaParadaResponse> response = new();

      #region Validaciones
      CorridaAsignacion entityCorridaAsignacion = await _context.CorridaAsignacions.FirstOrDefaultAsync(c => c.IdcorridaAsignacion == data.Idcorrida);


      /*
			if (entityCorridaAsignacion != null)
			{
				DateTime FechaAbordar = (DateTime)entityCorridaAsignacion.Fecha;
				string strFechaActual = DateTime.Now.ToString("yyyyMMdd");
				string strFechaAbordar = FechaAbordar.ToString("yyyyMMdd");

				//Validamos que la fecha del abordaje es diferente a la fecha actual
				if (strFechaActual != strFechaAbordar)
				{
					response.CodigoMensaje = "400";
					response.Mensaje = "No es posible comenzar abordaje por fecha fuera del rango configurado.";
					return response;
				}
			}
			else
			{
				response.CodigoMensaje = "400";
				response.Mensaje = "No se encontro información";
				return response;
			}
			*/

      List<CorridaAsignacionParadum> caps = await _context.CorridaAsignacionParada
                                .Where(x => x.CorridaAsignacionIdcorridaAsignacion == data.Idcorrida
                                  && x.EstatusestacionIdestatusestacion == 0).ToListAsync();


      if (caps.Count > 1)
      {
        List<ModelstpRevisaEstacionalaRedonda> lstEstaciones = new List<ModelstpRevisaEstacionalaRedonda>();
        string query = String.Format("CALL stpRevisaEstacionalaRedonda({0},{1},{2},{3},{4});", data.Idcorrida, ConductorId, data.Idparada, data.Latitud, data.Longitud);
        lstEstaciones = _context.Set<ModelstpRevisaEstacionalaRedonda>().FromSqlRaw(query).ToList<ModelstpRevisaEstacionalaRedonda>();

        if (lstEstaciones.Count == 0)
        {
          response.CodigoMensaje = "400";
          response.Mensaje = "No hay estaciones cercanas para comenzar el abordaje.";
          return response;
        }
      }

      #endregion

      CorridaAsignacionParadum cap = await _context.CorridaAsignacionParada.FirstOrDefaultAsync(x => x.CorridaAsignacionIdcorridaAsignacion == data.Idcorrida
                                        && x.ParadaIdparada == data.Idparada);

      if (cap != null)
      {
        cap.EstatusestacionIdestatusestacion = 1;
        _context.CorridaAsignacionParada.Update(cap);
        await _context.SaveChangesAsync();
      }



      CorridaAsignacion ca = await _context.CorridaAsignacions.FirstOrDefaultAsync(x => x.IdcorridaAsignacion == data.Idcorrida);
      if (ca != null)
      {
        if (ca.EstatusviajeIdestatusviaje != null)
        {
          if (ca.EstatusviajeIdestatusviaje != (sbyte)(EnumEstatusViaje.EN_CURSO))
          {
            //ca.IdestacionActual = data.Idparada;
            ca.EstatusviajeIdestatusviaje = (sbyte)(EnumEstatusViaje.EN_CURSO);
            _context.CorridaAsignacions.Update(ca);
            await _context.SaveChangesAsync();
          }
        }
      }



      CorridaParadaResponse responseCP = await LoadCorridaParada(data.Idcorrida, data.Idparada, true, true);


      response.Data = responseCP;
      response.CodigoMensaje = "200";
      response.Mensaje = "Comienzo de abordaje realizado con exito";

      return response;
    }

    public async Task<SuVanResponse<CorridaParadaResponse>> RegresarAbordaje(AbordajeRequest data)
    {
      SuVanResponse<CorridaParadaResponse> response = new();

      CorridaAsignacionParadum cap = await _context.CorridaAsignacionParada.FirstOrDefaultAsync(x => x.CorridaAsignacionIdcorridaAsignacion == data.Idcorrida
                                        && x.ParadaIdparada == data.Idparada);

      if (cap != null)
      {
        cap.EstatusestacionIdestatusestacion = 1;
        _context.CorridaAsignacionParada.Update(cap);
        await _context.SaveChangesAsync();
      }

      CorridaParadaResponse responseCP = await LoadCorridaParada(data.Idcorrida, data.Idparada, true, false);

      response.Data = responseCP;
      response.CodigoMensaje = "200";
      response.Mensaje = "Se regresa el abordaje con exito";

      return response;
    }

    public async Task<SuVanResponse<CorridaParadaResponse>> TerminarAbordaje(AbordajeRequest data)
    {
      SuVanResponse<CorridaParadaResponse> response = new();

      CorridaAsignacionParadum? cap = await _context.CorridaAsignacionParada
                    .FirstOrDefaultAsync(x => x.CorridaAsignacionIdcorridaAsignacion == data.Idcorrida
                      && x.ParadaIdparada == data.Idparada);
      // Se finaliza abordaje de la estación
      if (cap != null)
      {
        cap.EstatusestacionIdestatusestacion = 3;
        _context.CorridaAsignacionParada.Update(cap);
        await _context.SaveChangesAsync();
      }

      CorridaParadaResponse responseCP = await LoadCorridaParada(data.Idcorrida, data.Idparada, true, true);

      var pendientes = await (from a in _context.CorridaAsignacionParada
                              where a.CorridaAsignacionIdcorridaAsignacion == data.Idcorrida
                              &&
                              (
                                a.EstatusestacionIdestatusestacion == Convert.ToInt32(EnumEstatusViaje.RESERVANDO) ||
                                a.EstatusestacionIdestatusestacion == Convert.ToInt32(EnumEstatusViaje.EN_ESPERA) ||
                                a.EstatusestacionIdestatusestacion == Convert.ToInt32(EnumEstatusViaje.EN_CURSO)
                              )
                              select new
                              {
                                a.ParadaIdparada
                              }).ToListAsync();

      // Se finaliza viaje
      if (pendientes.Count == 0)
      {
        CorridaAsignacion? ca = await _context.CorridaAsignacions.FirstOrDefaultAsync(x => x.IdcorridaAsignacion == data.Idcorrida);
        if (ca != null)
        {
          ca.EstatusviajeIdestatusviaje = (sbyte)(EnumEstatusViaje.FINALIZADO);
          _context.CorridaAsignacions.Update(ca);
          await _context.SaveChangesAsync();

          #region Calculo de Comisiones
          // Se realiza calculo de comisiones para el operador
          var vMontoPagado = (from v in _context.Viajes
                              join t in _context.Transaccions on v.TransaccionIdtransaccion equals t.Idtransaccion
                              where v.CorridaAsignacionIdcorridaAsignacion == data.Idcorrida
                                 && t.EstatustransaccionIdestatustransaccion > 0
                              select new { Monto = t.Cantidad ?? 0 }).AsEnumerable().Sum(x => x.Monto);

          var vMtsRuta = (from c in _context.Corrida
                          join r in _context.Ruta on c.RutaIdruta equals r.Idruta
                          where c.Idcorrida == ca.CorridaIdcorrida
                          select r.Distanciamts).FirstOrDefault();

          var vOperador = (from o in _context.Conductors
                           where o.Idconductor == ca.ConductorIdconductor
                           select o).FirstOrDefault();

          var vComision = (vOperador.Comisionfija ?? 0) + (((vOperador.ComisionvariableIngresos ?? 0) / 100) * vMontoPagado) + ((vMtsRuta / 1000) * vOperador.ComisionvariableKm);

          CorridaLiquidacion vCorridaLiq = new CorridaLiquidacion
          {
            IdCorridaAsignacion = data.Idcorrida,
            MontoPagado = vMontoPagado,
            MontoComision = vComision
          };
          _context.CorridaLiquidacions.Add(vCorridaLiq);
          await _context.SaveChangesAsync();
          // Fin calculo de comisiones
          #endregion
        }
      }

      try
      {
        CorridaAsignacion? caNext = await _context.CorridaAsignacions.FirstOrDefaultAsync(x => x.IdcorridaAsignacion == data.Idcorrida);

        var ruta = _context.Corrida.FirstOrDefault(o => o.Idcorrida == caNext.CorridaIdcorrida);
        var parada = _context.RutaParada.Where(z => z.RutaIdruta == ruta.RutaIdruta);
        var ordenActual = parada.FirstOrDefault(o => o.ParadaIdparada == data.Idparada);
        int ordenSiguiente = (ordenActual.Orden ?? 0) + 1;
        var paradaNext = parada.FirstOrDefault(o => o.Orden == ordenSiguiente);

        // Notificaciones
        if (paradaNext != null)
        {
          #region Notificación para usuarios que bajan en la proxima estación
          List<CorridaUsuario> userNext = await GetBajanNext(data.Idcorrida, paradaNext.ParadaIdparada);
          foreach (CorridaUsuario usNe in userNext)
          {
            if (usNe.Status != Convert.ToInt32(EnumEstatusViaje.PERDIDO))
            {
              Models.Notification.Data dato = new()
              {
                comando = "casifin_viaje",
                reserva_id = usNe.Idviaje,
                corrida_asignacion_id = data.Idcorrida
              };

              await _notificacionPushService.EnvioNotificacion(usNe.Firebase, dato, "Prepara tu descenso", "¡Amig@, bajas en la siguiente!", string.Empty);
            }
          }
          #endregion

          #region Notificacion para usuarios que van a subir en la proxima estación
          List<CorridaUsuario> userNextUp = await GetSuben(data.Idcorrida, paradaNext.ParadaIdparada);
          foreach (CorridaUsuario usNeUp in userNextUp)
          {
            Models.Notification.Data dato = new()
            {
              comando = "casi_inicio_viaje",
              reserva_id = usNeUp.Idviaje,
              corrida_asignacion_id = data.Idcorrida
            };

            await _notificacionPushService.EnvioNotificacion(usNeUp.Firebase, dato, "Prepara tu ascenso", "¡Amig@, tu SUVAN esta por llegar, acercate a la estación de ascenso!", string.Empty);
          }
          #endregion
        }
      }
      catch
      { }
      response.Data = responseCP;
      response.CodigoMensaje = "200";
      response.Mensaje = "Termino de abordaje realizado con exito";
      return response;
    }

    public async Task<SuVanResponse<CorridaParadaResponse>> CheckIn(BoletoCheckRequest data, int ConductorId)
    {

      SuVanResponse<CorridaParadaResponse> response = new();
      response.CodigoMensaje = "400";
      response.Mensaje = "Datos incorrectos";

      Viaje? viaje = await _context.Viajes.FirstOrDefaultAsync(x => x.Boleto == data.Boleto);

      if (viaje == null)
      {
        response.CodigoMensaje = "400";
        response.Mensaje = "El boleto no existe";
        return response;
      }

      if (viaje.EstatusviajeIdestatusviaje != Convert.ToInt32(EnumEstatusViaje.EN_ESPERA))
      {
        response.CodigoMensaje = "400";
        response.Mensaje = "El boleto ya fue leído previamente";
        return response;

      }




      CorridaAsignacion corrida = await _context.CorridaAsignacions.Where(x => x.ConductorIdconductor == ConductorId &&
                           x.EstatusviajeIdestatusviaje == Convert.ToInt32(EnumEstatusViaje.EN_CURSO) &&
                           x.IdcorridaAsignacion == viaje.CorridaAsignacionIdcorridaAsignacion)
                          .OrderByDescending(x => x.Fecha).OrderByDescending(o => o.IdcorridaAsignacion)
                          .FirstOrDefaultAsync();

      if (corrida == null || corrida.EstatusviajeIdestatusviaje != Convert.ToInt32(EnumEstatusViaje.EN_CURSO)) //Se valida que el viaje esté en curso
      {
        response.CodigoMensaje = "400";
        response.Mensaje = "La corrida no está activa";
        return response;
      }

      if (corrida.IdestacionActual == null)
      {

        var actual = (from a in _context.CorridaAsignacions
                      join b in _context.CorridaAsignacionParada on a.IdcorridaAsignacion equals b.CorridaAsignacionIdcorridaAsignacion
                      join c in _context.Parada on b.ParadaIdparada equals c.Idparada
                      join d in _context.Corrida on a.CorridaIdcorrida equals d.Idcorrida
                      join e in _context.RutaParada on d.RutaIdruta equals e.RutaIdruta
                      where a.IdcorridaAsignacion == corrida.IdcorridaAsignacion
                       && (b.EstatusestacionIdestatusestacion == 0 || b.EstatusestacionIdestatusestacion == 1)
                       && b.ParadaIdparada == e.ParadaIdparada
                      orderby e.Orden
                      select new CorridaEstacion()
                      {
                        Idruta = e.RutaIdruta,
                        Idparada = c.Idparada,
                        Nombre = c.Nombre,
                        Direccion = (c.Calle ?? "") + (("No. " + c.Numero) ?? "") + ((", " + c.Colonia) ?? "") + ((", " + c.Codigopostal) ?? "") + ((", " + c.Municipio) ?? "") + ((", " + c.Ciudad) ?? ""),
                        Latitud = c.Latitud ?? 0,
                        Longitud = c.Longitud ?? 0,
                        IdEstatus = b.EstatusestacionIdestatusestacion,
                        Orden = e.Orden,
                        SiguienteAbordan = 0,
                        SiguienteBajan = 0
                      }).FirstOrDefault();

        corrida.IdestacionActual = actual.Idparada;
        _context.CorridaAsignacions.Update(corrida);
        await _context.SaveChangesAsync();
      }


      if (viaje.ParadaInicio != corrida.IdestacionActual)
      {
        response.CodigoMensaje = "400";
        response.Mensaje = "El boleto pertenece a otra estación";
        return response;
      }

      CorridaAsignacionParadum cap = await _context.CorridaAsignacionParada.FirstOrDefaultAsync(x => x.CorridaAsignacionIdcorridaAsignacion == corrida.IdcorridaAsignacion
                                      && x.ParadaIdparada == corrida.IdestacionActual);


      viaje.Fechacheckin = DateTime.Now;
      viaje.EstatusviajeIdestatusviaje = (sbyte)(EnumEstatusViaje.EN_CURSO);
      _context.Viajes.Update(viaje);
      await _context.SaveChangesAsync();


      if (cap != null)
      {
        cap.Subieron += viaje.Numeropasajeros;
        _context.CorridaAsignacionParada.Update(cap);
        await _context.SaveChangesAsync();
      }

      CorridaParadaResponse responseCP = await LoadCorridaParada(viaje.CorridaAsignacionIdcorridaAsignacion, corrida.IdestacionActual ?? 0, false, false);
      responseCP.pasajeros = viaje.Numeropasajeros;

      response.Data = responseCP;
      response.CodigoMensaje = "200";
      response.Mensaje = "Check-In realizado con exito";


      Usuario u = _context.Usuarios.FirstOrDefault(x => x.Idusuario == viaje.UsuarioIdusuario);

      try
      {
        if (u.FirebaseId != null && u.FirebaseId != String.Empty)
        {
          SUVAN.BackOffice.Models.Notification.Data dato = new Models.Notification.Data();
          dato.comando = "inicio_viaje";
          dato.reserva_id = viaje.Idviaje;
          dato.corrida_asignacion_id = corrida.IdcorridaAsignacion;

          await _notificacionPushService.EnvioNotificacion(u.FirebaseId, dato, "Viaje en curso", "Tu viaje con SUVAN ha iniciado. Esperamos sea de tu agrado.", "OpenRoute");
        }

      }
      catch
      (Exception ex)
      {
      }


      return response;
    }

    private async Task<CorridaParadaResponse> LoadCorridaParada(int? Idcorrida, int Idparada, Boolean bajan, Boolean notif)
    {
      CorridaParadaResponse result = new CorridaParadaResponse();

      result.Idcorrida = Idcorrida;

      result.Idparada = Idparada;

      result.Suben = await GetSuben(Idcorrida, Idparada);

      //if (bajan)
      //{
      result.Bajan = await GetBajan(Idcorrida, Idparada, notif, notif);
      //}

      result.pasajeros = await GetTotalPasajeros(Idcorrida);

      return result;
    }

    private async Task<List<CorridaUsuario>> GetSuben(int? Idcorrida, int Idparada)
    {
      List<CorridaUsuario> Suben = await (from a in _context.Viajes
                                          join b in _context.Usuarios on a.UsuarioIdusuario equals b.Idusuario
                                          join c in _context.Estatusviajes on a.EstatusviajeIdestatusviaje equals c.Idestatusviaje
                                          where a.CorridaAsignacionIdcorridaAsignacion == Idcorrida && a.ParadaInicio == Idparada
                                          && (c.Idestatusviaje == Convert.ToInt32(EnumEstatusViaje.EN_ESPERA) ||
                                          c.Idestatusviaje == Convert.ToInt32(EnumEstatusViaje.EN_CURSO))
                                          select new CorridaUsuario
                                          {
                                            Idviaje = a.Idviaje,
                                            Idusuario = a.UsuarioIdusuario,
                                            Nombre = b.Nombre,
                                            Pasajeros = a.Numeropasajeros,
                                            Status = a.EstatusviajeIdestatusviaje,
                                            Firebase = b.FirebaseId ?? string.Empty
                                          }).ToListAsync();

      return Suben;
    }

    private async Task<List<CorridaUsuario>> GetBajan(int? Idcorrida, int Idparada, Boolean notif, bool procesa)
    {
      var statusBajan = new int[] { Convert.ToInt32(EnumEstatusViaje.EN_CURSO)
        , Convert.ToInt32(EnumEstatusViaje.EN_ESPERA)
        , Convert.ToInt32(EnumEstatusViaje.FINALIZADO)
      };

      List<int> lstidsViaje = new List<int>();
      List<CorridaUsuario> Bajan = await (from a in _context.Viajes
                                          join b in _context.Usuarios on a.UsuarioIdusuario equals b.Idusuario
                                          join c in _context.Estatusviajes on a.EstatusviajeIdestatusviaje equals c.Idestatusviaje
                                          where a.CorridaAsignacionIdcorridaAsignacion == Idcorrida && a.ParadaFin == Idparada
                                          && statusBajan.Contains(a.EstatusviajeIdestatusviaje)
                                          select new CorridaUsuario
                                          {
                                            Idviaje = a.Idviaje,
                                            Idusuario = a.UsuarioIdusuario,
                                            Nombre = b.Nombre,
                                            Pasajeros = a.Numeropasajeros,
                                            Status = a.EstatusviajeIdestatusviaje,
                                            Firebase = b.FirebaseId ?? ""
                                          }).ToListAsync();

      foreach (var item in Bajan)
      {
        lstidsViaje.Add(item.Idviaje);

        if (notif)
        {
          if (item.Firebase != null && item.Firebase != String.Empty && item.Status == Convert.ToInt32(EnumEstatusViaje.EN_CURSO))
          {
            SUVAN.BackOffice.Models.Notification.Data dato = new Models.Notification.Data();
            dato.comando = "fin_viaje";
            dato.reserva_id = item.Idviaje;
            dato.corrida_asignacion_id = Idcorrida ?? 0;

            await _notificacionPushService.EnvioNotificacion(item.Firebase, dato, "Viaje finalizado", "Tu viaje con SUVAN ha finalizado. Te esperamos pronto.", "OpenRoute");
          }
        }
      }


      if (procesa)
      {
        if (lstidsViaje.Count > 0)
        {
          int[] idsViaje = lstidsViaje.ToArray();
          List<Viaje> viajeEntity = await _context.Viajes.Where(x => idsViaje.Contains(x.Idviaje)).ToListAsync();

          foreach (var vViaje in viajeEntity)
          {
            if (vViaje.EstatusviajeIdestatusviaje == Convert.ToInt32(EnumEstatusViaje.EN_CURSO))
            {
              vViaje.EstatusviajeIdestatusviaje = (sbyte)(EnumEstatusViaje.FINALIZADO);
            }
            else if (vViaje.EstatusviajeIdestatusviaje == Convert.ToInt32(EnumEstatusViaje.EN_ESPERA))
            {
              vViaje.EstatusviajeIdestatusviaje = (sbyte)(EnumEstatusViaje.PERDIDO);
              // Actualizar el status en la lista de Bajan
              var item2 = Bajan.FirstOrDefault(b => b.Idviaje == vViaje.Idviaje);
              if (item2 != null)
              {
                item2.Status = (sbyte)(EnumEstatusViaje.PERDIDO);
              }
            }
            _context.Viajes.Update(vViaje);
            await _context.SaveChangesAsync();
          }
        }
      }
      return Bajan;
    }

    private async Task<List<CorridaUsuario>> GetBajanNext(int? Idcorrida, int Idparada)
    {
      var statusBajan = new int[] { Convert.ToInt32(EnumEstatusViaje.EN_CURSO)
        , Convert.ToInt32(EnumEstatusViaje.EN_ESPERA)
        , Convert.ToInt32(EnumEstatusViaje.FINALIZADO)
      };

      List<int> lstidsViaje = new List<int>();
      List<CorridaUsuario> Bajan = await (from a in _context.Viajes
                                          join b in _context.Usuarios on a.UsuarioIdusuario equals b.Idusuario
                                          join c in _context.Estatusviajes on a.EstatusviajeIdestatusviaje equals c.Idestatusviaje
                                          where a.CorridaAsignacionIdcorridaAsignacion == Idcorrida && a.ParadaFin == Idparada
                                          && statusBajan.Contains(a.EstatusviajeIdestatusviaje)
                                          select new CorridaUsuario
                                          {
                                            Idviaje = a.Idviaje,
                                            Idusuario = a.UsuarioIdusuario,
                                            Nombre = b.Nombre,
                                            Pasajeros = a.Numeropasajeros,
                                            Status = a.EstatusviajeIdestatusviaje,
                                            Firebase = b.FirebaseId ?? ""
                                          }).ToListAsync();


      return Bajan;
    }

    private async Task<int> GetTotalPasajeros(int? Idcorrida)
    {
      int pasajeros = await (from a in _context.Viajes
                             join c in _context.Estatusviajes on a.EstatusviajeIdestatusviaje equals c.Idestatusviaje
                             where a.CorridaAsignacionIdcorridaAsignacion == Idcorrida
                            && (c.Idestatusviaje == Convert.ToInt32(EnumEstatusViaje.EN_ESPERA) ||
                            c.Idestatusviaje == Convert.ToInt32(EnumEstatusViaje.EN_CURSO) ||
                            c.Idestatusviaje == Convert.ToInt32(EnumEstatusViaje.FINALIZADO) ||
                            c.Idestatusviaje == Convert.ToInt32(EnumEstatusViaje.PERDIDO))
                             select a.Numeropasajeros).SumAsync(x => x.Value);
      return pasajeros;
    }
  }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.AppSettingsModels;
using SUVAN.BackOffice.Models.Auth.Token;
using SUVAN.BackOffice.Models.Auth.User;
using SUVAN.BackOffice.Models.Configuracion.Tarifas;
using SUVAN.BackOffice.Models.ManejoRespuesta;
using SUVAN.BackOffice.Models.Pago;
using SUVAN.BackOffice.Models.UnlimintPay.Pago;
using SUVAN.BackOffice.Models.UnlimintPay.Token;
using SUVAN.BackOffice.Service.RegistroUsuario;
using SUVAN.BackOffice.Service.UnlimintPay;
using SUVAN.BackOffice.Service.PayPal;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static SUVAN.BackOffice.Models.UnlimintPay.Pago.RespuestaValidacionPagoWS;
using static System.Reflection.Metadata.BlobBuilder;
using SUVAN.BackOffice.Models.PayPal.Token;
using SUVAN.BackOffice.Models.PayPal.Pago;
using SUVAN.BackOffice.Service.Configuracion;
using SUVAN.BackOffice.Service.Viajes;
using static SUVAN.BackOffice.Service.Viajes.ViajeService;
using SUVAN.BackOffice.Models.ViewModel.Enums;
using SUVAN.BackOffice.Models.Viajes;
using System.Transactions;
using SUVAN.BackOffice.Models.Corrida;
using SUVAN.BackOffice.Models.Notification;


namespace SUVAN.BackOffice.Service.Pago
{
  public class PagoService : IPagoService
  {
    private readonly SuvanDbContext _context;

    private readonly IUnlimintPayService _unlimintPayService;
    private readonly IUsuarioService _usuarioService;
    private readonly IPayPalService _payPalService;

    public PagoService(SuvanDbContext context, IUnlimintPayService unlimintPayService, IUsuarioService usuarioService, IPayPalService payPalService)
    {
      _context = context;
      _unlimintPayService = unlimintPayService;
      _usuarioService = usuarioService;
      _payPalService = payPalService;
    }
    #region Metodos para Pago con "Tarjeta de Credito o Debito"
    /// <summary>
    /// Pago de Viaje
    /// Metodo  para generar url de Pago
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public async Task<SuVanResponse<PagoResponse>> GenerarPago(int userId, PagoRequest data, string emailUser)
    {

      decimal Monto = 0;

      PagoResponse? result = new PagoResponse();
      SuVanResponse<PagoResponse> response = new();

      #region Validamos datos de entrada
      if (data.pasajeros <= 0)
      {
        //Ocurrió un error al generar token 
        response.CodigoMensaje = "400";
        response.Mensaje = "El valor pasajeros deber mayor a 0";
        return response;
      }

      //Tipo transaccion "Pago o Recarga"
      var tipoTransaccionResult = await _context.Tipotransaccions
          .Where(x => x.Idtipotransaccion == data.TipoTransaccionId)
          .FirstOrDefaultAsync();

      if (tipoTransaccionResult == null || tipoTransaccionResult.Idtipotransaccion != Convert.ToInt32(EnumTipoTransaccion.PAGO))
      {
        //Ocurrió un error al generar token 
        response.CodigoMensaje = "400";
        response.Mensaje = "Tipo de transacción incorrecta";
        return response;
      }

      //Tipo Metodo Pago "Tarjeta, Monedero, o PayPal"
      var tipoMetodoPagoResult = await _context.Metodopagos
          .Where(x => x.Idmetodopago == data.MetodoPagoId)
          .FirstOrDefaultAsync();

      if (tipoMetodoPagoResult == null || Convert.ToInt32(EnumMetodoPago.TARJETA_DEBITO_CREDITO) != tipoMetodoPagoResult.Idmetodopago)
      {
        response.CodigoMensaje = "400";
        response.Mensaje = "Tipo de método de pago incorrecto";
        return response;
      }

      //Validamos si existe Información para obtener la ruta
      ObtenDatosDeViajePorIdDeVajeModel datosViaje = null;
      datosViaje = await ObtenRutaViajePorId(data.reservacionId);


      if (datosViaje == null)
      {
        //Ocurrió un error al generar token 
        response.CodigoMensaje = "400";
        response.Mensaje = "No se encontro información";
        return response;
      }

      var disponibilidad = await ValidarDisponibilidad(datosViaje);
      if (!disponibilidad)
      {
        //Ocurrió validacion de disponibilidad
        response.Data = new PagoResponse { OrdenId = string.Empty };
        response.CodigoMensaje = "202";
        response.Mensaje = "Ya no se cuenta con disponibilidad de venta para el viaje";
        return response;
      }


      Monto = await ObtenTarifa(userId, emailUser, datosViaje.RutaId, datosViaje.CorridaId, datosViaje.ParadaInicio, datosViaje.ParadaFin, data.codigodescuento);
      #endregion

      //Calculamos el monto con base al número de pasajeros
      Monto = Monto * data.pasajeros;


      Monto = await AplicaPromocion(Monto, userId, emailUser, datosViaje.RutaId, datosViaje.CorridaId, data.codigodescuento);

      #region Región para generar token
      string tokenPay = string.Empty;
      RespuestaGeneracionToken responseGeneracionToken = null;
      responseGeneracionToken = _unlimintPayService.GenerarToken(userId, _unlimintPayService.ArmaRequestToken("password"));
      if (responseGeneracionToken.CodigoError != "00")
      {
        //Ocurrió un error al generar token 
        response.CodigoMensaje = "400";
        response.Mensaje = responseGeneracionToken.Error;
        return response;
      }
      //Respuesta Correcta de Token
      tokenPay = responseGeneracionToken.AccessToken;
      #endregion

      Guid gPeticionId = Guid.NewGuid();
      Guid gOrdenId = Guid.NewGuid();
      #region Región para generar uri de redirección de Pago   
      ////tokenPay = "eyJhbGciOiJSUzI1NiIsInR5cCIgOiAiSldUIiwia2lkIiA6ICJxY0FzMjlSNUFPbEltSFRobWJPR3lCYXI4aHZOdEo5Ni1GZGwyS2pVa1ZrIn0.eyJleHAiOjE3MDE4OTc5MzEsImlhdCI6MTcwMTg5NzYzMSwianRpIjoiZGM4NTI4NDgtMGViNi00MDk2LTk4OTktNTljZGQ5ZWUxOWJiIiwiaXNzIjoiaHR0cHM6Ly9zc28tc2FuZGJveC5jYXJkcGF5LmNvbS9hdXRoL3JlYWxtcy9hcGkiLCJzdWIiOiI3N2RhZWUwYy1hZjI2LTRlODYtYmU0Zi04YmQyYjRkYzhhMjYiLCJ0eXAiOiJCZWFyZXIiLCJhenAiOiJhcGl2My1hdXRoLWFnZW50Iiwic2Vzc2lvbl9zdGF0ZSI6ImJjNDNlNTZiLTI2YmYtNGU2Yy1hOTViLTAxZTYzYWVmMjBkOSIsInNjb3BlIjoiIiwic2lkIjoiYmM0M2U1NmItMjZiZi00ZTZjLWE5NWItMDFlNjNhZWYyMGQ5IiwidGVybWluYWwiOiI1MjcwOSJ9.ZEYaN0yRdBQwF5l-x84actONNFZk8ae7YQI1g-92OCsnp4OBlVZOmJj29Y2i_GlPg4iRj6A4qvsNwHi2am4rZ84k_JXxTzwwA5eCn0uAA1hrP-KVPoWGHURaRIToWDGjUnjQEA2AvEtWVWj-XYbXVeCZ4seS8MHMe9-syiYKH7-az2amZH9GDEJxpqS1RG9YVzhzBByzdY4IlOOiINH7upSM6Vpu9MsLyIImIA9lGbz28FncSus8HVvAHXpyaXKPOttw04hv5VTikZvaFMYvTKoFYzZ9o0XWEKzWWppM6CVPf6t_v__Wyer1XqoNc6MwOrIarv9mUeD5XA9-5lIuKw";
      RespuestaGeneracionPago responseGeneracionPago = null;

      int IntentosPermitidos = 2;
      for (int NoIntentos = 0; NoIntentos < IntentosPermitidos; NoIntentos++)
      {
        //Consumimos WS para generar el pago
        responseGeneracionPago = _unlimintPayService.GenerarUrlPago(_unlimintPayService.ArmaRequestPeticionGeneracionPago(tokenPay, gPeticionId.ToString(), gOrdenId.ToString(), emailUser, Monto, "MXN"));

        if (responseGeneracionPago.CodigoError == "02" && (!string.IsNullOrEmpty(responseGeneracionPago.Error) && responseGeneracionPago.Error.ToUpper().Contains("EXPIRED")))
        {
          //Si el token expiro volvemos a generar otro token pero con el "refresh_token" y volvemos a consumir el WS
          responseGeneracionToken = _unlimintPayService.GenerarToken(userId, _unlimintPayService.ArmaRequestToken("refresh_token", userId));
          if (responseGeneracionToken.CodigoError != "00")
          {
            //Ocurrió un error al generar token 
            response.CodigoMensaje = "400";
            response.Mensaje = responseGeneracionToken.Error;
            return response;
          }
          //Respuesta Correcta al intenar a generar el token por medio del "refresh_token"
          tokenPay = responseGeneracionToken.AccessToken;
        }
        else
        {
          //Si arrojo una expeción diferente a la expiracion del token o en este caso la respuesta haya sido correcta seteamos el valor de para que solo lo realice una vez el intento del consumo
          NoIntentos = IntentosPermitidos;
        }
      }

      if (responseGeneracionPago.CodigoError != "00")
      {
        //Ocurrió un error 
        response.CodigoMensaje = "400";
        response.Mensaje = responseGeneracionPago.Error;
        return response;
      }
      //Respuesta Correcta
      /*Guardamos Información de la Transacción*/
      #region Región para guardar registro del Pago en la tabla de Transaccion de la BD
      Transaccion entity = await GuardaTransaccion(userId, data.MetodoPagoId, data.TipoTransaccionId, Monto, gOrdenId.ToString(), gPeticionId.ToString(), null, null);
      #endregion

      #region Región para ligar Transaccion con la tabla de Viaje
      int Idtransaccion = entity.Idtransaccion;
      //Aqui el pasamos el estatus del Viaje Reservado ya que se cambia el estatus hasta que se confirme el Pago
      await AsignaIdTransaccionAViajeEstatus(data.reservacionId, Idtransaccion, (sbyte)EnumEstatusViaje.RESERVANDO);
      #endregion

      result.OrdenId = gOrdenId.ToString();
      result.PeticionId = gPeticionId.ToString();
      result.url = responseGeneracionPago.RedirectUrl;
      response.Mensaje = "Solicitud exitosa";
      response.CodigoMensaje = "200";
      response.Data = result;
      #endregion
      return response;

    }

    private async Task<bool> ValidarDisponibilidad(ObtenDatosDeViajePorIdDeVajeModel viaje)
    {


      var rutaParadasResult = await (from a in _context.RutaParada
                                     where a.RutaIdruta == viaje.RutaId
                                     && a.Activo == Convert.ToUInt64(true)
                                     select a).OrderBy(x => x.Orden).ToListAsync();

      var asientosDisponibles = 0;

      if (viaje.TipoTarifaId == Convert.ToInt32(EnumTipoTarifa.FIJA))
      {
        asientosDisponibles = await (from a in _context.CorridaAsignacionParada
                                     where a.CorridaAsignacionIdcorridaAsignacion == viaje.CorridaAsignacionId
                                     select a).MinAsync(x => x.Espacios) ?? 0;
      }
      else
      {
        var estacionesIntermedias = await ObtenEstacionesIntermedias(rutaParadasResult, viaje.ParadaInicio, viaje.ParadaFin);
        asientosDisponibles = await (from a in _context.CorridaAsignacionParada
                                     where a.CorridaAsignacionIdcorridaAsignacion == viaje.CorridaAsignacionId
                                     && estacionesIntermedias.Contains(a.ParadaIdparada)
                                     select a).MinAsync(x => x.Espacios) ?? 0;
      }


      return asientosDisponibles > 0;
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

    /// <summary>
    /// Confirma Pago de Viaje
    /// Metodo  para confirmar el pago por medio de "Tarjeta de Credito o Debito"
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public async Task<SuVanResponse<ConfirmarPagoResponse>> ConfirmaPago(int userId, ConfirmarPagoRequest data)
    {
      ConfirmarPagoResponse? result = new ConfirmarPagoResponse();
      SuVanResponse<ConfirmarPagoResponse> response = new();

      #region Validamos datos de entrada
      var ViajesResult = await _context.Viajes
      .Where(x => x.Idviaje == data.ReservacionId)
      .FirstOrDefaultAsync();

      if (ViajesResult == null)
      {
        response.CodigoMensaje = "400";
        response.Mensaje = "No se encontro información con el valor ReservacionId " + data.ReservacionId;
        return response;
      }

      var TransaccionsResult = await _context.Transaccions
          .Where(x => x.Idtransaccion == ViajesResult.TransaccionIdtransaccion)
          .FirstOrDefaultAsync();

      if (TransaccionsResult == null)
      {
        response.CodigoMensaje = "400";
        response.Mensaje = "No se encontro información de la Transacción";
        return response;
      }
      if (TransaccionsResult.EstatustransaccionIdestatustransaccion == Convert.ToInt32(EnumEstatusPago.COMPLETED) || TransaccionsResult.EstatustransaccionIdestatustransaccion == Convert.ToInt32(EnumEstatusPago.AUTHORIZED))
      {
        response.CodigoMensaje = "400";
        response.Mensaje = "Ya existe un pago con estatus COMPLETED para este viaje";
        return response;
      }
      #endregion

      #region Región para generar token
      string tokenPay = string.Empty;
      RespuestaGeneracionToken responseGeneracionToken = null;
      responseGeneracionToken = _unlimintPayService.GenerarToken(userId, _unlimintPayService.ArmaRequestToken("password"));
      if (responseGeneracionToken.CodigoError != "00")
      {
        //Ocurrió un error al generar token 
        response.CodigoMensaje = "400";
        response.Mensaje = responseGeneracionToken.Error;
        return response;
      }
      //Respuesta Correcta de Token
      tokenPay = responseGeneracionToken.AccessToken;
      #endregion

      #region Región para generar consumir wl ws para validar el estatus del pago
      //tokenPay = "eyJhbGciOiJSUzI1NiIsInR5cCIgOiAiSldUIiwia2lkIiA6ICJxY0FzMjlSNUFPbEltSFRobWJPR3lCYXI4aHZOdEo5Ni1GZGwyS2pVa1ZrIn0.eyJleHAiOjE3MDE4OTc5MzEsImlhdCI6MTcwMTg5NzYzMSwianRpIjoiZGM4NTI4NDgtMGViNi00MDk2LTk4OTktNTljZGQ5ZWUxOWJiIiwiaXNzIjoiaHR0cHM6Ly9zc28tc2FuZGJveC5jYXJkcGF5LmNvbS9hdXRoL3JlYWxtcy9hcGkiLCJzdWIiOiI3N2RhZWUwYy1hZjI2LTRlODYtYmU0Zi04YmQyYjRkYzhhMjYiLCJ0eXAiOiJCZWFyZXIiLCJhenAiOiJhcGl2My1hdXRoLWFnZW50Iiwic2Vzc2lvbl9zdGF0ZSI6ImJjNDNlNTZiLTI2YmYtNGU2Yy1hOTViLTAxZTYzYWVmMjBkOSIsInNjb3BlIjoiIiwic2lkIjoiYmM0M2U1NmItMjZiZi00ZTZjLWE5NWItMDFlNjNhZWYyMGQ5IiwidGVybWluYWwiOiI1MjcwOSJ9.ZEYaN0yRdBQwF5l-x84actONNFZk8ae7YQI1g-92OCsnp4OBlVZOmJj29Y2i_GlPg4iRj6A4qvsNwHi2am4rZ84k_JXxTzwwA5eCn0uAA1hrP-KVPoWGHURaRIToWDGjUnjQEA2AvEtWVWj-XYbXVeCZ4seS8MHMe9-syiYKH7-az2amZH9GDEJxpqS1RG9YVzhzBByzdY4IlOOiINH7upSM6Vpu9MsLyIImIA9lGbz28FncSus8HVvAHXpyaXKPOttw04hv5VTikZvaFMYvTKoFYzZ9o0XWEKzWWppM6CVPf6t_v__Wyer1XqoNc6MwOrIarv9mUeD5XA9-5lIuKw";
      RespuestaValidacionPago responseValidacionPago = null;
      int IntentosPermitidos = 3;
      for (int NoIntentos = 0; NoIntentos < IntentosPermitidos; NoIntentos++)
      {
        //Consumimos WS para validar el pago
        responseValidacionPago = _unlimintPayService.ValidarPago(_unlimintPayService.ArmaRequestPeticionValidaPago(tokenPay, TransaccionsResult.Numeroordenpay, TransaccionsResult.Numeropeticionpay));

        if (responseValidacionPago.CodigoError == "02" && (!string.IsNullOrEmpty(responseValidacionPago.Error) && responseValidacionPago.Error.ToUpper().Contains("EXPIRED")))
        {
          //Si el token expiro volvemos a generar otro token pero con el "refresh_token" y volvemos a consumir el WS
          responseGeneracionToken = _unlimintPayService.GenerarToken(userId, _unlimintPayService.ArmaRequestToken("refresh_token", userId));
          if (responseGeneracionToken.CodigoError != "00")
          {
            //Ocurrió un error al generar token 
            response.CodigoMensaje = "400";
            response.Mensaje = responseGeneracionToken.Error;
            return response;
          }
          //Respuesta Correcta al intenar a generar el token por medio del "refresh_token"
          tokenPay = responseGeneracionToken.AccessToken;
        }
        else if (responseValidacionPago.DatosPago.data.Count > 0)
        {
          // si la solicitud esta en inprogres deja que siga intentando
          if (responseValidacionPago.DatosPago.data[0].payment_data.status.ToUpper() != "IN_PROGRESS")
          {
            NoIntentos = IntentosPermitidos;
          }
        }
        else
        {
          //Si arrojo una expeción diferente a la expiracion del token o en este caso la respuesta haya sido correcta seteamos el valor de para que solo lo realice una vez el intento del consumo
          NoIntentos = IntentosPermitidos;
        }
      }

      if (responseValidacionPago.CodigoError != "00")
      {
        //Ocurrió un error 
        response.CodigoMensaje = "400";
        response.Mensaje = responseValidacionPago.Error;
        return response;
      }
      if (responseValidacionPago.DatosPago.data.Count > 0)
      {
        int EstatusPago = Convert.ToInt32(Convert.ToInt32(EnumEstatusPago.IN_PROGRESS));//Seteamos por default el estatus en Progreso 
        switch (responseValidacionPago.DatosPago.data[0].payment_data.status.ToUpper())
        {
          case "IN_PROGRESS":
            EstatusPago = Convert.ToInt32(EnumEstatusPago.IN_PROGRESS);
            break;
          case "COMPLETED":
            EstatusPago = Convert.ToInt32(EnumEstatusPago.COMPLETED);
            break;
          case "DECLINED":
            EstatusPago = Convert.ToInt32(EnumEstatusPago.DECLINED);
            break;
          case "CANCEL"://PENDIENTE validar que si devuelva este estatus
            EstatusPago = Convert.ToInt32(EnumEstatusPago.CANCEL);
            break;
          case "AUTHORIZED":
            EstatusPago = Convert.ToInt32(EnumEstatusPago.AUTHORIZED);
            break;
        }
        #region Región para ligar Transaccion con la tabla de Viaje
        if (EstatusPago == Convert.ToInt32(EnumEstatusPago.COMPLETED) || EstatusPago == Convert.ToInt32(EnumEstatusPago.AUTHORIZED))
        {
          //Como el estatus que regewo el WS es "COMPLETED" o "AUTHORIZED" cambiamos el estatus del Viaje a en "PROCESO"
          // await CambiaEstatusViaje(data.ReservacionId, (sbyte)EnumEstatusViaje.EN_ESPERA);
          //await ActualizaOcupacion(data.ReservacionId);
          await AsignaIdTransaccionAViajeParada(data.ReservacionId, TransaccionsResult.Idtransaccion, (sbyte)EnumEstatusViaje.EN_ESPERA);
        }
        #endregion

        ////Respuesta Correcta
        #region Región para guardar registro del Pago en la tabla de Transaccion de la BD
        if (EstatusPago == Convert.ToInt32(EnumEstatusPago.COMPLETED) || EstatusPago == Convert.ToInt32(EnumEstatusPago.AUTHORIZED))
        {
          TransaccionsResult.EstatustransaccionIdestatustransaccion = EstatusPago; //2
          await _context.SaveChangesAsync();
        }

        #endregion
        result.estatus = responseValidacionPago.DatosPago.data[0].payment_data.status.ToUpper(); //ok
        response.Mensaje = "Solicitud exitosa";
        response.CodigoMensaje = "200";
        response.Data = result;
      }
      else
      {
        //Ocurrió un error al generar token 
        response.CodigoMensaje = "400";
        response.Mensaje = "Sin información de estatus de pago ";
        return response;
      }
      #endregion

      return response;

    }
    #endregion

    /// <summary>
    /// Guardamos en BD a la tabla de "Logtransaccions" este es exclusivo para el metodo de pago por "Tarjeta de Credito o debito"
    /// Ya que nos mandan el callback de UNLIMIT
    /// </summary>
    /// <param name="respuesta"></param>
    /// <returns></returns>
    public async Task<bool> GuardaBitacoraTransaccion(string respuesta)
    {
      var entity = new Database.Entities.Logtransaccion()
      {

        Respuesta = respuesta,
        Fecharegistro = DateTime.Now
      };
      _context.Logtransaccions.Add(entity);
      await _context.SaveChangesAsync();

      RespuestaValidacionPagoWS jsonRespuesta2 = new RespuestaValidacionPagoWS();
      jsonRespuesta2 = JsonConvert.DeserializeObject<RespuestaValidacionPagoWS>(respuesta);

      if (jsonRespuesta2 != null)
      {
        if (jsonRespuesta2.data.Count > 0)
        {
          int EstatusPago = Convert.ToInt32(EnumEstatusPago.IN_PROGRESS);//Seteamos por default el estatus en Progreso 
          switch (jsonRespuesta2.data[0].payment_data.status.ToUpper())
          {
            case "IN_PROGRESS":
              EstatusPago = Convert.ToInt32(EnumEstatusPago.IN_PROGRESS);
              break;
            case "COMPLETED":
              EstatusPago = Convert.ToInt32(EnumEstatusPago.COMPLETED);
              break;
            case "DECLINED":
              EstatusPago = Convert.ToInt32(EnumEstatusPago.DECLINED);
              break;
            case "CANCEL"://PENDIENTE validar que si devuelva este estatus
              EstatusPago = Convert.ToInt32(EnumEstatusPago.CANCEL);
              break;
            case "AUTHORIZED":
              EstatusPago = Convert.ToInt32(EnumEstatusPago.AUTHORIZED);
              break;
          }
        }

      }


      string ok = "";

      return true;
    }

    #region Metodos para Pago con "Monedero"
    /// <summary>
    /// Con este metodo se paga el viaje pero con el Monedero
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="data"></param>
    /// <param name="code"></param>
    /// <returns></returns>
    public async Task<SuVanResponse<PagoMonederoResponse>> GenerarPagoMonedero(int userId, string emailUser, PagoMonederoRequest data, string code)
    {

      decimal Monto = 0;
      decimal SaldoFinal = 0;
      bool seGeneraOTP = false;

      PagoMonederoResponse? result = new PagoMonederoResponse();
      SuVanResponse<PagoMonederoResponse> response = new();

      #region Validamos datos de entrada
      if (data.pasajeros <= 0)
      {
        //Ocurrió un error al generar token 
        response.CodigoMensaje = "400";
        response.Mensaje = "El valor pasajeros deber mayor a 0";
        return response;
      }

      //Tipo transaccion "Pago o Recarga"
      var tipoTransaccionResult = await _context.Tipotransaccions
          .Where(x => x.Idtipotransaccion == data.TipoTransaccionId)
          .FirstOrDefaultAsync();


      if (tipoTransaccionResult == null || tipoTransaccionResult.Idtipotransaccion != Convert.ToInt32(EnumTipoTransaccion.PAGO))
      {
        //Ocurrió un error al generar token 
        response.CodigoMensaje = "400";
        response.Mensaje = "Tipo de transacción incorrecta";
        return response;
      }

      //Tipo Metodo Pago "Tarjeta, Monedero, o PayPal"
      var tipoMetodoPagoResult = await _context.Metodopagos
          .Where(x => x.Idmetodopago == data.MetodoPagoId)
          .FirstOrDefaultAsync();

      if (tipoMetodoPagoResult == null || Convert.ToInt32(EnumMetodoPago.MONEDERO) != tipoMetodoPagoResult.Idmetodopago)
      {
        response.CodigoMensaje = "400";
        response.Mensaje = "Tipo de método de pago incorrecto";
        return response;
      }

      //Validamos si existe Información para obtener la ruta
      ObtenDatosDeViajePorIdDeVajeModel datosViaje = null;
      datosViaje = await ObtenRutaViajePorId(data.reservacionId);

      if (datosViaje == null)
      {
        //Ocurrió un error al generar token 
        response.CodigoMensaje = "400";
        response.Mensaje = "No se encontro información";
        return response;
      }

      var disponibilidad = await ValidarDisponibilidad(datosViaje);
      if (!disponibilidad)
      {
        //Ocurrió validacion de disponibilidad
        response.Data = new PagoMonederoResponse { OrdenId = string.Empty };
        response.CodigoMensaje = "202";
        response.Mensaje = "Ya no se cuenta con disponibilidad de venta para el viaje";
        return response;
      }


      Monto = await ObtenTarifa(userId, emailUser, datosViaje.RutaId, datosViaje.CorridaId, datosViaje.ParadaInicio, datosViaje.ParadaFin, data.codigodescuento);

      //Calculamos el monto con base al número de pasajeros
      Monto = Monto * data.pasajeros;

      Monto = await AplicaPromocion(Monto, userId, emailUser, datosViaje.RutaId, datosViaje.CorridaId, data.codigodescuento);

      if (Monto <= 0)
      {
        Monto = 0;
        decimal SaldoMonederoDisponible = 0;
        //Actualizamos Saldo
        Database.Entities.Monedero? MonederoEntity = await _context.Monederos.FirstOrDefaultAsync(x => x.UsuarioIdusuario == userId);
        if (MonederoEntity == null)
        {
          //Insertamos Saldo
          var MonederoEntityInsert = new Database.Entities.Monedero()
          {
            UsuarioIdusuario = userId,
            Saldo = 0
          };
          _context.Monederos.Add(MonederoEntityInsert);
        }
        await _context.SaveChangesAsync();
      }


      //Revisamos el Saldo del Monedero 
      var SaldoMonederoResult = await _context.Monederos
          .FirstOrDefaultAsync(x => x.UsuarioIdusuario == userId);

      if (SaldoMonederoResult == null)
      {
        response.CodigoMensaje = "400";
        response.Mensaje = "Saldo insuficiente";
        return response;
      }
      if (SaldoMonederoResult.Saldo < Monto)
      {
        response.CodigoMensaje = "400";
        response.Mensaje = "Saldo insuficiente";
        return response;
      }
      #endregion

      Transaccion entity = null;
      #region Revisamos si se guarda Código OTP
      seGeneraOTP = string.IsNullOrEmpty(data.opt) ? true : false;//Si el valor data.opt viene vacio es porque Guardamos el valor del código
      string? CodigoOTP = null;
      DateTime? Codigoexp = null;
      if (seGeneraOTP)
      {
        CodigoOTP = code;
        Codigoexp = SUVAN.BackOffice.Utilities.GeneraCodigos.ExpiracionCodigoActivacion();

        /*Guardamos Información de la Transacción*/
        #region Región para guardar registro del Pago en la tabla de Transaccion de la BD
        if (Monto == 0)
        {
          entity = await GuardaTransaccion(userId, data.MetodoPagoId, data.TipoTransaccionId, Monto, "Transaccion monto $0 promoción", null, CodigoOTP, Codigoexp);
        }
        else
        {
          entity = await GuardaTransaccion(userId, data.MetodoPagoId, data.TipoTransaccionId, Monto, null, null, CodigoOTP, Codigoexp);
        }
        #endregion
      }
      else
      {
        ///Obtenemos la Informacion de la Transacción por medio del código OTP
        entity = await _context.Transaccions.FirstOrDefaultAsync(x => x.Codigo == data.opt && x.UsuarioIdusuario == userId && x.TipotransaccionIdtipotransaccion == data.TipoTransaccionId);
        //Validamos el Código OTP
        if (entity == null)
        {
          response.CodigoMensaje = "400";
          response.Mensaje = "Código Incorrecto";
          return response;
        }
        if (DateTime.Now > entity.Codigoexp)
        {
          response.CodigoMensaje = "400";
          response.Mensaje = "Código expirado";
          return response;
        }
        entity.EstatustransaccionIdestatustransaccion = Convert.ToInt32(EnumEstatusPago.COMPLETED);//Para este tipo de pago por MONEDERO se pone como Completado
        entity.Codigo = null;//Como ya paso todas las validaciones lp dejamos en null
        entity.Codigoexp = null;//Como ya paso todas las validaciones lp dejamos en null
        await _context.SaveChangesAsync();
      }
      #endregion

      if (seGeneraOTP)
      {
        result.saldomonederoactualizado = null;
        response.Mensaje = "Código generado";
        response.CodigoMensaje = "206";
        response.Data = result;
        return response;
      }

      #region Región para ligar Transaccion con la tabla de Viaje y tambien el estatus del Viaje
      int Idtransaccion = entity.Idtransaccion;
      await AsignaIdTransaccionAViajeParada(data.reservacionId, Idtransaccion, (sbyte)EnumEstatusViaje.EN_ESPERA);
      //await ActualizaOcupacion(data.reservacionId);
      #endregion

      #region Actualizamos saldo de Monedero
      //Actualizamos Saldo de Monedero
      SaldoFinal = (decimal)await ActualizaSaldoMonedero(userId, Monto);
      #endregion

      result.saldomonederoactualizado = SaldoFinal.ToString();
      response.Mensaje = "Solicitud exitosa";
      response.CodigoMensaje = "200";
      response.Data = result;

      return response;


    }
    #endregion
    /// <summary>
    /// Metodo para asignar Id de la transaccion a Viaje
    /// </summary>
    /// <param name="reservacionId"></param>
    /// <param name="Idtransaccion"></param>
    /// <param name="EstatusViaje"></param>
    /// <returns></returns>
    private async Task AsignaIdTransaccionAViaje(int reservacionId, int Idtransaccion, sbyte EstatusViaje)
    {
      Viaje? ViajeEntity = await _context.Viajes
      .Where(x => x.Idviaje == reservacionId)
      .FirstOrDefaultAsync();
      if (ViajeEntity != null)
      {
        ViajeEntity.EstatusviajeIdestatusviaje = EstatusViaje;
        ViajeEntity.TransaccionIdtransaccion = Idtransaccion;
        await _context.SaveChangesAsync();

        await ActualizaCorridaParadaAsignacion(reservacionId);
      }
    }

    private async Task CambiaEstatusViaje(int reservacionId, sbyte EstatusViaje)
    {
      Viaje? ViajeEntity = await _context.Viajes
      .Where(x => x.Idviaje == reservacionId)
      .FirstOrDefaultAsync();
      if (ViajeEntity != null)
      {
        ViajeEntity.EstatusviajeIdestatusviaje = EstatusViaje;
        await _context.SaveChangesAsync();

        await ActualizaCorridaParadaAsignacion(reservacionId);
      }
    }

    /// <summary>
    /// Actualizamos Saldo de Monedero
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="SaldoDeCompra"></param>
    /// <returns></returns>
    public async Task<decimal?> ActualizaSaldoMonedero(int userId, decimal SaldoDeCompra)
    {
      decimal SaldoFinal = 0;
      //Actualizamos Saldo
      Database.Entities.Monedero? MonederoEntity = await _context.Monederos.FirstOrDefaultAsync(x => x.UsuarioIdusuario == userId);
      if (MonederoEntity != null)
      {
        SaldoFinal = (decimal)(MonederoEntity.Saldo ?? 0) - SaldoDeCompra;//Restamos el saldo del Monedero
        MonederoEntity.Saldo = SaldoFinal;
        await _context.SaveChangesAsync();
      }
      return SaldoFinal;
    }

    /// <summary>
    /// Guardamos en BD en la tabla de "Transaccion" el apgo que se realizo
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="MetodoPagoId"></param>
    /// <param name="TipoTransacionId"></param>
    /// <param name="Monto"></param>
    /// <param name="Numeroordenpay"></param>
    /// <param name="Numeropeticionpay"></param>
    /// <param name="CodigoOTP"></param>
    /// <param name="Codigoexp"></param>
    /// <returns></returns>
    public async Task<Transaccion> GuardaTransaccion(int userId, int MetodoPagoId, int TipoTransacionId, decimal Monto, string? Numeroordenpay, string? Numeropeticionpay, string? CodigoOTP, DateTime? Codigoexp)
    {
      var entity = new Database.Entities.Transaccion()
      {
        MetodopagoIdmetodopago = MetodoPagoId,
        Terminacion = null,
        Cantidad = Monto,
        TipotransaccionIdtipotransaccion = TipoTransacionId,
        UsuarioIdusuario = userId,
        Numeroordenpay = Numeroordenpay,
        Numeropeticionpay = Numeropeticionpay,
        EstatustransaccionIdestatustransaccion = Convert.ToInt32(EnumEstatusPago.IN_PROGRESS),
        Fecharegistro = DateTime.Now,
        Codigo = CodigoOTP,
        Codigoexp = Codigoexp
      };
      _context.Transaccions.Add(entity);
      await _context.SaveChangesAsync();
      return entity;
    }


    #region  Metodos para Pago con "PayPal"
    /// <summary>
    /// Pago de Viaje
    /// Metodo  para confirmar el pago por medio de "Tarjeta de Credito o Debito"
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public async Task<SuVanResponse<PagoResponse>> GenerarPagoConPayPal(int userId, string emailUser, PagoRequest data)
    {

      decimal Monto = 0;
      string UrlRedirect = string.Empty;
      PagoResponse? result = new PagoResponse();
      SuVanResponse<PagoResponse> response = new();

      #region Validamos datos de entrada
      if (data.pasajeros <= 0)
      {
        //Ocurrió un error al generar token 
        response.CodigoMensaje = "400";
        response.Mensaje = "El valor pasajeros deber mayor a 0";
        return response;
      }

      //Tipo transaccion "Pago o Recarga"
      var tipoTransaccionResult = await _context.Tipotransaccions
          .Where(x => x.Idtipotransaccion == data.TipoTransaccionId)
          .FirstOrDefaultAsync();

      if (tipoTransaccionResult == null || tipoTransaccionResult.Idtipotransaccion != Convert.ToInt32(EnumTipoTransaccion.PAGO))
      {
        //Ocurrió un error al generar token 
        response.CodigoMensaje = "400";
        response.Mensaje = "Tipo de transacción incorrecta";
        return response;
      }

      //Tipo Metodo Pago "Tarjeta, Monedero, o PayPal"
      var tipoMetodoPagoResult = await _context.Metodopagos
          .Where(x => x.Idmetodopago == data.MetodoPagoId)
          .FirstOrDefaultAsync();

      if (tipoMetodoPagoResult == null || Convert.ToInt32(EnumMetodoPago.PAYPAL) != tipoMetodoPagoResult.Idmetodopago)
      {
        response.CodigoMensaje = "400";
        response.Mensaje = "Tipo de método de pago incorrecto";
        return response;
      }

      //Validamos si existe Información para obtener la ruta
      ObtenDatosDeViajePorIdDeVajeModel datosViaje = null;
      datosViaje = await ObtenRutaViajePorId(data.reservacionId);

      if (datosViaje == null)
      {
        //Ocurrió un error al generar token 
        response.CodigoMensaje = "400";
        response.Mensaje = "No se encontro información";
        return response;
      }

      var disponibilidad = await ValidarDisponibilidad(datosViaje);
      if (!disponibilidad)
      {
        //Ocurrió validacion de disponibilidad
        response.Data = new PagoResponse { OrdenId = string.Empty };
        response.CodigoMensaje = "202";
        response.Mensaje = "Ya no se cuenta con disponibilidad de venta para el viaje";
        return response;
      }

      Monto = await ObtenTarifa(userId, emailUser, datosViaje.RutaId, datosViaje.CorridaId, datosViaje.ParadaInicio, datosViaje.ParadaFin, data.codigodescuento);
      #endregion

      //Calculamos el monto con base al número de pasajeros
      Monto = Monto * data.pasajeros;


      Monto = await AplicaPromocion(Monto, userId, emailUser, datosViaje.RutaId, datosViaje.CorridaId, data.codigodescuento);

      #region Región para generar token
      string tokenPayPal = string.Empty;
      PayPalAccessTokenResponse responseGeneracionToken = null;
      responseGeneracionToken = await _payPalService.GenerarTokenPayPal(userId, _payPalService.ArmaObjetoRequestTokenPayPal("client_credentials"));
      if (responseGeneracionToken.CodigoError != "00")
      {
        //Ocurrió un error al generar token 
        response.CodigoMensaje = "400";
        response.Mensaje = responseGeneracionToken.Error;
        return response;
      }
      //Respuesta Correcta de Token
      tokenPayPal = responseGeneracionToken.Access_token;
      #endregion

      Guid gPeticionId = Guid.NewGuid();//Para PayPal no es neceario este Id de Peticion pero lo generamos para guardarlo en la tabla de transaccion
      string gOrdenId = string.Empty;//Para PayPal E obitene un ID que se llama "Token" pero aqui lo seguimos manejando como Id de Orden
      #region Región para consumir el WS para generar la orden
      CreateOrderResponse responseOrderResponse = null;
      CreateOrderResponseWS responseOrderResponseWS = new CreateOrderResponseWS();
      ///Consumimos WS para generar la orden
      responseOrderResponse = await _payPalService.CreateOrder(tokenPayPal, _payPalService.ArmaObjetoRequestCreateOrder(gPeticionId.ToString(), Monto.ToString()));

      if (responseOrderResponse.CodigoError != "00")
      {
        //Ocurrió un error 
        response.CodigoMensaje = "400";
        response.Mensaje = responseOrderResponse.Error;
        return response;
      }
      else
      {
        //Respuesta Correcta
        responseOrderResponseWS = responseOrderResponse.DatosOrder;//Asignamos el objeto
        gOrdenId = responseOrderResponseWS.id;
        foreach (var item in responseOrderResponseWS.links)
        {
          if (item.rel == "approve")//Solo nos interesa la url para aprobar el pago
          {
            UrlRedirect = item.href;
          }
        }
        /*Guardamos Información de la Transacción*/
        #region Región para guardar registro del Pago en la tabla de Transaccion de la BD
        Transaccion entity = await GuardaTransaccion(userId, data.MetodoPagoId, data.TipoTransaccionId, Monto, gOrdenId.ToString(), null, null, null);
        #endregion

        #region Región para ligar Transaccion con la tabla de Viaje
        int Idtransaccion = entity.Idtransaccion;
        //Aqui el pasamos el estatus del Viaje Reservado ya que se cambia el estatus hasta que se confirme el Pago
        await AsignaIdTransaccionAViajeEstatus(data.reservacionId, Idtransaccion, (sbyte)EnumEstatusViaje.RESERVANDO);
        #endregion

        result.OrdenId = gOrdenId.ToString();
        result.PeticionId = gPeticionId.ToString();
        result.url = UrlRedirect;///Url para que el usuario Confirme la compra
        response.Mensaje = "Solicitud exitosa";
        response.CodigoMensaje = "200";
        response.Data = result;
      }
      #endregion
      return response;

    }




    /// <summary>
    /// Confirma Pago de Viaje
    /// Metodo para confirmar el pago por PayPal
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="data"></param>
    /// <param name="order_id"></param>
    /// <returns></returns>
    public async Task<SuVanResponse<ConfirmarPagoResponse>> ConfirmaPagoPayPal(int userId, ConfirmarPagoRequest data)
    {
      ConfirmarPagoResponse? result = new ConfirmarPagoResponse();
      SuVanResponse<ConfirmarPagoResponse> response = new();
      string EstatusOrden = string.Empty;
      string order_id = string.Empty;

      #region Validamos datos de entrada
      var ViajesResult = await _context.Viajes
                 .Where(x => x.Idviaje == data.ReservacionId)
                 .FirstOrDefaultAsync();

      if (ViajesResult == null)
      {
        response.CodigoMensaje = "400";
        response.Mensaje = "No se encontro información con el valor ReservacionId " + data.ReservacionId;
        return response;
      }

      var TransaccionsResult = await _context.Transaccions
          .Where(x => x.Idtransaccion == ViajesResult.TransaccionIdtransaccion)
          .FirstOrDefaultAsync();

      if (TransaccionsResult == null)
      {
        //Ocurrió un error al generar token 
        response.CodigoMensaje = "400";
        response.Mensaje = "No se encontro información de la Transacción";
        return response;
      }
      else
      {
        if (!string.IsNullOrEmpty(TransaccionsResult.Numeroordenpay))
        {
          order_id = TransaccionsResult.Numeroordenpay;
        }
      }
      if (TransaccionsResult.EstatustransaccionIdestatustransaccion == Convert.ToInt32(EnumEstatusPago.COMPLETED) || TransaccionsResult.EstatustransaccionIdestatustransaccion == Convert.ToInt32(EnumEstatusPago.AUTHORIZED))
      {
        response.CodigoMensaje = "400";
        response.Mensaje = "Ya existe un pago con estatus COMPLETED para este viaje";
        return response;
      }
      #endregion

      #region Región para generar token
      string tokenPayPal = string.Empty;
      PayPalAccessTokenResponse responseGeneracionToken = null;
      responseGeneracionToken = await _payPalService.GenerarTokenPayPal(userId, _payPalService.ArmaObjetoRequestTokenPayPal("client_credentials"));
      if (responseGeneracionToken.CodigoError != "00")
      {
        //Ocurrió un error al generar token 
        response.CodigoMensaje = "400";
        response.Mensaje = responseGeneracionToken.Error;
        return response;
      }
      //Respuesta Correcta de Token
      tokenPayPal = responseGeneracionToken.Access_token;
      #endregion

      #region Región para generar consumir el ws para validar el estatus del pago
      OrdersCaptureResponse responseOrdersCapture = new OrdersCaptureResponse();
      OrdersCaptureResponseWS OrdersCaptureResponseWS = new OrdersCaptureResponseWS();

      //Consumimos WS para validar la orden "pago"
      responseOrdersCapture = await _payPalService.OrdersCapture(tokenPayPal, order_id);
      if (responseOrdersCapture.CodigoError != "00")
      {
        //Ocurrió un error 
        response.CodigoMensaje = "400";
        response.Mensaje = responseOrdersCapture.Error;
        return response;
      }
      else
      {
        if (OrdersCaptureResponseWS != null)
        {
          OrdersCaptureResponseWS = responseOrdersCapture.OrderCapture;
          EstatusOrden = OrdersCaptureResponseWS.status;
          int EstatusPago = Convert.ToInt32(EnumEstatusPago.IN_PROGRESS);//Seteamos por default el estatus en Progreso 
          switch (EstatusOrden.ToUpper())
          {
            case "COMPLETED":
              EstatusPago = Convert.ToInt32(EnumEstatusPago.COMPLETED);
              break;
          }
          #region Región para ligar Transaccion con la tabla de Viaje
          if (EstatusPago == Convert.ToInt32(EnumEstatusPago.COMPLETED))
          {
            //await CambiaEstatusViaje(data.ReservacionId, (sbyte)EnumEstatusViaje.EN_ESPERA);
            //await ActualizaOcupacion(data.ReservacionId);
            await AsignaIdTransaccionAViajeParada(data.ReservacionId, TransaccionsResult.Idtransaccion, (sbyte)EnumEstatusViaje.EN_ESPERA);
          }
          #endregion

          //Respuesta Correcta
          #region Región para guardar registro del Pago en la tabla de Transaccion de la BD
          if (EstatusPago == Convert.ToInt32(EnumEstatusPago.COMPLETED))
          {
            TransaccionsResult.EstatustransaccionIdestatustransaccion = EstatusPago;
            await _context.SaveChangesAsync();
          }
          #endregion
        }
        else
        {
          //Ocurrió un error al generar token 
          response.CodigoMensaje = "400";
          response.Mensaje = "Sin información de estatus de pago ";
          return response;
        }
      }

      result.estatus = EstatusOrden;
      response.Mensaje = "Solicitud exitosa";
      response.CodigoMensaje = "200";
      response.Data = result;
      #endregion

      return response;
    }

    public async Task<Transaccion?> ObtenInfoTransaccionOrdenPeticion(string OrdenId, string PeticionId)
    {
      return await _context.Transaccions.Where(x => x.Numeroordenpay == OrdenId && x.Numeropeticionpay == PeticionId).FirstOrDefaultAsync();
    }

    public async Task<decimal> ObtenTarifa(int userId, string emailUser, int rutaId, int? corridaId, int estacionAbordajeId, int estacionDescensoId, string codigoDescuento, bool aplicaPromocion = false)
    {
      //La bandera "aplicaPromocion" indica si se regresa el total del costo de la corrida sin aplicar ningún tipo de descuento 
      decimal tarifa = 0;

      //DETERMINAMOS TIPO DE TARIFA
      var rutaResult = await (from a in _context.Ruta
                              where a.Idruta == rutaId
                              select a).FirstOrDefaultAsync();

      if (rutaResult.TipotarifaIdtipotarifa == Convert.ToInt32(EnumTipoTarifa.FIJA))
      {
        var tarifaFijaResult = await (from a in _context.TarifaGenerals
                                      where a.RutaIdruta == rutaResult.Idruta
                                      && a.EmpresaIdempresa == rutaResult.EmpresaIdempresa
                                      select a).FirstOrDefaultAsync();
        if (tarifaFijaResult != null)
        {
          tarifa = tarifaFijaResult.Precio ?? 0;
        }
      }
      else if (rutaResult.TipotarifaIdtipotarifa == Convert.ToInt32(EnumTipoTarifa.ESCALONADA))
      {
        var tarifaEscalonadaResult = await (from a in _context.TarifaEscalonada
                                            where a.RutaIdruta == rutaResult.Idruta
                                            && a.EmpresaIdempresa == rutaResult.EmpresaIdempresa
                                            && a.ParadaIdparada == estacionAbordajeId
                                            && a.ParadaIdparada1 == estacionDescensoId
                                            select a).FirstOrDefaultAsync();
        if (tarifaEscalonadaResult != null)
        {
          tarifa = tarifaEscalonadaResult.Precio ?? 0;
        }
      }

      decimal total = tarifa;

      if (aplicaPromocion)
      {

        total = await AplicaPromocion(tarifa, userId, emailUser, rutaId, corridaId, codigoDescuento);
      }


      return Math.Round(total, 2);
    }

    public async Task<decimal> AplicaPromocion(decimal tarifa, int userId, string emailUser, int rutaId, int? corridaId, string codigoDescuento)
    {
      decimal descuento = 0;
      int tipoDescuento = Convert.ToInt32(EnumTipoDescuento.Cantidad);
      decimal cantidadDescuento = 0;
      bool aplicoCodigoDescuento = false;

      //BUSCAMOS CODIGO DE DESCUENTO SI ES QUE NOS LO ENVIAN
      if (!String.IsNullOrEmpty(codigoDescuento))
      {
        var codigoDescuentoResult = await (from a in _context.Codigodescuentos
                                           where a.Codigo == codigoDescuento
                                           && a.Activo == Convert.ToUInt64(true)
                                           && DateTime.Now >= a.Vigenciadesde && DateTime.Now <= a.Vigenciahasta
                                           select a).FirstOrDefaultAsync();

        if (codigoDescuentoResult != null)
        {
          if (codigoDescuentoResult.TipocodigodescuentoIdtipocodigodescuento == Convert.ToInt32(EnumTipoCodigo.Exclisivo))
          {
            var codigoExclusivoResult = await (from a in _context.Codigocorreos
                                               where a.Email == emailUser
                                               select a).FirstOrDefaultAsync();

            if (codigoExclusivoResult != null)
            {
              tipoDescuento = codigoDescuentoResult.TipodescuentoIdtipodescuento1;
              cantidadDescuento = codigoDescuentoResult.Cantidad ?? 0;
              aplicoCodigoDescuento = true;
            }
          }
          else
          {
            tipoDescuento = codigoDescuentoResult.TipodescuentoIdtipodescuento1;
            cantidadDescuento = codigoDescuentoResult.Cantidad ?? 0;
            aplicoCodigoDescuento = true;
          }
        }
      }

      //SI NO SE APLICO CODIGO DE DESCUENTO SE BUSCA MEMBRESIA O PROMOCIONES
      if (!aplicoCodigoDescuento)
      {
        //BUSCAMOS MEMBRESIA
        var membresiaResult = await (from a in _context.Membresia
                                     where a.UsuarioIdusuario == userId
                                     && a.Hasta >= DateTime.Now
                                     select a).FirstOrDefaultAsync();

        if (membresiaResult != null)
        {
          var tipoDescuentoResult = await (from a in _context.Variables
                                           where a.Idvariable == Convert.ToInt32(Models.ViewModel.Enums.EnumVariable.TDM)
                                           select a).FirstOrDefaultAsync();

          var cantidadDescuentoResult = await (from a in _context.Variables
                                               where a.Idvariable == Convert.ToInt32(Models.ViewModel.Enums.EnumVariable.CDM)
                                               select a).FirstOrDefaultAsync();

          if (tipoDescuentoResult != null)
          {
            if (tipoDescuentoResult.TipovariableIdtipovariable == Convert.ToInt32(EnumTipoVariable.Global))
            {
              var tipoDescuentoGlobalResult = await (from a in _context.Variableglobals
                                                     where a.VariableIdvariable == tipoDescuentoResult.Idvariable
                                                     select a).FirstOrDefaultAsync();
              if (tipoDescuentoGlobalResult != null)
              {
                tipoDescuento = tipoDescuentoGlobalResult.Valor == "%" ? Convert.ToInt32(EnumTipoDescuento.Porcentaje) : Convert.ToInt32(EnumTipoDescuento.Cantidad);

              }
            }
          }

          if (cantidadDescuentoResult != null)
          {
            if (cantidadDescuentoResult.TipovariableIdtipovariable == Convert.ToInt32(EnumTipoVariable.Global))
            {
              var descuentoGlobalResult = await (from a in _context.Variableglobals
                                                 where a.VariableIdvariable == cantidadDescuentoResult.Idvariable
                                                 select a).FirstOrDefaultAsync();

              if (descuentoGlobalResult != null)
              {
                cantidadDescuento = Convert.ToDecimal(descuentoGlobalResult.Valor);
              }
            }
          }
        }
        else
        {
          bool terminar = false;
          int prioridadPromocion = 1;
          do
          {
            switch (prioridadPromocion)
            {
              case 1: // CORRIDA
                if (corridaId != null)
                {
                  var pCorridaResult = await (from a in _context.Ruta
                                              join b in _context.Promocions on a.EmpresaIdempresa equals b.EmpresaIdempresa
                                              join c in _context.PromocionCorrida on b.Idpromocion equals c.PromocionIdpromocion
                                              where a.Idruta == rutaId
                                              && b.TipopromocionIdtipopromocion == Convert.ToInt32(EnumTipoPromocion.CORRIDA)
                                              && c.CorridaIdcorrida == corridaId
                                              && DateTime.Now.Date >= b.Vigenciadesde!.Value.Date && DateTime.Now.Date <= b.Vigenciahasta!.Value.Date
                                              select b).FirstOrDefaultAsync();

                  if (pCorridaResult != null)
                  {
                    terminar = true;
                    tipoDescuento = pCorridaResult.TipodescuentoIdtipodescuento;
                    cantidadDescuento = pCorridaResult.Cantidad ?? 0;
                  }
                }
                break;
              case 2: // RUTA
                var pRutaResult = await (from a in _context.Ruta
                                         join b in _context.Promocions on a.EmpresaIdempresa equals b.EmpresaIdempresa
                                         join c in _context.PromocionRuta on b.Idpromocion equals c.PromocionIdpromocion
                                         where a.Idruta == rutaId
                                         && c.RutaIdruta == rutaId
                                         && b.TipopromocionIdtipopromocion == Convert.ToInt32(EnumTipoPromocion.RUTA)
                                         && c.Activo == Convert.ToUInt64(true)
                                         && DateTime.Now.Date >= b.Vigenciadesde!.Value.Date && DateTime.Now.Date <= b.Vigenciahasta!.Value.Date
                                         select b).FirstOrDefaultAsync();

                if (pRutaResult != null)
                {
                  terminar = true;
                  tipoDescuento = pRutaResult.TipodescuentoIdtipodescuento;
                  cantidadDescuento = pRutaResult.Cantidad ?? 0;
                }

                break;

              case 3: // HORARIO
                if (corridaId != null)
                {
                  var corridaResult = await (from a in _context.Corrida
                                             where a.Idcorrida == corridaId
                                             select a).FirstOrDefaultAsync();

                  var pHorarioResult = await (from a in _context.Ruta
                                              join b in _context.Promocions on a.EmpresaIdempresa equals b.EmpresaIdempresa
                                              join c in _context.PromocionHorarios on b.Idpromocion equals c.PromocionIdpromocion
                                              where a.Idruta == rutaId
                                              && b.TipopromocionIdtipopromocion == Convert.ToInt32(EnumTipoPromocion.HORARIO)
                                              && corridaResult.HoraInicio >= c.Horadesde && corridaResult.HoraFin <= c.Horahasta
                                            && DateTime.Now.Date >= b.Vigenciadesde!.Value.Date && DateTime.Now.Date <= b.Vigenciahasta!.Value.Date
                                              select b).FirstOrDefaultAsync();

                  if (pHorarioResult != null)
                  {
                    terminar = true;
                    tipoDescuento = pHorarioResult.TipodescuentoIdtipodescuento;
                    cantidadDescuento = pHorarioResult.Cantidad ?? 0;
                  }
                }
                break;
              case 4: // EMPRESA
                var pEmpresaResult = await (from a in _context.Ruta
                                            join b in _context.Promocions on a.EmpresaIdempresa equals b.EmpresaIdempresa
                                            //join c in _context.PromocionEmpresas on b.Idpromocion equals c.PromocionIdpromocion
                                            where a.Idruta == rutaId
                                            //&& c.EmpresaIdempresa == a.EmpresaIdempresa
                                            && b.TipopromocionIdtipopromocion == Convert.ToInt32(EnumTipoPromocion.EMPRESA)
                                            //&& c.Activo == Convert.ToUInt64(true)
                                            && DateTime.Now.Date >= b.Vigenciadesde!.Value.Date && DateTime.Now.Date <= b.Vigenciahasta!.Value.Date
                                            select b).FirstOrDefaultAsync();

                if (pEmpresaResult != null)
                {
                  terminar = true;
                  tipoDescuento = pEmpresaResult.TipodescuentoIdtipodescuento;
                  cantidadDescuento = pEmpresaResult.Cantidad ?? 0;
                }

                break;

            }

            prioridadPromocion++;

            if (prioridadPromocion > 4)
            {
              terminar = true;
            }
          }
          while (!terminar);

        }
      }

      if (tipoDescuento == Convert.ToInt32(EnumTipoDescuento.Porcentaje))
      {
        if (cantidadDescuento >= 1)
        {
          cantidadDescuento = cantidadDescuento / 100;
        }
        descuento = tarifa * cantidadDescuento;
      }
      else
      {
        descuento = cantidadDescuento;
      }

      //return (tarifa - descuento);
      var total = Math.Round(tarifa - descuento, 2);
      return total <= 0 ? 0 : total;
    }
    #endregion


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


    public async Task<bool> ActualizaEstatusTransaccion(int TransaccionId, int EstatusTransaccionId)
    {
      bool estatus = false;
      Transaccion? TransaccionEntity = await _context.Transaccions
      .Where(x => x.Idtransaccion == TransaccionId)
      .FirstOrDefaultAsync();
      if (TransaccionEntity != null)
      {
        TransaccionEntity.EstatustransaccionIdestatustransaccion = EstatusTransaccionId;
        _context.SaveChanges();
        estatus = true;
      }
      return estatus;
    }

    public async Task ActualizaCorridaParadaAsignacion(int reservacionId)
    {

      Viaje? viaje = await _context.Viajes.FirstOrDefaultAsync(x => x.Idviaje == reservacionId);

      if (viaje != null)
      {
        CorridaAsignacionParadum? capInicio = await _context.CorridaAsignacionParada.FirstOrDefaultAsync(x =>
                                                                        x.CorridaAsignacionIdcorridaAsignacion == viaje.CorridaAsignacionIdcorridaAsignacion
                                                                     && x.ParadaIdparada == viaje.ParadaInicio);

        if (capInicio != null)
        {
          capInicio.Suben += viaje.Numeropasajeros;
          //capInicio.Espacios -= capInicio.Suben;

          _context.CorridaAsignacionParada.Update(capInicio);
          await _context.SaveChangesAsync();
        }

        CorridaAsignacionParadum? capTermino = await _context.CorridaAsignacionParada.FirstOrDefaultAsync(x =>
                                                        x.CorridaAsignacionIdcorridaAsignacion == viaje.CorridaAsignacionIdcorridaAsignacion
                                                     && x.ParadaIdparada == viaje.ParadaFin);

        if (capTermino != null)
        {
          capTermino.Bajan += viaje.Numeropasajeros;
          //capInicio.Espacios += capTermino.Bajan;

          _context.CorridaAsignacionParada.Update(capTermino);
          await _context.SaveChangesAsync();
        }



      }

    }

    private int OrdenParada(int idViaje, int idParada)
    {
      int? parada = (from a in _context.Viajes
                     join b in _context.CorridaAsignacions on a.CorridaAsignacionIdcorridaAsignacion equals b.IdcorridaAsignacion
                     join c in _context.Corrida on b.CorridaIdcorrida equals c.Idcorrida
                     join d in _context.Ruta on c.RutaIdruta equals d.Idruta
                     join e in _context.RutaParada on d.Idruta equals e.RutaIdruta
                     where a.Idviaje == idViaje && e.ParadaIdparada == idParada
                     orderby e.Orden
                     select e.Orden).FirstOrDefault();

      return parada ?? 0;
    }

    public async Task ActualizaOcupacion(int reservacionId, bool cancelaOcupacion = false)
    {
      Viaje? ViajeEntity = await _context.Viajes.FirstOrDefaultAsync(x => x.Idviaje == reservacionId);

      if (ViajeEntity != null)
      {
        var idruta = await (from a in _context.CorridaAsignacions
                            join b in _context.Corrida on a.CorridaIdcorrida equals b.Idcorrida
                            where a.IdcorridaAsignacion == ViajeEntity.CorridaAsignacionIdcorridaAsignacion
                            select b.RutaIdruta).FirstOrDefaultAsync();

        var corridaAsignacionParadaResult = await (from a in _context.CorridaAsignacions
                                                   join b in _context.CorridaAsignacionParada on a.IdcorridaAsignacion equals b.CorridaAsignacionIdcorridaAsignacion
                                                   join c in _context.RutaParada on b.ParadaIdparada equals c.ParadaIdparada
                                                   where a.IdcorridaAsignacion == ViajeEntity.CorridaAsignacionIdcorridaAsignacion
                                                   && c.RutaIdruta == idruta
                                                   && c.Orden >= (from a in _context.RutaParada where a.RutaIdruta == idruta && a.ParadaIdparada == ViajeEntity.ParadaInicio select a.Orden).FirstOrDefault()
                                                   && c.Orden < (from a in _context.RutaParada where a.RutaIdruta == idruta && a.ParadaIdparada == ViajeEntity.ParadaFin select a.Orden).FirstOrDefault()
                                                   select b).ToListAsync();

        foreach (var parada in corridaAsignacionParadaResult)
        {
          if (cancelaOcupacion)
          {
            parada.Espacios += ViajeEntity.Numeropasajeros;
            // parada.Suben = parada.Suben - ViajeEntity.Numeropasajeros;
            //parada.Bajan = parada.Bajan - ViajeEntity.Numeropasajeros;
          }
          else
          {
            parada.Espacios -= ViajeEntity.Numeropasajeros;
          }

        }

        await _context.SaveChangesAsync();

        if (cancelaOcupacion)
        {
          await ActualizarParadaSubenBajan(ViajeEntity);

        }
      }
    }

    private async Task ActualizarParadaSubenBajan(Viaje viaje)
    {
      CorridaAsignacionParadum? capInicio = await _context.CorridaAsignacionParada.FirstOrDefaultAsync(x =>
                                                                        x.CorridaAsignacionIdcorridaAsignacion == viaje.CorridaAsignacionIdcorridaAsignacion
                                                                     && x.ParadaIdparada == viaje.ParadaInicio);

      if (capInicio is not null)
      {
        capInicio.Suben -= viaje.Numeropasajeros;
        //capInicio.Espacios -= capInicio.Suben;

        _context.CorridaAsignacionParada.Update(capInicio);
        await _context.SaveChangesAsync();
      }

      CorridaAsignacionParadum? capTermino = await _context.CorridaAsignacionParada.FirstOrDefaultAsync(x =>
                                                      x.CorridaAsignacionIdcorridaAsignacion == viaje.CorridaAsignacionIdcorridaAsignacion
                                                   && x.ParadaIdparada == viaje.ParadaFin);

      if (capTermino is not null)
      {
        capTermino.Bajan -= viaje.Numeropasajeros;
        //capInicio.Espacios += capTermino.Bajan;

        _context.CorridaAsignacionParada.Update(capTermino);
        await _context.SaveChangesAsync();
      }
    }

    private async Task AsignaIdTransaccionAViajeEstatus(int reservacionId, int Idtransaccion, sbyte EstatusViaje)
    {
      Viaje? ViajeEntity = await _context.Viajes
      .Where(x => x.Idviaje == reservacionId)
      .FirstOrDefaultAsync();
      if (ViajeEntity != null)
      {
        ViajeEntity.EstatusviajeIdestatusviaje = EstatusViaje;
        ViajeEntity.TransaccionIdtransaccion = Idtransaccion;
        await _context.SaveChangesAsync();

        //await ActualizaCorridaParadaAsignacion(reservacionId);
      }
    }


    private async Task AsignaIdTransaccionAViajeParada(int reservacionId, int Idtransaccion, sbyte EstatusViaje)
    {
      Viaje? ViajeEntity = await _context.Viajes
      .FirstOrDefaultAsync(x => x.Idviaje == reservacionId);

      if (ViajeEntity != null)
      {
        ViajeEntity.EstatusviajeIdestatusviaje = EstatusViaje;
        if (Idtransaccion != 0)
        {
          ViajeEntity.TransaccionIdtransaccion = Idtransaccion;
        }


        await _context.SaveChangesAsync();

        await ActualizaAsignacionParada(reservacionId);
      }
    }


    private async Task ActualizaAsignacionParada(int reservacionId)
    {
      var viaje = await _context.Viajes
                                .Include(v => v.CorridaAsignacionIdcorridaAsignacionNavigation)
                                .FirstOrDefaultAsync(v => v.Idviaje == reservacionId);

      if (viaje != null)
      {
        var corridaAsignacionId = viaje.CorridaAsignacionIdcorridaAsignacion;
        var paradaInicio = viaje.ParadaInicio;
        var paradaFin = viaje.ParadaFin;
        var numPasajeros = viaje.Numeropasajeros;

        var idRuta = await _context.CorridaAsignacions
                                   .Where(a => a.IdcorridaAsignacion == corridaAsignacionId)
                                   .Select(a => a.CorridaIdcorridaNavigation.RutaIdruta)
                                   .FirstOrDefaultAsync();

        var rutaParadaInicio = await _context.RutaParada
                                             .Where(rp => rp.RutaIdruta == idRuta && rp.ParadaIdparada == paradaInicio)
                                             .Select(rp => rp.Orden)
                                             .FirstOrDefaultAsync();

        var rutaParadaFin = await _context.RutaParada
                                          .Where(rp => rp.RutaIdruta == idRuta && rp.ParadaIdparada == paradaFin)
                                          .Select(rp => rp.Orden)
                                          .FirstOrDefaultAsync();

        var corridaAsignacionParadas = await _context.CorridaAsignacionParada
                                                     .Where(cap => cap.CorridaAsignacionIdcorridaAsignacion == corridaAsignacionId)
                                                     .ToListAsync();

        var capInicio = corridaAsignacionParadas.FirstOrDefault(cap => cap.ParadaIdparada == paradaInicio);
        var capTermino = corridaAsignacionParadas.FirstOrDefault(cap => cap.ParadaIdparada == paradaFin);

        var corridaAsignacionParadaResult = corridaAsignacionParadas
                                            .Where(cap => _context.RutaParada.Any(rp => rp.ParadaIdparada == cap.ParadaIdparada
                                                                                        && rp.RutaIdruta == idRuta
                                                                                        && rp.Orden >= rutaParadaInicio
                                                                                        && rp.Orden < rutaParadaFin))
                                            .ToList();

        if (capInicio != null)
        {
          capInicio.Suben += numPasajeros;
          _context.CorridaAsignacionParada.Update(capInicio);
        }

        if (capTermino != null)
        {
          capTermino.Bajan += numPasajeros;
          _context.CorridaAsignacionParada.Update(capTermino);
        }

        foreach (var parada in corridaAsignacionParadaResult)
        {
          parada.Espacios -= numPasajeros;
        }

        await _context.SaveChangesAsync();
      }
    }


  }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.AppSettingsModels;
using SUVAN.BackOffice.Models.ManejoRespuesta;
using SUVAN.BackOffice.Models.Monedero;
using SUVAN.BackOffice.Models.Pago;
using SUVAN.BackOffice.Models.PayPal.Pago;
using SUVAN.BackOffice.Models.PayPal.Token;
using SUVAN.BackOffice.Models.UnlimintPay.Pago;
using SUVAN.BackOffice.Models.UnlimintPay.Token;
using SUVAN.BackOffice.Models.ViewModel.Enums;
using SUVAN.BackOffice.Service.Pago;
using SUVAN.BackOffice.Service.PayPal;
using SUVAN.BackOffice.Service.RegistroUsuario;
using SUVAN.BackOffice.Service.UnlimintPay;
using static SUVAN.BackOffice.Models.UnlimintPay.Pago.RespuestaValidacionPagoWS;
using static SUVAN.BackOffice.Service.Pago.PagoService;

namespace SUVAN.BackOffice.Service.Monedero
{
    public class MonederoService : IMonederoService
    {
        private readonly SuvanDbContext _context;
        private readonly UnlimitPaySettingsOptions _unlimitPaySettingsOptions;

        private readonly IUnlimintPayService _unlimintPayService;
        private readonly IUsuarioService _usuarioService;
        private readonly IPagoService _pagoService;
        private readonly IPayPalService _payPalService;

        public MonederoService(SuvanDbContext context, IUsuarioService usuarioService, IUnlimintPayService unlimintPayService, IOptions<UnlimitPaySettingsOptions> unlimitPaySettingsOptions, IPagoService pagoService, IPayPalService payPalService)
        {
            _context = context;
            _usuarioService = usuarioService;
            _unlimintPayService = unlimintPayService;
            _unlimitPaySettingsOptions = unlimitPaySettingsOptions.Value;
            _pagoService = pagoService;
            _payPalService = payPalService;
        }

        public async Task<SuVanResponse<MonederoSaldoResponse>> ObtenMonedero(int userId)
        {
            SuVanResponse<MonederoSaldoResponse> response = new();

            MonederoSaldoResponse? result = await (from o in _context.Monederos
                                                   where o.UsuarioIdusuario == userId
                                                   select new MonederoSaldoResponse()
                                                   {
                                                       Saldo = o.Saldo ?? 0
                                                   }).FirstOrDefaultAsync();


            response.Data = result == null ? new MonederoSaldoResponse() { Saldo = 0 } : result;

            response.CodigoMensaje = "200";
            response.Mensaje = "Búsqueda exitosa";

            return response;
        }

        #region Metodos para Recarga de Monedero con "Tarjeta de Credito o Debito"
        /// <summary>
        /// Metodo para Recargar Monedero con Pago con "Tarjeta de Credito o Debito"
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="data"></param>
        /// <param name="emailUser"></param>
        /// <returns></returns>
        public async Task<SuVanResponse<RecargaMonederoResponse>> RecargaMonederoConTarjeta(int userId, RecargaMonederoRequest data, string emailUser)
        {
            RecargaMonederoResponse? result = new RecargaMonederoResponse();
            SuVanResponse<RecargaMonederoResponse> response = new();
            decimal Monto = 0;

            #region Validamos datos de entrada
            if (data.Cantidad <= 0)
            {
                //Ocurrió un error al generar token 
                response.CodigoMensaje = "400";
                response.Mensaje = "La cantidad debe ser mayor a 0";
                return response;
            }
            else
            {
                Monto = data.Cantidad;
            }
            //Tipo transaccion "Pago o Recarga"
            var tipoTransaccionResult = await _context.Tipotransaccions
                .Where(x => x.Idtipotransaccion == data.TipoTransaccionId)
                .FirstOrDefaultAsync();

            if (tipoTransaccionResult == null || tipoTransaccionResult.Idtipotransaccion != Convert.ToInt32(EnumTipoTransaccion.RECARGA))
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

            if (tipoMetodoPagoResult == null)
            {
                response.CodigoMensaje = "400";
                response.Mensaje = "Tipo de método de pago incorrecto";
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
            Transaccion entity = await _pagoService.GuardaTransaccion(userId, data.MetodoPagoId, data.TipoTransaccionId, Monto, gOrdenId.ToString(), gPeticionId.ToString(), null, null);
            int Idtransaccion = entity.Idtransaccion;
            #endregion

            result.OrdenId = gOrdenId.ToString();
            result.PeticionId = gPeticionId.ToString();
            result.url = responseGeneracionPago.RedirectUrl;
            response.Mensaje = "Solicitud exitosa";
            response.CodigoMensaje = "200";
            response.Data = result;
            #endregion

            response.CodigoMensaje = "200";
            response.Mensaje = "Solicitud exitosa";

            return response;
        }

        /// <summary>
        /// Metodo para confirmar la recarga(Pago) de monedero con "Tarjeta de Credito o Debito"
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<SuVanResponse<ConfirmaRecargaMonederoResponse>> ConfirmaRecargaMonederoConTarjeta(int userId, ConfirmaRecargaMonederoRequest data)
        {
            ConfirmaRecargaMonederoResponse? result = new ConfirmaRecargaMonederoResponse();
            SuVanResponse<ConfirmaRecargaMonederoResponse> response = new();
            decimal SaldoFinal = 0;

            #region Validamos datos de entrada
            var TransaccionsResult = await _pagoService.ObtenInfoTransaccionOrdenPeticion(data.OrdenId, data.PeticionId);

            if (TransaccionsResult == null)
            {
                response.CodigoMensaje = "400";
                response.Mensaje = "No se encontro información de la Transacción";
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
            int IntentosPermitidos = 2;
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
                //Respuesta Correcta
                int EstatusPago = Convert.ToInt32(EnumEstatusPago.IN_PROGRESS);//Seteamos por default el estatus en Progreso 
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
                #region Actualizamos Estatus de la Transacción
                bool estatusUdt = await _pagoService.ActualizaEstatusTransaccion(TransaccionsResult.Idtransaccion, EstatusPago);
                #endregion

                #region Actualizamos Saldo siempre y cuando los estatus sean "COMPLETED" o "AUTHORIZED"
                if (EstatusPago == Convert.ToInt32(EnumEstatusPago.COMPLETED) || EstatusPago == Convert.ToInt32(EnumEstatusPago.AUTHORIZED))
                {
                    SaldoFinal = (decimal)await ActualizaSaldoMonedero(userId, data.OrdenId, data.PeticionId, EstatusPago);
                }
                #endregion
                result.estatus = responseValidacionPago.DatosPago.data[0].payment_data.status.ToUpper();
                result.Saldo = SaldoFinal;
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

        #region Metodos para Recarga de Monedero con "PayPal"
        /// <summary>
        /// /// Metodo para Recargar Monedero con Pago con "PayPal"
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="data"></param>
        /// <param name="emailUser"></param>
        /// <returns></returns>
        public async Task<SuVanResponse<RecargaMonederoResponse>> RecargaMonederoConPayPal(int userId, RecargaMonederoRequest data, string emailUser)
        {
            RecargaMonederoResponse? result = new RecargaMonederoResponse();
            SuVanResponse<RecargaMonederoResponse> response = new();
            decimal Monto = 0;
            string UrlRedirect = string.Empty;

            #region Validamos datos de entrada
            if (data.Cantidad <= 0)
            {
                //Ocurrió un error al generar token 
                response.CodigoMensaje = "400";
                response.Mensaje = "La cantidad debe ser mayor a 0";
                return response;
            }
            else
            {
                Monto = data.Cantidad;
            }
            //Tipo transaccion "Pago o Recarga"
            var tipoTransaccionResult = await _context.Tipotransaccions
                .Where(x => x.Idtipotransaccion == data.TipoTransaccionId)
                .FirstOrDefaultAsync();

            if (tipoTransaccionResult == null || tipoTransaccionResult.Idtipotransaccion != Convert.ToInt32(EnumTipoTransaccion.RECARGA))
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

            Guid gPeticionId = Guid.NewGuid();
            string gOrdenId = string.Empty;
            #region Región para consumir el WS para generar la orden
            CreateOrderResponse responseOrderResponse = null;
            CreateOrderResponseWS responseOrderResponseWS = new CreateOrderResponseWS();
            ///Consumimos WS para generar el pago
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
                    if (item.rel == "approve")
                    {
                        UrlRedirect = item.href;
                    }
                }
                /*Guardamos Información de la Transacción*/
                #region Región para guardar registro del Pago en la tabla de Transaccion de la BD
                Transaccion entity = await _pagoService.GuardaTransaccion(userId, data.MetodoPagoId, data.TipoTransaccionId, Monto, gOrdenId.ToString(), gPeticionId.ToString(), null, null);
                int Idtransaccion = entity.Idtransaccion;
                #endregion

                result.OrdenId = gOrdenId;
                result.PeticionId = gPeticionId.ToString();
                result.url = UrlRedirect;
                response.Mensaje = "Solicitud exitosa";
                response.CodigoMensaje = "200";
                response.Data = result;
            }
            #endregion
            return response;
        }

        /// <summary>
        /// Metodo para confirmar la recarga(Pago) de monedero con "PayPal"
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<SuVanResponse<ConfirmaRecargaMonederoResponse>> ConfirmaRecargaMonederoConPayPal(int userId, ConfirmaRecargaMonederoRequest data)
        {
            ConfirmaRecargaMonederoResponse? result = new ConfirmaRecargaMonederoResponse();
            SuVanResponse<ConfirmaRecargaMonederoResponse> response = new();
            decimal SaldoFinal = 0;
            string EstatusOrden = string.Empty;

            #region Validamos datos de entrada
            var TransaccionsResult = await _pagoService.ObtenInfoTransaccionOrdenPeticion(data.OrdenId, data.PeticionId);

            if (TransaccionsResult == null)
            {
                response.CodigoMensaje = "400";
                response.Mensaje = "No se encontro información de la Transacción";
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

            //Consumimos WS para validar el pago
            responseOrdersCapture = await _payPalService.OrdersCapture(tokenPayPal, data.OrdenId);
            if (responseOrdersCapture.CodigoError != "00")
            {
                //Ocurrió un error 
                response.CodigoMensaje = "400";
                response.Mensaje = responseOrdersCapture.Error;
                return response;
            }
            else
            {
                if (responseOrdersCapture.OrderCapture != null)
                {
                    //Respuesta Correcta
                    EstatusOrden = responseOrdersCapture.OrderCapture.status;
                    int EstatusPago = Convert.ToInt32(EnumEstatusPago.IN_PROGRESS);//Seteamos por default el estatus en Progreso 
                    switch (EstatusOrden.ToUpper())
                    {
                        case "COMPLETED":
                            EstatusPago = Convert.ToInt32(EnumEstatusPago.COMPLETED);
                            break;
                    }

                    #region Actualizamos Estatus de la Transacción
                    bool estatusUdt = await _pagoService.ActualizaEstatusTransaccion(TransaccionsResult.Idtransaccion, EstatusPago);
                    #endregion

                    #region Actualizamos saldo
                    if (EstatusPago == Convert.ToInt32(EnumEstatusPago.COMPLETED))
                    {
                        SaldoFinal = (decimal)await ActualizaSaldoMonedero(userId, data.OrdenId, data.PeticionId, EstatusPago);
                    }
                    #endregion
                    result.estatus = EstatusOrden.ToUpper();
                    result.Saldo = SaldoFinal;
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
            }

            #endregion

            return response;
        }

        public async Task<decimal?> ActualizaSaldoMonedero(int userId, string OrdenId, string PeticionId, int EstatusPago)
        {
            decimal SaldoFinal = 0;
            decimal SaldoMonederoDisponible = 0;

            var TransaccionsResult = await _pagoService.ObtenInfoTransaccionOrdenPeticion(OrdenId, PeticionId);

            if (TransaccionsResult != null)
            {
                //Actualizamos el estatus del Pago Completado
                TransaccionsResult.EstatustransaccionIdestatustransaccion = EstatusPago;
                await _context.SaveChangesAsync();

                //Actualizamos Saldo
                Database.Entities.Monedero? MonederoEntity = await _context.Monederos.Where(x => x.UsuarioIdusuario == userId).FirstOrDefaultAsync();
                if (MonederoEntity == null)
                {
                    SaldoFinal = TransaccionsResult.Cantidad ?? 0;
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
                    SaldoFinal = (SaldoMonederoDisponible + TransaccionsResult.Cantidad ?? 0);
                    MonederoEntity.Saldo = SaldoFinal;//Actualizamos Saldo del Monedero
                    _context.Monederos.Entry(MonederoEntity);
                }
                await _context.SaveChangesAsync();
            }
            return SaldoFinal;
        }
        #endregion



    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.AppSettingsModels;
using SUVAN.BackOffice.Models.ManejoRespuesta;
using SUVAN.BackOffice.Models.Membresia;
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
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;
using static SUVAN.BackOffice.Service.Pago.PagoService;

namespace SUVAN.BackOffice.Service.Membresia
{
    public class MembresiaService : IMembresiaService
    {
        private readonly SuvanDbContext _context;
        private readonly UnlimitPaySettingsOptions _unlimitPaySettingsOptions;

        private readonly IUnlimintPayService _unlimintPayService;
        private readonly IUsuarioService _usuarioService;
        private readonly IPagoService _pagoService;
        private readonly IPayPalService _payPalService;


        public MembresiaService(SuvanDbContext context, IUsuarioService usuarioService, IUnlimintPayService unlimintPayService, IOptions<UnlimitPaySettingsOptions> unlimitPaySettingsOptions, IPagoService pagoService, IPayPalService payPalService)
        {
            _context = context;
            _usuarioService = usuarioService;
            _unlimintPayService = unlimintPayService;
            _unlimitPaySettingsOptions = unlimitPaySettingsOptions.Value;
            _pagoService = pagoService;
            _payPalService = payPalService;
        }

        #region  Metodos para Pago de Memebresia con "Tarjeta de Credito o Debito"
        /// <summary>
        /// Pago de membresia
        /// Metodo  para generar url de Pago
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="data"></param>
        /// <param name="emailUser"></param>
        /// <returns></returns>
        public async Task<SuVanResponse<PagaMembresiaConTarjetaResponse>> PagaMembresiaConTarjeta(int userId, PagaMembresiaConTarjetaRequest data, string emailUser)
        {
            PagaMembresiaConTarjetaResponse? result = new PagaMembresiaConTarjetaResponse();
            SuVanResponse<PagaMembresiaConTarjetaResponse> response = new();
            decimal Monto = 0;

            #region Validamos datos de entrada
            //Tipo transaccion "Pago o Recarga"
            var tipoTransaccionResult = await _context.Tipotransaccions
                .Where(x => x.Idtipotransaccion == data.TipoTransaccionId)
                .FirstOrDefaultAsync();

            if (tipoTransaccionResult == null || tipoTransaccionResult.Idtipotransaccion != Convert.ToInt32(EnumTipoTransaccion.PAGO_MEMBRESIA))
            {
                //Ocurrió un error al generar token 
                response.CodigoMensaje = "400";
                response.Mensaje = "Tipo de transacción incorrecta";
                return response;
            }

            //Tipo Metodo Pago "Tarjeta"
            var tipoMetodoPagoResult = await _context.Metodopagos
                .Where(x => x.Idmetodopago == data.MetodoPagoId)
                .FirstOrDefaultAsync();

            if (tipoMetodoPagoResult == null || tipoMetodoPagoResult.Idmetodopago != Convert.ToInt32(EnumMetodoPago.TARJETA_DEBITO_CREDITO))
            {
                response.CodigoMensaje = "400";
                response.Mensaje = "Tipo de método de pago incorrecto";
                return response;
            }
            #endregion

            Monto = await ObtenMontoVariable(Monto);

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
        /// Metodo  para confirmar el pago de membresia por medio de "Tarjeta de Credito o Debito"
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<SuVanResponse<ConfirmaPagoMembresiaConTarjetaResponse>> ConfirmaPagoMembresiaConTarjeta(int userId, ConfirmaPagoMembresiaConTarjetaRequest data)
        {
            ConfirmaPagoMembresiaConTarjetaResponse? result = new ConfirmaPagoMembresiaConTarjetaResponse();
            SuVanResponse<ConfirmaPagoMembresiaConTarjetaResponse> response = new();

            #region Validamos datos de entrada
            var TransaccionsResult = await _context.Transaccions.Where(x => x.Numeroordenpay == data.OrdenId && x.Numeropeticionpay == data.PeticionId).FirstOrDefaultAsync();

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

                DateTime? desde = DateTime.Now;
                DateTime? hasta = DateTime.Now.AddDays(1);



                //Respuesta Correcta
                #region Región para guardar registro del Pago en la tabla de Transaccion de la BD
                if (EstatusPago == Convert.ToInt32(EnumEstatusPago.COMPLETED) || EstatusPago == Convert.ToInt32(EnumEstatusPago.AUTHORIZED))
                {
                    //Actualizamos el estatus del Pago Completado
                    TransaccionsResult.EstatustransaccionIdestatustransaccion = EstatusPago;
                    await _context.SaveChangesAsync();

                    #region  Insertamos o Actualizamos la vigencia de la membresia
                    hasta = await InsertaActualizaMembresia(userId, TransaccionsResult.Idtransaccion, desde, hasta);
                    #endregion
                }
                else { hasta = null; }
                #endregion

                result.Estatus = responseValidacionPago.DatosPago.data[0].payment_data.status.ToUpper();
                result.Vigencia = hasta == null ? String.Empty : hasta.Value.ToString("dd/MM/yyyy");
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

        #region Metodos para Pago de Memebresia con "Monedero"
        /// <summary>
        /// Pagamos membreia por el monedero
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="data"></param>
        /// <param name="emailUser"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public async Task<SuVanResponse<PagaMembresiaConMonederoResponse>> PagaMembresiaConMonedero(int userId, PagaMembresiaConMonederoRequest data, string emailUser, string code)
        {
            decimal Monto = 0;
            decimal SaldoMonederoDisponible = 0;
            decimal SaldoFinal = 0;
            bool seGeneraOTP = false;

            PagaMembresiaConMonederoResponse? result = new PagaMembresiaConMonederoResponse();
            SuVanResponse<PagaMembresiaConMonederoResponse> response = new();

            #region Validamos datos de entrada
            //Tipo transaccion "Pago o Recarga"
            var tipoTransaccionResult = await _context.Tipotransaccions
                .Where(x => x.Idtipotransaccion == data.TipoTransaccionId)
                .FirstOrDefaultAsync();


            if (tipoTransaccionResult == null || tipoTransaccionResult.Idtipotransaccion != Convert.ToInt32(EnumTipoTransaccion.PAGO_MEMBRESIA))
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

            //Obtenemos el Monto
            Monto = await ObtenMontoVariable(Monto);

            //Revisamos el Saldo del Monedero 
            var SaldoMonederoResult = await _context.Monederos
                .Where(x => x.UsuarioIdusuario == userId)
                .FirstOrDefaultAsync();

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
            SaldoMonederoDisponible = (decimal)SaldoMonederoResult.Saldo;
            #endregion

            Transaccion entity = null;
            DateTime? desde = DateTime.Now;
            DateTime? hasta = DateTime.Now.AddDays(1);
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
                entity = await _pagoService.GuardaTransaccion(userId, data.MetodoPagoId, data.TipoTransaccionId, Monto, null, null, CodigoOTP, Codigoexp);
                #endregion
            }
            else
            {
                int TransaccionIDPagocConMonedero = 0;
                ///Obtenemos la Informacion de la Transacción por medio del código OTP
                entity = await _context.Transaccions.Where(x => x.Codigo == data.opt && x.UsuarioIdusuario == userId && x.TipotransaccionIdtipotransaccion == data.TipoTransaccionId).FirstOrDefaultAsync();
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
                _context.SaveChanges();
                TransaccionIDPagocConMonedero = entity.Idtransaccion;

                //Hasta aqui asumimos que el pago fue correcto y actualizamos estatus de la membresia
                #region  Insertamos o Actualizamos la vigencia de la membresia
                hasta = DateTime.Now.AddYears(1);
                hasta = await InsertaActualizaMembresia(userId, TransaccionIDPagocConMonedero, desde, hasta);
                #endregion
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


            //Actualizamos Saldo de Monedero
            SUVAN.BackOffice.Database.Entities.Monedero? MonederoEntity = await _context.Monederos
            .Where(x => x.UsuarioIdusuario == userId)
            .FirstOrDefaultAsync();
            if (MonederoEntity != null)
            {
                SaldoFinal = SaldoMonederoDisponible - Monto;//Restamos el saldo del Monedero
                MonederoEntity.Saldo = SaldoFinal;
                _context.SaveChanges();
            }

            //result.Estatus = responseValidacionPago.DatosPago.data[0].payment_data.status.ToUpper();
            result.Estatus = "COMPLETED";//Para este tipo de pago por MONEDERO se pone como Completado
            result.Vigencia = hasta == null ? String.Empty : hasta.Value.ToString("dd/MM/yyyy");
            result.saldomonederoactualizado = SaldoFinal.ToString();
            response.Mensaje = "Solicitud exitosa";
            response.CodigoMensaje = "200";
            response.Data = result;

            return response;
        }
        #endregion

        private async Task<DateTime?> InsertaActualizaMembresia(int userId, int TransaccionId, DateTime? desde, DateTime? hasta)
        {

			string VIGENCIA = _context.Variableglobals.FirstOrDefault(o => o.VariableIdvariable == 10).Valor;


			//[{ "id": "Q","valor": "Quincenal"},{ "id": "M","valor": "Mensual"},{ "id":"A","valor": "Anual"}]

			switch (VIGENCIA)
			{
				case "Q":
					hasta = desde.Value.AddDays(15);
					break;
				case "M":
					hasta = desde.Value.AddMonths(1);
					break;
				case "A":
					hasta = desde.Value.AddYears(1);
					break;
			}

			Database.Entities.Membresium? MembresiaEntity = await _context.Membresia.Where(x => x.UsuarioIdusuario == userId).FirstOrDefaultAsync();
            if (MembresiaEntity == null)
            {



                //Insertamos Saldo
                var MembresiaEntityInsert = new Database.Entities.Membresium()
                {
                    UsuarioIdusuario = userId,
                    Desde = desde,
                    Hasta = hasta,
                    TransaccionIdtransaccion = TransaccionId,

                };
                _context.Membresia.Add(MembresiaEntityInsert);
            }
            else
            {
                if (MembresiaEntity.Hasta > desde)
                {
					switch (VIGENCIA)
					{
						case "Q":
							hasta = MembresiaEntity.Hasta.Value.AddDays(15);
							break;
						case "M":
							hasta = MembresiaEntity.Hasta.Value.AddMonths(1);
							break;
						case "A":
							hasta = MembresiaEntity.Hasta.Value.AddYears(1);
							break;
					}
                }
                MembresiaEntity.TransaccionIdtransaccion = TransaccionId;
                MembresiaEntity.Desde = desde;
                MembresiaEntity.Hasta = hasta;
                _context.Membresia.Entry(MembresiaEntity);
            }
            await _context.SaveChangesAsync();
            return hasta;
        }


        #region  Metodos para Pago de Membresia con "PayPal"
        /// <summary>
        /// Pago de membresia con "PayPal"
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="data"></param>
        /// <param name="emailUser"></param>
        /// <returns></returns>
        public async Task<SuVanResponse<PagaMembresiaConPayPal>> PagaMembresiaConPayPal(int userId, PagaMembresiaConPayPalRequest data, string emailUser)
        {
            PagaMembresiaConPayPal? result = new PagaMembresiaConPayPal();
            SuVanResponse<PagaMembresiaConPayPal> response = new();
            decimal Monto = 0;
            string UrlRedirect = string.Empty;

            #region Validamos datos de entrada
            //Tipo transaccion "Pago o Recarga"
            var tipoTransaccionResult = await _context.Tipotransaccions
                .Where(x => x.Idtipotransaccion == data.TipoTransaccionId)
                .FirstOrDefaultAsync();

            if (tipoTransaccionResult == null || tipoTransaccionResult.Idtipotransaccion != Convert.ToInt32(EnumTipoTransaccion.PAGO_MEMBRESIA))
            {
                //Ocurrió un error al generar token 
                response.CodigoMensaje = "400";
                response.Mensaje = "Tipo de transacción incorrecta";
                return response;
            }

            //Tipo Metodo Pago "Tarjeta"
            var tipoMetodoPagoResult = await _context.Metodopagos
                .Where(x => x.Idmetodopago == data.MetodoPagoId)
                .FirstOrDefaultAsync();

            if (tipoMetodoPagoResult == null || tipoMetodoPagoResult.Idmetodopago != Convert.ToInt32(EnumMetodoPago.PAYPAL))
            {
                response.CodigoMensaje = "400";
                response.Mensaje = "Tipo de método de pago incorrecto";
                return response;
            }
            #endregion

            Monto = await ObtenMontoVariable(Monto);

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
                responseOrderResponseWS = responseOrderResponse.DatosOrder;
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
                Transaccion entity = await _pagoService.GuardaTransaccion(userId, data.MetodoPagoId, data.TipoTransaccionId, Monto, gOrdenId, gPeticionId.ToString(), null, null);
                int Idtransaccion = entity.Idtransaccion;
                #endregion

                result.OrdenId = gOrdenId;
                result.PeticionId = gPeticionId.ToString();
                result.url = UrlRedirect;
                response.CodigoMensaje = "200";
                response.Mensaje = "Solicitud exitosa";
                response.Data = result;
            }
            #endregion
            return response;
        }

        private async Task<decimal> ObtenMontoVariable(decimal Monto)
        {
            var variableResult = await _context.Variables.Where(x => x.Idvariable == Convert.ToInt32(EnumVariable.CM)).FirstOrDefaultAsync();

            if (variableResult != null)
            {
                if (variableResult.TipovariableIdtipovariable == Convert.ToInt32(EnumTipoVariable.Global))
                {
                    var variableGlobalResult = await _context.Variableglobals.Where(x => x.VariableIdvariable == variableResult.TipovariableIdtipovariable).FirstOrDefaultAsync();
                    Monto = Convert.ToDecimal(variableGlobalResult.Valor);
                }
                else
                {
                    var variableEmpresaResult = await _context.Variableempresas.Where(x => x.VariableIdvariable == variableResult.TipovariableIdtipovariable).FirstOrDefaultAsync();
                    Monto = Convert.ToDecimal(variableEmpresaResult.Valor);
                }
            }

            return Monto;
        }

        /// <summary>
        /// Metodo para confirmar pago de membresia con "PayPal"
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<SuVanResponse<ConfirmaPagoMembresiaConPayPalResponse>> ConfirmaPagoMembresiaConPayPal(int userId, ConfirmaPagoMembresiaConPayPalRequest data)
        {
            ConfirmaPagoMembresiaConPayPalResponse? result = new ConfirmaPagoMembresiaConPayPalResponse();
            SuVanResponse<ConfirmaPagoMembresiaConPayPalResponse> response = new();
            string EstatusOrden = string.Empty;

            #region Validamos datos de entrada
            var TransaccionsResult = await _context.Transaccions.Where(x => x.Numeroordenpay == data.OrdenId && x.Numeropeticionpay == data.PeticionId).FirstOrDefaultAsync();

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

            //Consumimos WS para validar la orden "pago"
            responseOrdersCapture =  await _payPalService.OrdersCapture(tokenPayPal, data.OrdenId);
            if (responseOrdersCapture.CodigoError != "00")
            {
                //Ocurrió un error 
                response.CodigoMensaje = "400";
                response.Mensaje = responseOrdersCapture.Error;
                return response;
            }
            else
            {
                //Respuesta Correcta
                if (responseOrdersCapture.OrderCapture != null)
                {
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

                    DateTime? desde = DateTime.Now;
                    DateTime? hasta = DateTime.Now.AddDays(1);
                    //#region Actualizamos datos de membresia
                    if (EstatusPago == Convert.ToInt32(EnumEstatusPago.COMPLETED))
                    {
                        #region  Insertamos o Actualizamos la vigencia de la membresia
                        hasta = await InsertaActualizaMembresia(userId, TransaccionsResult.Idtransaccion, desde, hasta);
                        #endregion
                    }
                    else { hasta = null; }
                    result.Estatus = EstatusOrden.ToUpper();
                    result.Vigencia = hasta == null ? String.Empty : hasta.Value.ToString("dd/MM/yyyy");
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
        #endregion
    }
}

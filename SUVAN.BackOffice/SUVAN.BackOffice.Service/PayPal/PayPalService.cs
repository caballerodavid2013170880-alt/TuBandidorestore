using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.AppSettingsModels;
using SUVAN.BackOffice.Models.PayPal.Pago;
using SUVAN.BackOffice.Models.PayPal.Token;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using static SUVAN.BackOffice.Models.PayPal.Pago.CreateOrderRequest;
using static SUVAN.BackOffice.Models.PayPal.Pago.CreateOrderResponseWS;
using static SUVAN.BackOffice.Models.PayPal.Pago.OrdersCaptureResponseWS;
using static System.Net.WebRequestMethods;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SUVAN.BackOffice.Service.PayPal
{
    public class PayPalService : IPayPalService
    {
        private readonly SuvanDbContext _context;
        private readonly PayPalSettingsOptions _payPalSettingsOptions;


        public PayPalService(SuvanDbContext context, IOptions<PayPalSettingsOptions> payPalSettingsOptions)
        {
            _context = context;
            _payPalSettingsOptions = payPalSettingsOptions.Value;


            var variables = _context.Variableglobals.Where(a=>a.VariableIdvariable>0);

			string ULUrl = variables.FirstOrDefault(o => o.VariableIdvariable == 22).Valor;
			string ULClientID = variables.FirstOrDefault(o => o.VariableIdvariable == 23).Valor;
			string ULClientSecret = variables.FirstOrDefault(o => o.VariableIdvariable == 24).Valor;
			string ULUrlSuccess = variables.FirstOrDefault(o => o.VariableIdvariable == 25).Valor;
			string ULUrlCancel = variables.FirstOrDefault(o => o.VariableIdvariable == 26).Valor;

			_payPalSettingsOptions.endPointAmbiente = ULUrl;
			_payPalSettingsOptions.client_id = ULClientID;
			_payPalSettingsOptions.client_secret = ULClientSecret;
			_payPalSettingsOptions.success_url = ULUrlSuccess;
			_payPalSettingsOptions.cancel_url = ULUrlCancel;

		}


		/// <summary>
		/// Metodo para consumir el WS de generacion de Token
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="pmtPeticion"></param>
		/// <returns></returns>
		public async Task<PayPalAccessTokenResponse> GenerarTokenPayPal(int userId, PayPalPeticionGeneracionToken pmtPeticion)
        {
            string BasicAuth = string.Empty;
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(pmtPeticion.client_id + ":" + pmtPeticion.client_secret);
            BasicAuth = System.Convert.ToBase64String(plainTextBytes);

            PayPalAccessTokenResponse respuesta = new PayPalAccessTokenResponse();
            HttpResponseMessage responseWS = new HttpResponseMessage();
            try
            {
                var parametrosPeticion = new List<KeyValuePair<string, string>>();
                if (pmtPeticion.grant_type == "refresh_token")
                {
                    parametrosPeticion = new List<KeyValuePair<string, string>>(){
                    new KeyValuePair<string, string>("Content-Type", "application/x-www-form-urlencoded"),
                    new KeyValuePair<string, string>("grant_type", pmtPeticion.grant_type),
                    new KeyValuePair<string, string>("refresh_token", pmtPeticion.refresh_token)
                    };
                }
                else
                {
                    parametrosPeticion = new List<KeyValuePair<string, string>>(){
                    new KeyValuePair<string, string>("Content-Type", "application/x-www-form-urlencoded"),
                    new KeyValuePair<string, string>("grant_type", pmtPeticion.grant_type),
                    new KeyValuePair<string, string>("response_type", pmtPeticion.response_type),
                    };
                }
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", BasicAuth);
                    responseWS = client.PostAsync(pmtPeticion.endPoint, new FormUrlEncodedContent(parametrosPeticion)).Result;
                }
                if (responseWS.IsSuccessStatusCode)
                {
                    string content = await responseWS.Content.ReadAsStringAsync();
                    var jsonRespuesta = JsonConvert.DeserializeObject<Dictionary<string, string>>(content);

                    respuesta.generar("00", "", jsonRespuesta["scope"], jsonRespuesta["access_token"], jsonRespuesta["token_type"], jsonRespuesta["app_id"], Int32.Parse(jsonRespuesta["expires_in"]), jsonRespuesta["nonce"]);
                }
                else
                {
                    var ErrorWS = await JsonConvert.DeserializeObject<dynamic>(responseWS.Content.ReadAsStringAsync().Result);
                    if (ErrorWS != null)
                    {
                        string jsonError = ErrorWS.ToString();
                        if (jsonError.Contains("error"))
                        {
                            respuesta.generar("01", "Error al generar el token: " + jsonError, "", "", "", "", 0, "");
                        }
                        else
                        {
                            respuesta.generar("02", "Error al generar el token  ", "", "", "", "", 0, "");
                        }
                    }
                    else
                    {
                        respuesta.generar("03", "Error al generar el token", "", "", "", "", 0, "");
                    }
                }
            }
            catch (Exception ex)
            {
                respuesta.generar("04", "Error al generar el token " + ex.Message, "", "", "", "", 0, "");
            }
            return respuesta;
        }

        public PayPalPeticionGeneracionToken ArmaObjetoRequestTokenPayPal(string grant_type, int userId = 0)
        {
            string refresh_token = string.Empty;
            string response_type = string.Empty;

            if (grant_type == "client_credentials")
            {
                response_type = "token";
            }
            string endPontAmbiente = _payPalSettingsOptions.endPointAmbiente;
            string endPontGeneraToken = _payPalSettingsOptions.endPointGeneraTokenPayPal;
            PayPalPeticionGeneracionToken peticionGeneracionToken = null;

            peticionGeneracionToken = new PayPalPeticionGeneracionToken()
            {
                grant_type = grant_type,
                endPoint = endPontAmbiente + endPontGeneraToken,
                client_id = _payPalSettingsOptions.client_id,
                client_secret = _payPalSettingsOptions.client_secret,
                refresh_token = refresh_token,
                response_type = response_type
            };

            return peticionGeneracionToken;
        }


        /// <summary>
        /// Metodo para consumir WS para crear Orden
        ///Ejemplo de uri para consumir WS para Crear Orden
        ///{{base_url}}/v2/checkout/orders
        ///Documentación de PayPal: https://developer.paypal.com/docs/api/orders/v2/#orders_create
        /// </summary>
        /// <param name="token"></param>
        /// <param name="pmtPeticion"></param>
        /// <returns></returns>
        public async Task<CreateOrderResponse> CreateOrder(string token, CreateOrderRequest pmtPeticion)
        {
            CreateOrderResponse ObjetoRespuesta = new CreateOrderResponse();//Clase que contiene metodo generar para asigan respuesta exitosa y obtener errores 
            CreateOrderResponseWS ObjetoRespuestaWS = new CreateOrderResponseWS();//Clase exclusiva para deserialzar respuesta exitosa
            HttpResponseMessage response = new HttpResponseMessage();

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    string json = JsonConvert.SerializeObject(pmtPeticion);
                    var data = new StringContent(json, Encoding.UTF8, "application/json");
                    response = await client.PostAsync(_payPalSettingsOptions.endPointAmbiente + _payPalSettingsOptions.endPointCrearOrden, data);
                }

                if (response.IsSuccessStatusCode)
                {
                    //Respuesta Exitosa
                    var jsonRespuestaExitosa = response.Content.ReadAsStringAsync().Result;
                    ObjetoRespuestaWS = JsonConvert.DeserializeObject<CreateOrderResponseWS>(jsonRespuestaExitosa);
                    ObjetoRespuesta.generar("00", "", ObjetoRespuestaWS);
                }
                else
                {
                    //Error
                    var ErrorWS = await JsonConvert.DeserializeObject<dynamic>(response.Content.ReadAsStringAsync().Result);
                    if (ErrorWS != null)
                    {
                        string jsonError = ErrorWS.ToString();
                        if (jsonError.Contains("error"))
                        {
                            ObjetoRespuesta.generar("01", "Error al generar la orden: " + jsonError);
                        }
                        else
                        {
                            ObjetoRespuesta.generar("02", "Error al generar la orden");
                        }
                    }
                    else
                    {
                        ObjetoRespuesta.generar("03", "Error al generar la orden");
                    }
                }
            }
            catch (Exception ex)
            {
                ObjetoRespuesta.generar("04", "Error al generar la orden: " + ex.Message);
            }
            return ObjetoRespuesta;
        }

        public CreateOrderRequest ArmaObjetoRequestCreateOrder(string gPeticionId, string monto)
        {
            string paramurl = "?pid=" + gPeticionId;
            var orden = new CreateOrderRequest()
            {
                intent = "CAPTURE",
                purchase_units = new List<CreateOrderRequest.PurchaseUnit>() {
                        new CreateOrderRequest.PurchaseUnit() {
                            items = new List<CreateOrderRequest.Item>(){
                                new CreateOrderRequest.Item()
                                {
                                    name = _payPalSettingsOptions.description_pay,
                                    description = _payPalSettingsOptions.description_pay,
                                    quantity = "1",
                                    unit_amount = new   CreateOrderRequest.UnitAmount()
                                    {
                                        currency_code = "MXN",
                                        value = monto
                                    }
                                }
                            },
                            amount = new CreateOrderRequest.Amount() {
                                currency_code = "MXN",
                                value = monto,
                                breakdown = new  CreateOrderRequest.Breakdown(){
                                item_total = new CreateOrderRequest.ItemTotal() {
                                currency_code = "MXN",
                                value = monto
                                }
                                }
                            }
                        }
                    },
                application_context = new ApplicationContext()
                {
                    return_url = _payPalSettingsOptions.success_url + paramurl,// cuando se aprovo la solicitud del cobro
                    cancel_url = _payPalSettingsOptions.cancel_url + paramurl// cuando cancela la operacion
                }
            };
            return orden;
        }


        /// <summary>
        /// Metodo para consumir WS para Confirmar Orden
        ///Ejemplo de uri para consumir WS para Confirmar Orden
        ///https://api-m.sandbox.paypal.com/v2/checkout/orders/:order_id/capture
        ///Documentación de PayPal: https://developer.paypal.com/docs/api/orders/v2/#orders_capture
        /// </summary>
        /// <param name="token"></param>
        /// <param name="order_id"></param>
        /// <returns></returns>
        public async Task<OrdersCaptureResponse> OrdersCapture(string token, string order_id)
        {
            OrdersCaptureResponse ObjetoRespuesta = new OrdersCaptureResponse();//Clase que contiene metodo generar para asigan respuesta exitosa y obtener errores 
            OrdersCaptureResponseWS ObjetoRespuestaWS = new OrdersCaptureResponseWS();//Clase exclusiva para deserialzar respuesta exitosa
            OrdersCaptureExcepcionResponse ObjetoCaptureExcepcionWS = new OrdersCaptureExcepcionResponse();//Clase para obtener excepcion controlada de PayPal
             HttpResponseMessage response = new HttpResponseMessage();

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    var data = new StringContent("{}", Encoding.UTF8, "application/json");
                    response = await client.PostAsync(_payPalSettingsOptions.endPointAmbiente + _payPalSettingsOptions.endPointCrearOrden + "/" + order_id + _payPalSettingsOptions.endPointOrdersCapture, data);
                }

                if (response.IsSuccessStatusCode)
                {
                    //Respuesta Exitosa
                    var jsonRespuestaExitosa = response.Content.ReadAsStringAsync().Result;
                    ObjetoRespuestaWS = JsonConvert.DeserializeObject<OrdersCaptureResponseWS?>(jsonRespuestaExitosa);
                    ObjetoRespuesta.generar("00", "", ObjetoRespuestaWS);
                }
                else if (response.StatusCode == HttpStatusCode.UnprocessableEntity)
                {
                    //Entra en esta parte cuando la orden no se ha aprobado o ya se aprobo
                    string ErrorExcepcion = string.Empty;
                    var jsonErrorExcepcion = response.Content.ReadAsStringAsync().Result;
                    ObjetoCaptureExcepcionWS = JsonConvert.DeserializeObject<OrdersCaptureExcepcionResponse>(jsonErrorExcepcion);
                    if (ObjetoCaptureExcepcionWS != null)
                    {
                        foreach (var item in ObjetoCaptureExcepcionWS.details)
                        {
                            ErrorExcepcion = item.issue;
                        }
                    }
                    ObjetoRespuesta.generar("01", "Error al capturar orden: " + ErrorExcepcion);
                }
                else
                {
                    var ErrorWS = await JsonConvert.DeserializeObject<dynamic>(response.Content.ReadAsStringAsync().Result);
                    if (ErrorWS != null)
                    {
                        string jsonError = ErrorWS.ToString();
                        if (jsonError.Contains("error"))
                        {
                            ObjetoRespuesta.generar("02", "Error al capturar orden: " + jsonError);
                        }
                        else
                        {
                            ObjetoRespuesta.generar("03", "Error al capturar orden");
                        }
                    }
                    else
                    {
                        ObjetoRespuesta.generar("04", "Error al capturar orden");
                    }
                }
            }
            catch (Exception ex)
            {
                ObjetoRespuesta.generar("05", "Error al capturar orden: " + ex.Message);
            }
            return ObjetoRespuesta;
        }


    }
}

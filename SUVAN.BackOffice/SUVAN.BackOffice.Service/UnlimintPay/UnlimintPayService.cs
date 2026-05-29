using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.AppSettingsModels;
using SUVAN.BackOffice.Models.Pago;
using SUVAN.BackOffice.Models.UnlimintPay.Pago;
using SUVAN.BackOffice.Models.UnlimintPay.Token;
using SUVAN.BackOffice.Service.RegistroUsuario;
using System.Collections.Specialized;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Web;

namespace SUVAN.BackOffice.Service.UnlimintPay
{
    public class UnlimintPayService : IUnlimintPayService
    {
        private readonly SuvanDbContext _context;
        private readonly UnlimitPaySettingsOptions _unlimitPaySettingsOptions;

        public UnlimintPayService(SuvanDbContext context, IOptions<UnlimitPaySettingsOptions> unlimitPaySettingsOptions)
        {
            _context = context;

			_unlimitPaySettingsOptions = unlimitPaySettingsOptions.Value;

			string ULUrl = _context.Variableglobals.FirstOrDefault(o => o.VariableIdvariable == 15).Valor;
			string ULTerminalCode = _context.Variableglobals.FirstOrDefault(o => o.VariableIdvariable == 16).Valor;
			string ULPass = _context.Variableglobals.FirstOrDefault(o => o.VariableIdvariable == 17).Valor;
			string ULUrlSuccess = _context.Variableglobals.FirstOrDefault(o => o.VariableIdvariable == 18).Valor;
			string ULUrlCancel = _context.Variableglobals.FirstOrDefault(o => o.VariableIdvariable == 19).Valor;
			string ULUrlProcess = _context.Variableglobals.FirstOrDefault(o => o.VariableIdvariable == 20).Valor;
			string ULUrlDeclinado = _context.Variableglobals.FirstOrDefault(o => o.VariableIdvariable == 21).Valor;


            _unlimitPaySettingsOptions.endPointAmbiente = ULUrl;
			_unlimitPaySettingsOptions.terminal_code = ULTerminalCode;
			_unlimitPaySettingsOptions.password = ULPass;
            _unlimitPaySettingsOptions.success_url = ULUrlSuccess;
			_unlimitPaySettingsOptions.cancel_url = ULUrlCancel;
			_unlimitPaySettingsOptions.inprocess_url = ULUrlProcess;
			_unlimitPaySettingsOptions.decline_url = ULUrlDeclinado;

		}


		public RespuestaGeneracionToken GenerarToken(int userId, PeticionGeneracionToken pmtPeticion)
        {
            RespuestaGeneracionToken respuesta = new RespuestaGeneracionToken();
            HttpResponseMessage responseToken = new HttpResponseMessage();
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
                    new KeyValuePair<string, string>("password", pmtPeticion.password),
                    new KeyValuePair<string, string>("terminal_code", pmtPeticion.terminal_code)
                    };
                }
                using (HttpClient client = new HttpClient())
                {
                    if (pmtPeticion.grant_type == "refresh_token")
                    {
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", pmtPeticion.token);
                    }
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    responseToken = client.PostAsync(pmtPeticion.endPoint, new FormUrlEncodedContent(parametrosPeticion)).Result;
                }
                var jsonRespuesta = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseToken.Content.ReadAsStringAsync().Result);
                if (responseToken.IsSuccessStatusCode)
                {
                    #region Guardamos informacion de Token en BD
                    Tokenpago? InfoTokenpago = _context.Tokenpagos.FirstOrDefault(x => x.UsuarioIdusuario == userId);
                    if (InfoTokenpago == null)
                    {
                        var tokenEntity = new Database.Entities.Tokenpago()
                        {
                            Token = jsonRespuesta["access_token"],
                            TokenRefresh = jsonRespuesta["refresh_token"],
                            Fecharegistro = DateTime.Now,
                            UsuarioIdusuario = userId
                        };
                        _context.Tokenpagos.Add(tokenEntity);
                    }
                    else
                    {
                        InfoTokenpago.Fecharegistro = DateTime.Now;
                        InfoTokenpago.Token = jsonRespuesta["access_token"];
                        InfoTokenpago.TokenRefresh = jsonRespuesta["refresh_token"];
                        _context.Tokenpagos.Entry(InfoTokenpago);
                    }
                    _context.SaveChanges();
                    #endregion
                    respuesta.generar("00", "", jsonRespuesta["token_type"], jsonRespuesta["access_token"], jsonRespuesta["refresh_token"], Int32.Parse(jsonRespuesta["expires_in"]), Int32.Parse(jsonRespuesta["refresh_expires_in"]));
                }
                else
                {
                    if (jsonRespuesta != null)
                    {
                        respuesta.generar("02", "Error al generar el token=" + jsonRespuesta["name"] + ":" + jsonRespuesta["message"], "", "", "", 0, 0);
                    }
                    else
                    {
                        respuesta.generar("01", "Error al generar el token", "", "", "", 0, 0);
                    }
                }
            }
            catch (Exception ex)
            {
                respuesta.generar("01", "Error al generar el token " + ex.Message, "", "", "", 0, 0);
            }
            return respuesta;
        }


        public RespuestaGeneracionPago GenerarUrlPago(PeticionGeneracionPago pmtPeticion)
        {
            RespuestaGeneracionPago respuesta = new RespuestaGeneracionPago();
            HttpResponseMessage responseToken = new HttpResponseMessage();

            try
            {
                if (pmtPeticion.token != null)
                {
                    var parametrosPeticion = new List<KeyValuePair<string, string>>(){
                    new KeyValuePair<string, string>("Content-Type", "application/json")
                };
                    using (HttpClient client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", pmtPeticion.token);
                        PeticionCreatePayment peticion = new PeticionCreatePayment(pmtPeticion);
                        string json = JsonConvert.SerializeObject(peticion);
                        var data = new StringContent(json, Encoding.UTF8, "application/json");
                        responseToken = client.PostAsync(pmtPeticion.endPoint, data).Result;
                    }
                    var jsonRespuesta = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseToken.Content.ReadAsStringAsync().Result);
                    if (responseToken.IsSuccessStatusCode)
                    {
                        respuesta.generar("00", "", jsonRespuesta["redirect_url"]);
                    }
                    else
                    {
                        if (jsonRespuesta != null)
                        {
                            respuesta.generar("02", "Error al generar el pago=" + jsonRespuesta["name"] + ":" + jsonRespuesta["message"], "");
                        }
                        else
                        {
                            respuesta.generar("02", "Error al generar el pago", "");
                        }
                    }
                }
                else
                {
                    respuesta.generar("03", "Error al generar el pago= No se ha enviado el token para la petición", "");
                }
            }
            catch (Exception ex)
            {
                respuesta.generar("01", "Error al generar el pago=" + ex.Message, "");
            }
            return respuesta;
        }

        public RespuestaValidacionPago ValidarPago(PeticionValidacionPago pmtPeticion)
        {
            RespuestaValidacionPago respuesta = new RespuestaValidacionPago();
            HttpResponseMessage responseToken = new HttpResponseMessage();

            try
            {
                string UrlPeticion = pmtPeticion.endPoint + "/?request_id=" + pmtPeticion.request_id + "&merchant_order_id=" + pmtPeticion.merchant_order_id;
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", pmtPeticion.token);
                    responseToken = client.GetAsync(UrlPeticion).Result;
                }
                var jsonRespuesta = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseToken.Content.ReadAsStringAsync().Result);

                if (responseToken.IsSuccessStatusCode)
                {
                    RespuestaValidacionPagoWS jsonRespuesta2 = new RespuestaValidacionPagoWS();
                    jsonRespuesta2 = JsonConvert.DeserializeObject<RespuestaValidacionPagoWS>(responseToken.Content.ReadAsStringAsync().Result);
                    respuesta.generar("00", "", jsonRespuesta2);

                }
                else
                {
                    if (jsonRespuesta != null)
                    {
                        respuesta.generar("02", "Error al validar el pago=" + jsonRespuesta["name"] + ":" + jsonRespuesta["message"]);
                    }
                    else
                    {
                        respuesta.generar("02", "Error al validar el pago");
                    }
                }
            }
            catch (Exception ex)
            {
                respuesta.generar("01", "Error al validar el pago=" + ex.Message);
            }
            return respuesta;
        }


        public PeticionGeneracionToken ArmaRequestToken(string grant_type, int userId = 0)
        {
            string refresh_token = string.Empty;
            if (grant_type == "refresh_token")
            {
                Tokenpago? InfoTokenpago = _context.Tokenpagos.FirstOrDefault(x => x.UsuarioIdusuario == userId);
                if (InfoTokenpago != null)
                {
                    refresh_token = InfoTokenpago.TokenRefresh;
                }
            }

            string endPontAmbiente = _unlimitPaySettingsOptions.endPointAmbiente;
            string endPontGeneraToken = _unlimitPaySettingsOptions.endPointGeneraToken;
            PeticionGeneracionToken peticionGeneracionToken = null;

            peticionGeneracionToken = new PeticionGeneracionToken()
            {
                grant_type = grant_type,
                endPoint = endPontAmbiente + endPontGeneraToken,
                terminal_code = _unlimitPaySettingsOptions.terminal_code,
                password = _unlimitPaySettingsOptions.password,
                refresh_token = refresh_token
            };

            return peticionGeneracionToken;
        }

        public PeticionGeneracionPago ArmaRequestPeticionGeneracionPago(string tokenPay, string gPeticionId, string gOrdenId, string emailUser, decimal Monto, string Moneda)
        {
            string endPontAmbiente = _unlimitPaySettingsOptions.endPointAmbiente;
            string endPointCreatePaymentPage = _unlimitPaySettingsOptions.endPointCreatePaymentPage;
            string paramurl = "?noorderpayid=" + gOrdenId;

            PeticionGeneracionPago peticionGeneracionPago = null;

            peticionGeneracionPago = new PeticionGeneracionPago()
            {
                endPoint = endPontAmbiente + endPointCreatePaymentPage,
                token = tokenPay,
                merchant_order = new PeticionGeneracionPago.Merchant_order()
                {
                    id = gOrdenId,
                    description = _unlimitPaySettingsOptions.description_pay,
                },
                request = new PeticionGeneracionPago.Request()
                {
                    id = gPeticionId,
                    time = DateTime.UtcNow.ToString("yyyy-MM-dd'T'HH:mm:ss'Z'")
                },
                payment_method = _unlimitPaySettingsOptions.payment_method,//De acuerdo a Unlimit se deja este valor Fijo
                payment_Data = new PeticionGeneracionPago.Payment_data()
                {
                    currency = Moneda.ToUpper(),//Revisar si va a quedar en duro
                    amount = Monto.ToString()
                },
                customer = new PeticionGeneracionPago.Customer()
                {
                    email = emailUser,
                    id = emailUser,
                    locale = "es"//De momento se deja este valor Fijo
                },
                return_urls = new PeticionGeneracionPago.Return_urls()
                {
                    success_url = _unlimitPaySettingsOptions.success_url + paramurl,
                    cancel_url = _unlimitPaySettingsOptions.cancel_url + paramurl,
                    decline_url = "http://www.test.com/decline-url" + paramurl,
                    inprocess_url = _unlimitPaySettingsOptions.inprocess_url + paramurl
                }
            };
            return peticionGeneracionPago;
        }

        public PeticionValidacionPago ArmaRequestPeticionValidaPago(string tokenPay, string OrdenId, string PeticionId)
        {
            string endPontAmbiente = _unlimitPaySettingsOptions.endPointAmbiente;
            string endPointCreatePaymentPage = _unlimitPaySettingsOptions.endPointCreatePaymentPage;

            PeticionValidacionPago peticionValidacionPago = null;

            peticionValidacionPago = new PeticionValidacionPago()
            {
                endPoint = endPontAmbiente + endPointCreatePaymentPage,
                token = tokenPay,
                merchant_order_id = OrdenId,
                request_id = PeticionId
            };

            return peticionValidacionPago;
        }


    }
}

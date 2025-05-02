using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic;
using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.AppSettingsModels;
using SUVAN.BackOffice.Models.Facturacion;
using SUVAN.BackOffice.Models.ManejoRespuesta;
using SUVAN.BackOffice.Service.RegistroUsuario;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
//using FacturacionPegaso;
using System.ServiceModel;
using System.Security.AccessControl;
using System.Xml.Linq;
using static SUVAN.BackOffice.Models.UnlimintPay.Pago.RespuestaValidacionPagoWS;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Diagnostics.Eventing.Reader;
using SUVAN.BackOffice.Service.Notificaciones;
using PdfSharpCore;
using PdfSharpCore.Pdf;
using TheArtOfDev.HtmlRenderer.PdfSharp;
using PdfSharpCore.Pdf.Content.Objects;
using System.Collections;
using System.Reflection.Metadata;
using SUVAN.BackOffice.Models.ViewModel.Enums;

namespace SUVAN.BackOffice.Service.Facturacion
{
  public class FacturacionService : IFacturacionService
  {
    private readonly SuvanDbContext _context;
    private readonly IUsuarioService _usuarioService;
    private readonly INotificacionCorreoService _notificacionCorreoService;

    public FacturacionService(SuvanDbContext context, IUsuarioService usuarioService, INotificacionCorreoService notificacionCorreoService)
    {
      _context = context;
      _usuarioService = usuarioService;
      _notificacionCorreoService = notificacionCorreoService;
    }

    public async Task<SuVanResponse<GeneraFacturaResponse>> GenerarFactura(int userId, string emailUser, GeneraFacturaRequest data)
    {
      GeneraFacturaResponse? result = new GeneraFacturaResponse();
      SuVanResponse<GeneraFacturaResponse> response = new();

      int TransaccionId = 0;
      int EmpresaId = 0;
      bool? Facturado = false;

      //Variables de para generar Factura
      string Serie = string.Empty;
      int Folio = 0;
      string FormaPago = "01";
      string TipoComprobante = "I";
      string LugarexpedicionCp = string.Empty;
      string MetodoPago = "PUE";
      string Exportacion = "01";//01 = (No aplica)
                                //Variables del EMISOR (Empresa que emite la factura)
      string RfcEmpresa = string.Empty;
      string RegimenfiscalEmisor = string.Empty;
      string NombreEmpresa = string.Empty;
      //Variables del RECEPTOR  (Datos del Usuario)
      string RfcReceptor = string.Empty;
      string NombreReceptor = string.Empty;
      string UsoCfdiReceptor = string.Empty;
      string DomicilioFiscalReceptor = string.Empty;
      string RegimenFiscalReceptor = string.Empty;
      //Variables de Concepto
      string Cantidad = string.Empty; ;
      string Claveunidad = string.Empty;
      string Claveprodserv = string.Empty;
      string Descripcion = string.Empty;
      string NoIdentificacion = string.Empty;
      string Objetoimp = string.Empty;
      string TipoComprobanteClave = string.Empty;
      string Sucursal = string.Empty;

      //Variables de monto
      decimal IVA = 0;// (decimal)datosfacturacionproductoResult.Iva;//De momento obtenemos este dato de esta tabla pero tendriamos que revisar si tenemos otra tabla por lo del tema del IVA Fronterizo
      decimal MontoTotal = 0;//(decimal)InfoTransaccionResult.Cantidad;
      decimal PorcentajeIVA = 0;//IVA / 100;
      decimal MonTotalSinIVA = 0;// Math.Round(MontoTotal / (decimal)1.16, 2);
      decimal MontoIVA = 0; //MontoTotal - MonTotalSinIVA;


      //Variables para obtener los datos timbrados
      string fechaCreacion = string.Empty;
      string Transaccion_EstatusTimbrado = string.Empty;//Transaccion EstatusTimbrado
      string Transaccion_Tipo = string.Empty;//Tipo

      string CFD_codigoDeBarras = string.Empty;//CFD
      string CFD_selloEmisor = string.Empty;//CFD
      string CFD_Folio = string.Empty;//CFD
      string CFD_FechaEmision = string.Empty;//CFD
      string CFD_cadenaOriginal = string.Empty;//CFD
      string CFD_noCertificadoCSD = string.Empty;///CFD
			string CFD_comprobanteStr = string.Empty;

      string TFD_foliofiscalUUID = string.Empty;//TFD
      string TFD_FechaTimbrado = string.Empty;//TFD
      string TFD_noCertificadoSAT = string.Empty;//TFD
      string TFD_selloSAT = string.Empty;//CFD

      Guid Transaccionidpeticionwebservice = Guid.NewGuid();//El PAC solicita un ID de transacción en este caso generamos uno
      SUVAN.Facturacion.Facturacion Fac = new SUVAN.Facturacion.Facturacion();
      Dictionary<string, byte[]> Attachments = new Dictionary<string, byte[]>();
      string HtmlContent = string.Empty;

      #region Validamos datos de entrada
      ModelDatosTransaccion DatosTransaccion = await getDatosTransaccionViaje(data.viaje_id);
      if (DatosTransaccion == null)
      {
        response.CodigoMensaje = "400";
        response.Mensaje = "No se encontro información de la transacción";
        return response;
      }
      else
      {
        TransaccionId = DatosTransaccion.Idtransaccion;
        EmpresaId = (int)DatosTransaccion.EmpresaIdempresa;
        Facturado = DatosTransaccion.Facturado;
      }
      #endregion

      if (!(bool)Facturado)
      {
        if (data.perfil_facturacion_id <= 0)
        {
          response.CodigoMensaje = "400";
          response.Mensaje = "Falta el parámetro perfil_facturacion_id";
          return response;
        }

        #region Obtenemos Informacion para poblar datos de la factura
        Datosfacturacionproducto? datosfacturacionproductoResult = await getDatosfacturacionproductos();
        if (datosfacturacionproductoResult == null)
        {
          //Ocurrió un error al generar token 
          response.CodigoMensaje = "400";
          response.Mensaje = "No se encontro información para facturar";
          return response;
        }
        else
        {
          Cantidad = datosfacturacionproductoResult.Cantidad.ToString();
          Claveunidad = datosfacturacionproductoResult.Claveunidad;
          Claveprodserv = datosfacturacionproductoResult.Claveprodserv;
          Descripcion = datosfacturacionproductoResult.Descripcion;
          NoIdentificacion = datosfacturacionproductoResult.Noidentificacion;
          Objetoimp = datosfacturacionproductoResult.Objetoimp;
          TipoComprobanteClave = datosfacturacionproductoResult.Tipocomprobanteclave;
          Sucursal = datosfacturacionproductoResult.Sucursal;
        }

        //Datos de Facturacion del Emisor (Empresa)
        DatosFacturacionEmisor DatosFacturacionEmisorResult = await getDatosFacturacionEmisor(EmpresaId);
        if (DatosFacturacionEmisorResult == null)
        {
          response.CodigoMensaje = "400";
          response.Mensaje = "No se encontro información para facturar";
          return response;
        }
        else
        {
          Folio = (int)DatosFacturacionEmisorResult.Folio;
          Serie = DatosFacturacionEmisorResult.Serie;
          LugarexpedicionCp = DatosFacturacionEmisorResult.LugarexpedicionCp;
          RfcEmpresa = DatosFacturacionEmisorResult.RfcEmpresa;
          RegimenfiscalEmisor = DatosFacturacionEmisorResult.RegimenfiscalEmisor;
          NombreEmpresa = DatosFacturacionEmisorResult.NombreEmpresa;
        }

        //Datos de Facturacion del Receptor (Usuario)
        DatosFacturacionReceptor DatosFacturacionReceptorResult = await getDatosFacturacionReceptor(userId, data.perfil_facturacion_id);

        if (DatosFacturacionReceptorResult == null)
        {
          response.CodigoMensaje = "400";
          response.Mensaje = "No se encontro información del receptor para facturar";
          return response;
        }
        else
        {
          RfcReceptor = DatosFacturacionReceptorResult.RfcReceptor;
          NombreReceptor = DatosFacturacionReceptorResult.NombreReceptor;
          UsoCfdiReceptor = DatosFacturacionReceptorResult.UsoCfdiReceptor;
          DomicilioFiscalReceptor = DatosFacturacionReceptorResult.DomicilioFiscalReceptor;
          RegimenFiscalReceptor = DatosFacturacionReceptorResult.RegimenFiscalReceptor;
        }
        #endregion

        //Asignamos valores de montos
        IVA = (decimal)datosfacturacionproductoResult.Iva;//De momento obtenemos este dato de esta tabla pero tendriamos que revisar si tenemos otra tabla por lo del tema del IVA Fronterizo
        MontoTotal = (decimal)DatosTransaccion.Cantidad;
        PorcentajeIVA = IVA / 100;
        MonTotalSinIVA = Math.Round(MontoTotal / (decimal)1.16, 2);
        MontoIVA = MontoTotal - MonTotalSinIVA;

        string strxml = Fac.XMLrequest(Serie, Folio, TipoComprobante, LugarexpedicionCp, MetodoPago, Exportacion, RfcEmpresa, RegimenfiscalEmisor, NombreEmpresa, RfcReceptor, NombreReceptor, UsoCfdiReceptor, DomicilioFiscalReceptor, RegimenFiscalReceptor, Cantidad, Claveunidad, Claveprodserv, Descripcion, NoIdentificacion, Objetoimp, TipoComprobanteClave, Sucursal, MontoTotal, PorcentajeIVA, MonTotalSinIVA, MontoIVA, Transaccionidpeticionwebservice);
        strxml = Strings.Replace(Strings.Replace(strxml, @"\'", "\""), "  ", " ");
        XElement xmlElement = XElement.Parse(strxml);



        string liga = ObtenLiga(EmpresaId);

        string RespuestaWS = Fac.TimbraFactura(strxml, liga);
        //string RespuestaWS = "<ResponseAdmon fechaCreacion=\"2024-02-19T16:35:41\" version=\"3.0\" xmlns=\"http://www.pegasotecnologia.com/\"><Transaccion id=\"51145860-2cbc-45d5-847d-53d3431d8694\" tipo=\"EMISION\" estatus=\"EXITO\" /><CFD codigoDeBarras=\"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\" fecha=\"2024-02-19T16:35:40\" sello=\"V3o6iP5ZkO8Q31d5nN5Uhemf5GJWoOWMbQKVtIg4OTciZnCEmOy1BeI/efG9EcvR+WZ44L6dBF2sHeQrWLUGNK3LH9Q0VKk9fucLzMdmSL0RMYYt+wW/fra/xE/ajgjpUttdmZm2fjwy1D9EEKO/8Hxahj7Qab5WwxsRg0N4T5gLvtvmOTf0HkYPrSQFXWlFsNbYalEezerdYxy3rYptS2N8LPg37LvlsjxfKbyT4U7jWQkdPzeSFcpJLcyX5Rk8r2uWmqh8i2qzja7q/Ceq1PhyjpKzj8riOsU11mHoEHkCNMWP727VyjuHTpTDqYoiAZB/pvHH7Reyt7QDbll3yA==\" serie=\"F\" folio=\"105\" cadenaOriginal=\"||1.1|1D59A583-3443-458C-B67C-B11B01745A05|2024-02-19T16:35:41|PAC010101000|V3o6iP5ZkO8Q31d5nN5Uhemf5GJWoOWMbQKVtIg4OTciZnCEmOy1BeI/efG9EcvR+WZ44L6dBF2sHeQrWLUGNK3LH9Q0VKk9fucLzMdmSL0RMYYt+wW/fra/xE/ajgjpUttdmZm2fjwy1D9EEKO/8Hxahj7Qab5WwxsRg0N4T5gLvtvmOTf0HkYPrSQFXWlFsNbYalEezerdYxy3rYptS2N8LPg37LvlsjxfKbyT4U7jWQkdPzeSFcpJLcyX5Rk8r2uWmqh8i2qzja7q/Ceq1PhyjpKzj8riOsU11mHoEHkCNMWP727VyjuHTpTDqYoiAZB/pvHH7Reyt7QDbll3yA==|00000000000000000000||\" comprobanteStr=\"&lt;?xml version=&quot;1.0&quot; encoding=&quot;utf-8&quot;?&gt;&lt;cfdi:Comprobante xmlns:cfdi=&quot;http://www.sat.gob.mx/cfd/4&quot; xmlns:xsi=&quot;http://www.w3.org/2001/XMLSchema-instance&quot; xsi:schemaLocation=&quot;http://www.sat.gob.mx/cfd/4 http://www.sat.gob.mx/sitio_internet/cfd/4/cfdv40.xsd&quot; Version=&quot;4.0&quot; Serie=&quot;F&quot; Folio=&quot;105&quot; Fecha=&quot;2024-02-19T16:35:40&quot; Moneda=&quot;MXN&quot; TipoCambio=&quot;1&quot; SubTotal=&quot;232.76&quot; Total=&quot;270.00&quot; FormaPago=&quot;01&quot; TipoDeComprobante=&quot;I&quot; MetodoPago=&quot;PUE&quot; LugarExpedicion=&quot;50740&quot; Exportacion=&quot;01&quot; NoCertificado=&quot;30001000000500003444&quot; Certificado=&quot;MIIFoTCCA4mgAwIBAgIUMzAwMDEwMDAwMDA1MDAwMDM0NDQwDQYJKoZIhvcNAQELBQAwggErMQ8wDQYDVQQDDAZBQyBVQVQxLjAsBgNVBAoMJVNFUlZJQ0lPIERFIEFETUlOSVNUUkFDSU9OIFRSSUJVVEFSSUExGjAYBgNVBAsMEVNBVC1JRVMgQXV0aG9yaXR5MSgwJgYJKoZIhvcNAQkBFhlvc2Nhci5tYXJ0aW5lekBzYXQuZ29iLm14MR0wGwYDVQQJDBQzcmEgY2VycmFkYSBkZSBjYWxpejEOMAwGA1UEEQwFMDYzNzAxCzAJBgNVBAYTAk1YMRkwFwYDVQQIDBBDSVVEQUQgREUgTUVYSUNPMREwDwYDVQQHDAhDT1lPQUNBTjERMA8GA1UELRMIMi41LjQuNDUxJTAjBgkqhkiG9w0BCQITFnJlc3BvbnNhYmxlOiBBQ0RNQS1TQVQwHhcNMjMwNTE4MTM1MjIzWhcNMjcwNTE4MTM1MjIzWjCByDEiMCAGA1UEAxMZSklNRU5FWiBFU1RSQURBIFNBTEFTIEEgQTEiMCAGA1UEKRMZSklNRU5FWiBFU1RSQURBIFNBTEFTIEEgQTEiMCAGA1UEChMZSklNRU5FWiBFU1RSQURBIFNBTEFTIEEgQTElMCMGA1UELRMcSkVTOTAwMTA5UTkwIC8gVkFEQTgwMDkyN0RKMzEeMBwGA1UEBRMVIC8gVkFEQTgwMDkyN0hTUlNSTDA1MRMwEQYDVQQLEwpTdWN1cnNhbCAyMIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAiCXqWezB86LQauOQR0kCeowaCnqRb9rbzpcnSU5o9fPngPaa+U5Y0ZMkRbH/KOdXtIxZd4ZZmjv8h1wUuY6gEy0cknr+TTS4q+s4OXrlOUNFmzee/lSRJxLW7zEdfXNWO36slzYplMmmMWYQuFs6KI51GeB6U2fRFBdbhnr4CJY2T7/1foU8LkwTNIaXe3Ftr34t9SrDSpQeYSTSKhuQaf3RR5/n7ePCuHgoqGP8U3UyUxSaKsdNVu9W1bPnNYNqQra4hP8EahguTsSkZzbaK94qS2L4oz4CcqFVxCsjm0ZwWwfvHQ5TPKAmGBd8L4mW7Yb6qPQZmhU4aRSGrMMGeQIDAQABox0wGzAMBgNVHRMBAf8EAjAAMAsGA1UdDwQEAwIGwDANBgkqhkiG9w0BAQsFAAOCAgEAXVn15XA/xnwkvbWOkPHEXJTlmYxKnv8eOctybWlOT7NNzqm9Kt1Bu2PtYPaKrvcBYKezT2DARsp2HJNmU/oGPFL2hDFqVac+FLuYfiWY1GBMiNtnOu9GqiPoualKq1UM4dUspuir6ccRf33glbi4m8al89Rz+vZSRSRL44iYWQ4p3w6kxEk23MT3R8eQG4pGEcHYfkUchibT1BUUex3alW1xH9BuST5zHh8zii+aWkCC/UP2mTomEmMf4vJc0xVtnoNb8lDvYNwCO4ZODc0pssO8QaC4kEmiSqOeGxghORGkvqd0joDZ2cnmQ8ilgWJ+RBVQzxny/hem3AQFzP5CGFgZQ+I6frMV97jZzPbOqnLKyjre6Fc+SjPN+ExfXM0FrGMxeeXMzBubrD8UkmG/OT5nZI7urqXVOywRYbQkMVf+19BLiuTYHctBTxjjQ7HNWz5SS2qeKzPvKL4Av7828hBsweRpi8+KVqnxToDzH0FPNiYDexLg/QQKF9Dh4ryEYoMc54YpfC3H/Q+GMmUeMLf43mv+4bcSFwnKEDswY38V4ooOP4v9IIK3z7YP9+Gk4Vu4+0kTQrI1KuIiRYCGEQPOLWi6WcYGUHHO9FDz5hV0cYsC81QbWelSSqxazrrhb9SnbtYRqlYzdpY+S9gecBx+JKvDArJTypgomMUer44=&quot; Sello=&quot;V3o6iP5ZkO8Q31d5nN5Uhemf5GJWoOWMbQKVtIg4OTciZnCEmOy1BeI/efG9EcvR+WZ44L6dBF2sHeQrWLUGNK3LH9Q0VKk9fucLzMdmSL0RMYYt+wW/fra/xE/ajgjpUttdmZm2fjwy1D9EEKO/8Hxahj7Qab5WwxsRg0N4T5gLvtvmOTf0HkYPrSQFXWlFsNbYalEezerdYxy3rYptS2N8LPg37LvlsjxfKbyT4U7jWQkdPzeSFcpJLcyX5Rk8r2uWmqh8i2qzja7q/Ceq1PhyjpKzj8riOsU11mHoEHkCNMWP727VyjuHTpTDqYoiAZB/pvHH7Reyt7QDbll3yA==&quot;&gt;&#xD;&#xA;  &lt;cfdi:Emisor Rfc=&quot;ATP561002J67&quot; Nombre=&quot;Kratos Consultores&quot; RegimenFiscal=&quot;624&quot; /&gt;&#xD;&#xA;  &lt;cfdi:Receptor Rfc=&quot;UIC980724RXA&quot; Nombre=&quot;EDUARDO REYES MORENO&quot; UsoCFDI=&quot;G03&quot; DomicilioFiscalReceptor=&quot;50740&quot; RegimenFiscalReceptor=&quot;601&quot; /&gt;&#xD;&#xA;  &lt;cfdi:Conceptos&gt;&#xD;&#xA;    &lt;cfdi:Concepto ClaveProdServ=&quot;78111802&quot; NoIdentificacion=&quot;SUV12345&quot; Cantidad=&quot;1&quot; ClaveUnidad=&quot;E48&quot; Descripcion=&quot;SERVICIO PUBLICO DE TRANSPORTE DE PERSONAS&quot; ValorUnitario=&quot;232.76&quot; Importe=&quot;232.76&quot; ObjetoImp=&quot;02&quot;&gt;&#xD;&#xA;      &lt;cfdi:Impuestos&gt;&#xD;&#xA;        &lt;cfdi:Traslados&gt;&#xD;&#xA;          &lt;cfdi:Traslado Base=&quot;232.76&quot; Impuesto=&quot;002&quot; TipoFactor=&quot;Tasa&quot; TasaOCuota=&quot;0.160000&quot; Importe=&quot;37.24&quot; /&gt;&#xD;&#xA;        &lt;/cfdi:Traslados&gt;&#xD;&#xA;      &lt;/cfdi:Impuestos&gt;&#xD;&#xA;    &lt;/cfdi:Concepto&gt;&#xD;&#xA;  &lt;/cfdi:Conceptos&gt;&#xD;&#xA;  &lt;cfdi:Impuestos TotalImpuestosTrasladados=&quot;37.24&quot;&gt;&#xD;&#xA;    &lt;cfdi:Traslados&gt;&#xD;&#xA;      &lt;cfdi:Traslado Impuesto=&quot;002&quot; TipoFactor=&quot;Tasa&quot; TasaOCuota=&quot;0.160000&quot; Importe=&quot;37.24&quot; Base=&quot;232.76&quot; /&gt;&#xD;&#xA;    &lt;/cfdi:Traslados&gt;&#xD;&#xA;  &lt;/cfdi:Impuestos&gt;&#xD;&#xA;  &lt;cfdi:Complemento&gt;&#xD;&#xA;    &lt;tfd:TimbreFiscalDigital xmlns:tfd=&quot;http://www.sat.gob.mx/TimbreFiscalDigital&quot; xmlns:xsi=&quot;http://www.w3.org/2001/XMLSchema-instance&quot; xsi:schemaLocation=&quot;http://www.sat.gob.mx/TimbreFiscalDigital http://www.sat.gob.mx/sitio_internet/cfd/TimbreFiscalDigital/TimbreFiscalDigitalv11.xsd&quot; Version=&quot;1.1&quot; UUID=&quot;1D59A583-3443-458C-B67C-B11B01745A05&quot; FechaTimbrado=&quot;2024-02-19T16:35:41&quot; RfcProvCertif=&quot;PAC010101000&quot; SelloCFD=&quot;V3o6iP5ZkO8Q31d5nN5Uhemf5GJWoOWMbQKVtIg4OTciZnCEmOy1BeI/efG9EcvR+WZ44L6dBF2sHeQrWLUGNK3LH9Q0VKk9fucLzMdmSL0RMYYt+wW/fra/xE/ajgjpUttdmZm2fjwy1D9EEKO/8Hxahj7Qab5WwxsRg0N4T5gLvtvmOTf0HkYPrSQFXWlFsNbYalEezerdYxy3rYptS2N8LPg37LvlsjxfKbyT4U7jWQkdPzeSFcpJLcyX5Rk8r2uWmqh8i2qzja7q/Ceq1PhyjpKzj8riOsU11mHoEHkCNMWP727VyjuHTpTDqYoiAZB/pvHH7Reyt7QDbll3yA==&quot; NoCertificadoSAT=&quot;00000000000000000000&quot; SelloSAT=&quot;AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA==&quot; /&gt;&#xD;&#xA;  &lt;/cfdi:Complemento&gt;&#xD;&#xA;&lt;/cfdi:Comprobante&gt;\" noCertificado=\"30001000000500003444\" /><TFD UUID=\"1D59A583-3443-458C-B67C-B11B01745A05\" FechaTimbrado=\"2024-02-19T16:35:41\" noCertificadoSAT=\"00000000000000000000\" selloSAT=\"AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA==\" /></ResponseAdmon>";

        //Ejemplo de una respuesta  con error
        //string RespuestaWS = "<ResponseAdmon fechaCreacion=\"2024-02-15T15:27:08\" version=\"3.0\" xmlns=\"http://www.pegasotecnologia.com/\"><Transaccion id=\"1234567897\" tipo=\"EMISION\" estatus=\"ERROR\" /><CFD fecha=\"0001-01-01T00:00:00\" /><Error noIdentificacion=\"3001\" descripcion=\"Error al emitir el CFDI. --- (ValidarContenidoFiscal) La validación del contenido del CFDI v4.0 arrojo el siguiente problema: {CFDI40158}-La clave del campo RegimenFiscalR debe corresponder con el tipo de persona (física o moral).&#xD;&#xA;{RegimenFiscal: 605}  ----  {Pegaso.SCFD.Emision.Base.Procesamiento.Builders.Version40.ProcCFD.GenerarCFDv40.ValidarContenidoFiscal}\" /></ResponseAdmon>";
        XmlDocument xmlDocResponse = new XmlDocument();
        if (!string.IsNullOrEmpty(RespuestaWS))
        {
          if (RespuestaWS.Contains("EXITO"))
          {
            xmlDocResponse.LoadXml(RespuestaWS);
            var doc = XDocument.Parse(RespuestaWS);

            //agregamos un Namespace, que usaremos para buscar que el nodo no exista:
            XmlNamespaceManager nsm = new XmlNamespaceManager(xmlDocResponse.NameTable);
            nsm.AddNamespace("ResponseAdmon", "http://www.pegasotecnologia.com/");

            if (xmlDocResponse.SelectSingleNode("//ResponseAdmon:ResponseAdmon", nsm) != null)
            {
              XmlNode NodoResponseAdmon = xmlDocResponse.SelectSingleNode("//ResponseAdmon:ResponseAdmon", nsm);
              //Obtenemos valores
              fechaCreacion = NodoResponseAdmon.Attributes["fechaCreacion"]?.Value;
            }

            //***************Accedemos a nodo "Transaccion"***************
            if (xmlDocResponse.SelectSingleNode("//ResponseAdmon:Transaccion", nsm) != null)
            {
              XmlNode NodoTransaccion = xmlDocResponse.SelectSingleNode("//ResponseAdmon:Transaccion", nsm);
              //Obtenemos valores
              Transaccion_EstatusTimbrado = NodoTransaccion.Attributes["estatus"]?.Value;
              Transaccion_Tipo = NodoTransaccion.Attributes["tipo"]?.Value;
            }

            if (Transaccion_EstatusTimbrado.ToUpper() == "EXITO")
            {
              ////***************Accedemos a nodo "CFD"***************
              XmlNode NodoCFD = xmlDocResponse.SelectSingleNode("//ResponseAdmon:CFD", nsm);
              CFD_comprobanteStr = NodoCFD.Attributes["comprobanteStr"]?.Value;
              CFD_codigoDeBarras = NodoCFD.Attributes["codigoDeBarras"]?.Value;
              CFD_selloEmisor = NodoCFD.Attributes["sello"]?.Value;
              CFD_Folio = NodoCFD.Attributes["folio"]?.Value;
              CFD_FechaEmision = NodoCFD.Attributes["fecha"]?.Value;
              CFD_cadenaOriginal = NodoCFD.Attributes["cadenaOriginal"]?.Value;
              CFD_noCertificadoCSD = NodoCFD.Attributes["noCertificado"]?.Value;

              ////***************Accedemos a nodo "TFD"***************
              XmlNode NodoTFD = xmlDocResponse.SelectSingleNode("//ResponseAdmon:TFD", nsm);
              TFD_foliofiscalUUID = NodoTFD.Attributes["UUID"]?.Value;
              TFD_FechaTimbrado = NodoTFD.Attributes["FechaTimbrado"]?.Value;
              TFD_noCertificadoSAT = NodoTFD.Attributes["noCertificadoSAT"]?.Value;
              TFD_selloSAT = NodoTFD.Attributes["selloSAT"]?.Value;

              #region Guardamos y actualizamos en tablas 
              var FacturaAdd = new Database.Entities.Factura()
              {
                UsuarioIdusuario = userId,
                TransaccionIdtransaccion = TransaccionId,
                Transaccionidpeticionwebservice = Transaccionidpeticionwebservice.ToString(),
                Folio = Folio,
                Serie = Serie,
                Fechacreacion = Convert.ToDateTime(fechaCreacion),
                Fechaemision = Convert.ToDateTime(CFD_FechaEmision),
                Cadenaoriginal = CFD_cadenaOriginal,
                Codigobarras = CFD_codigoDeBarras,
                Nocertificadocsd = CFD_noCertificadoCSD,
                FoliofiscalUuid = TFD_foliofiscalUUID,
                Fechatimbrado = Convert.ToDateTime(TFD_FechaTimbrado),
                Certificadosat = TFD_noCertificadoSAT,
                Sellosat = TFD_selloSAT,
                Xmltimbrado = CFD_comprobanteStr,
                Xmltimbradopac = RespuestaWS,
                Fecharegistro = DateTime.Now
              };
              _context.Facturas.Add(FacturaAdd);
              await _context.SaveChangesAsync();

              //////Actualizamos estatus de facturacion en Viaje
              Viaje? ViajeEntity = await _context.Viajes.Where(x => x.Idviaje == data.viaje_id).FirstOrDefaultAsync();
              if (ViajeEntity != null)
              {
                ViajeEntity.Facturado = true;
                _context.SaveChanges();
              }

              //////Actualizamos el No Folio
              Datosfacturacionemisor? DatosfacturacionemisorEntity = await _context.Datosfacturacionemisors.Where(x => x.EmpresaIdempresa == EmpresaId).FirstOrDefaultAsync();
              if (DatosfacturacionemisorEntity != null)
              {
                DatosfacturacionemisorEntity.Folio = DatosfacturacionemisorEntity.Folio + 1;
                _context.SaveChanges();
              }
              #endregion

              #region Obtenemos XML Timbrado para reenviarlo
              byte[] bytesXML = Encoding.ASCII.GetBytes(CFD_comprobanteStr);
              //string base64_XML = Convert.ToBase64String(bytesXML);
              Attachments.Add(Transaccionidpeticionwebservice + ".xml", bytesXML);
              #endregion

              #region Generamos PDF de factura
              //CFD_comprobanteStr = "'<?xml version=\"1.0\" encoding=\"utf-8\"?><cfdi:Comprobante xmlns:cfdi=\"http://www.sat.gob.mx/cfd/4\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:schemaLocation=\"http://www.sat.gob.mx/cfd/4 http://www.sat.gob.mx/sitio_internet/cfd/4/cfdv40.xsd\" Version=\"4.0\" Serie=\"F\" Folio=\"105\" Fecha=\"2024-02-19T16:35:40\" Moneda=\"MXN\" TipoCambio=\"1\" SubTotal=\"232.76\" Total=\"270.00\" FormaPago=\"01\" TipoDeComprobante=\"I\" MetodoPago=\"PUE\" LugarExpedicion=\"50740\" Exportacion=\"01\" NoCertificado=\"30001000000500003444\" Certificado=\"MIIFoTCCA4mgAwIBAgIUMzAwMDEwMDAwMDA1MDAwMDM0NDQwDQYJKoZIhvcNAQELBQAwggErMQ8wDQYDVQQDDAZBQyBVQVQxLjAsBgNVBAoMJVNFUlZJQ0lPIERFIEFETUlOSVNUUkFDSU9OIFRSSUJVVEFSSUExGjAYBgNVBAsMEVNBVC1JRVMgQXV0aG9yaXR5MSgwJgYJKoZIhvcNAQkBFhlvc2Nhci5tYXJ0aW5lekBzYXQuZ29iLm14MR0wGwYDVQQJDBQzcmEgY2VycmFkYSBkZSBjYWxpejEOMAwGA1UEEQwFMDYzNzAxCzAJBgNVBAYTAk1YMRkwFwYDVQQIDBBDSVVEQUQgREUgTUVYSUNPMREwDwYDVQQHDAhDT1lPQUNBTjERMA8GA1UELRMIMi41LjQuNDUxJTAjBgkqhkiG9w0BCQITFnJlc3BvbnNhYmxlOiBBQ0RNQS1TQVQwHhcNMjMwNTE4MTM1MjIzWhcNMjcwNTE4MTM1MjIzWjCByDEiMCAGA1UEAxMZSklNRU5FWiBFU1RSQURBIFNBTEFTIEEgQTEiMCAGA1UEKRMZSklNRU5FWiBFU1RSQURBIFNBTEFTIEEgQTEiMCAGA1UEChMZSklNRU5FWiBFU1RSQURBIFNBTEFTIEEgQTElMCMGA1UELRMcSkVTOTAwMTA5UTkwIC8gVkFEQTgwMDkyN0RKMzEeMBwGA1UEBRMVIC8gVkFEQTgwMDkyN0hTUlNSTDA1MRMwEQYDVQQLEwpTdWN1cnNhbCAyMIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAiCXqWezB86LQauOQR0kCeowaCnqRb9rbzpcnSU5o9fPngPaa+U5Y0ZMkRbH/KOdXtIxZd4ZZmjv8h1wUuY6gEy0cknr+TTS4q+s4OXrlOUNFmzee/lSRJxLW7zEdfXNWO36slzYplMmmMWYQuFs6KI51GeB6U2fRFBdbhnr4CJY2T7/1foU8LkwTNIaXe3Ftr34t9SrDSpQeYSTSKhuQaf3RR5/n7ePCuHgoqGP8U3UyUxSaKsdNVu9W1bPnNYNqQra4hP8EahguTsSkZzbaK94qS2L4oz4CcqFVxCsjm0ZwWwfvHQ5TPKAmGBd8L4mW7Yb6qPQZmhU4aRSGrMMGeQIDAQABox0wGzAMBgNVHRMBAf8EAjAAMAsGA1UdDwQEAwIGwDANBgkqhkiG9w0BAQsFAAOCAgEAXVn15XA/xnwkvbWOkPHEXJTlmYxKnv8eOctybWlOT7NNzqm9Kt1Bu2PtYPaKrvcBYKezT2DARsp2HJNmU/oGPFL2hDFqVac+FLuYfiWY1GBMiNtnOu9GqiPoualKq1UM4dUspuir6ccRf33glbi4m8al89Rz+vZSRSRL44iYWQ4p3w6kxEk23MT3R8eQG4pGEcHYfkUchibT1BUUex3alW1xH9BuST5zHh8zii+aWkCC/UP2mTomEmMf4vJc0xVtnoNb8lDvYNwCO4ZODc0pssO8QaC4kEmiSqOeGxghORGkvqd0joDZ2cnmQ8ilgWJ+RBVQzxny/hem3AQFzP5CGFgZQ+I6frMV97jZzPbOqnLKyjre6Fc+SjPN+ExfXM0FrGMxeeXMzBubrD8UkmG/OT5nZI7urqXVOywRYbQkMVf+19BLiuTYHctBTxjjQ7HNWz5SS2qeKzPvKL4Av7828hBsweRpi8+KVqnxToDzH0FPNiYDexLg/QQKF9Dh4ryEYoMc54YpfC3H/Q+GMmUeMLf43mv+4bcSFwnKEDswY38V4ooOP4v9IIK3z7YP9+Gk4Vu4+0kTQrI1KuIiRYCGEQPOLWi6WcYGUHHO9FDz5hV0cYsC81QbWelSSqxazrrhb9SnbtYRqlYzdpY+S9gecBx+JKvDArJTypgomMUer44=\" Sello=\"V3o6iP5ZkO8Q31d5nN5Uhemf5GJWoOWMbQKVtIg4OTciZnCEmOy1BeI/efG9EcvR+WZ44L6dBF2sHeQrWLUGNK3LH9Q0VKk9fucLzMdmSL0RMYYt+wW/fra/xE/ajgjpUttdmZm2fjwy1D9EEKO/8Hxahj7Qab5WwxsRg0N4T5gLvtvmOTf0HkYPrSQFXWlFsNbYalEezerdYxy3rYptS2N8LPg37LvlsjxfKbyT4U7jWQkdPzeSFcpJLcyX5Rk8r2uWmqh8i2qzja7q/Ceq1PhyjpKzj8riOsU11mHoEHkCNMWP727VyjuHTpTDqYoiAZB/pvHH7Reyt7QDbll3yA==\">\r\n  <cfdi:Emisor Rfc=\"ATP561002J67\" Nombre=\"Kratos Consultores\" RegimenFiscal=\"624\" />\r\n  <cfdi:Receptor Rfc=\"UIC980724RXA\" Nombre=\"EDUARDO REYES MORENO\" UsoCFDI=\"G03\" DomicilioFiscalReceptor=\"50740\" RegimenFiscalReceptor=\"601\" />\r\n  <cfdi:Conceptos>\r\n    <cfdi:Concepto ClaveProdServ=\"78111802\" NoIdentificacion=\"SUV12345\" Cantidad=\"1\" ClaveUnidad=\"E48\" Descripcion=\"SERVICIO PUBLICO DE TRANSPORTE DE PERSONAS\" ValorUnitario=\"232.76\" Importe=\"232.76\" ObjetoImp=\"02\">\r\n      <cfdi:Impuestos>\r\n        <cfdi:Traslados>\r\n          <cfdi:Traslado Base=\"232.76\" Impuesto=\"002\" TipoFactor=\"Tasa\" TasaOCuota=\"0.160000\" Importe=\"37.24\" />\r\n        </cfdi:Traslados>\r\n      </cfdi:Impuestos>\r\n    </cfdi:Concepto>\r\n  </cfdi:Conceptos>\r\n  <cfdi:Impuestos TotalImpuestosTrasladados=\"37.24\">\r\n    <cfdi:Traslados>\r\n      <cfdi:Traslado Impuesto=\"002\" TipoFactor=\"Tasa\" TasaOCuota=\"0.160000\" Importe=\"37.24\" Base=\"232.76\" />\r\n    </cfdi:Traslados>\r\n  </cfdi:Impuestos>\r\n  <cfdi:Complemento>\r\n    <tfd:TimbreFiscalDigital xmlns:tfd=\"http://www.sat.gob.mx/TimbreFiscalDigital\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:schemaLocation=\"http://www.sat.gob.mx/TimbreFiscalDigital http://www.sat.gob.mx/sitio_internet/cfd/TimbreFiscalDigital/TimbreFiscalDigitalv11.xsd\" Version=\"1.1\" UUID=\"1D59A583-3443-458C-B67C-B11B01745A05\" FechaTimbrado=\"2024-02-19T16:35:41\" RfcProvCertif=\"PAC010101000\" SelloCFD=\"V3o6iP5ZkO8Q31d5nN5Uhemf5GJWoOWMbQKVtIg4OTciZnCEmOy1BeI/efG9EcvR+WZ44L6dBF2sHeQrWLUGNK3LH9Q0VKk9fucLzMdmSL0RMYYt+wW/fra/xE/ajgjpUttdmZm2fjwy1D9EEKO/8Hxahj7Qab5WwxsRg0N4T5gLvtvmOTf0HkYPrSQFXWlFsNbYalEezerdYxy3rYptS2N8LPg37LvlsjxfKbyT4U7jWQkdPzeSFcpJLcyX5Rk8r2uWmqh8i2qzja7q/Ceq1PhyjpKzj8riOsU11mHoEHkCNMWP727VyjuHTpTDqYoiAZB/pvHH7Reyt7QDbll3yA==\" NoCertificadoSAT=\"00000000000000000000\" SelloSAT=\"AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA==\" />\r\n  </cfdi:Complemento>\r\n</cfdi:Comprobante>'";
              HtmlContent = Fac.GeneraHtmlPDF(CFD_comprobanteStr, CFD_codigoDeBarras, CFD_cadenaOriginal, TFD_foliofiscalUUID, TFD_FechaTimbrado, TFD_noCertificadoSAT, TFD_selloSAT);
              byte[]? bytesPDF = Utilities.PDF.getBytesPDF(HtmlContent);
              //string base64_PDF = Convert.ToBase64String(bytesPDF);
              Attachments.Add(Transaccionidpeticionwebservice + ".pdf", bytesPDF);
              #endregion

              #region Enviamos correo
              await _notificacionCorreoService.EnvioFactura(emailUser, string.Empty, string.Empty, string.Empty, string.Empty, Attachments);
              #endregion

              response.Mensaje = "Solicitud exitosa";
              response.CodigoMensaje = "200";
              result.Facturado = true;
            }
            else
            {
              //Error al consumir el WS
              var LogerrorfacturaAdd = new Database.Entities.Logerrorfactura()
              {
                UsuarioIdusuario = userId,
                Respuestaservicio = RespuestaWS,
                Fecharegistro = DateTime.Now,
                Xmlrequest = strxml
              };
              _context.Logerrorfacturas.Add(LogerrorfacturaAdd);

              await _context.SaveChangesAsync();
              response.Mensaje = "Error al generar factura";
              response.CodigoMensaje = "400";
              result.Facturado = false;
            }
          }
          else
          {
            //Error al consumir el WS
            var LogerrorfacturaAdd = new Database.Entities.Logerrorfactura()
            {
              UsuarioIdusuario = userId,
              Respuestaservicio = RespuestaWS,
              Fecharegistro = DateTime.Now,
              Xmlrequest = strxml
            };
            _context.Logerrorfacturas.Add(LogerrorfacturaAdd);

            await _context.SaveChangesAsync();
            response.Mensaje = "Error al generar factura";
            response.CodigoMensaje = "400";
            result.Facturado = false;
          }
        }
      }
      else
      {

        var DatosTransaccionViaje = await (from t in _context.Transaccions
                                           join v in _context.Viajes on t.Idtransaccion equals v.TransaccionIdtransaccion
                                           join f in _context.Facturas on t.Idtransaccion equals f.TransaccionIdtransaccion
                                           where v.Idviaje == data.viaje_id
                                           select new
                                           {
                                             f.Transaccionidpeticionwebservice,
                                             f.Cadenaoriginal,
                                             f.Codigobarras,
                                             f.Nocertificadocsd,
                                             f.FoliofiscalUuid,
                                             f.Fechatimbrado,
                                             f.Certificadosat,
                                             f.Sellosat,
                                             f.Xmltimbrado

                                           }).FirstOrDefaultAsync();

        if (DatosTransaccionViaje == null)
        {
          response.CodigoMensaje = "400";
          response.Mensaje = "No se encontro información para enviar factura" + data.viaje_id;
          return response;
        }
        else
        {
          Transaccionidpeticionwebservice = Guid.Parse(DatosTransaccionViaje.Transaccionidpeticionwebservice);
          CFD_comprobanteStr = DatosTransaccionViaje.Xmltimbrado;
          CFD_codigoDeBarras = DatosTransaccionViaje.Codigobarras;
          CFD_cadenaOriginal = DatosTransaccionViaje.Cadenaoriginal;
          TFD_foliofiscalUUID = DatosTransaccionViaje.FoliofiscalUuid;
          TFD_FechaTimbrado = DatosTransaccionViaje.Fechatimbrado.ToString();
          TFD_noCertificadoSAT = DatosTransaccionViaje.Certificadosat;
          TFD_selloSAT = DatosTransaccionViaje.Sellosat;
        }
        #region Obtenemos XML Timbrado para reenviarlo
        byte[] bytesXML = Encoding.ASCII.GetBytes(CFD_comprobanteStr);
        //string base64_XML = Convert.ToBase64String(bytesXML);
        Attachments.Add(Transaccionidpeticionwebservice + ".xml", bytesXML);
        #endregion

        #region Generamos PDF de factura
        //CFD_comprobanteStr = "'<?xml version=\"1.0\" encoding=\"utf-8\"?><cfdi:Comprobante xmlns:cfdi=\"http://www.sat.gob.mx/cfd/4\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:schemaLocation=\"http://www.sat.gob.mx/cfd/4 http://www.sat.gob.mx/sitio_internet/cfd/4/cfdv40.xsd\" Version=\"4.0\" Serie=\"F\" Folio=\"105\" Fecha=\"2024-02-19T16:35:40\" Moneda=\"MXN\" TipoCambio=\"1\" SubTotal=\"232.76\" Total=\"270.00\" FormaPago=\"01\" TipoDeComprobante=\"I\" MetodoPago=\"PUE\" LugarExpedicion=\"50740\" Exportacion=\"01\" NoCertificado=\"30001000000500003444\" Certificado=\"MIIFoTCCA4mgAwIBAgIUMzAwMDEwMDAwMDA1MDAwMDM0NDQwDQYJKoZIhvcNAQELBQAwggErMQ8wDQYDVQQDDAZBQyBVQVQxLjAsBgNVBAoMJVNFUlZJQ0lPIERFIEFETUlOSVNUUkFDSU9OIFRSSUJVVEFSSUExGjAYBgNVBAsMEVNBVC1JRVMgQXV0aG9yaXR5MSgwJgYJKoZIhvcNAQkBFhlvc2Nhci5tYXJ0aW5lekBzYXQuZ29iLm14MR0wGwYDVQQJDBQzcmEgY2VycmFkYSBkZSBjYWxpejEOMAwGA1UEEQwFMDYzNzAxCzAJBgNVBAYTAk1YMRkwFwYDVQQIDBBDSVVEQUQgREUgTUVYSUNPMREwDwYDVQQHDAhDT1lPQUNBTjERMA8GA1UELRMIMi41LjQuNDUxJTAjBgkqhkiG9w0BCQITFnJlc3BvbnNhYmxlOiBBQ0RNQS1TQVQwHhcNMjMwNTE4MTM1MjIzWhcNMjcwNTE4MTM1MjIzWjCByDEiMCAGA1UEAxMZSklNRU5FWiBFU1RSQURBIFNBTEFTIEEgQTEiMCAGA1UEKRMZSklNRU5FWiBFU1RSQURBIFNBTEFTIEEgQTEiMCAGA1UEChMZSklNRU5FWiBFU1RSQURBIFNBTEFTIEEgQTElMCMGA1UELRMcSkVTOTAwMTA5UTkwIC8gVkFEQTgwMDkyN0RKMzEeMBwGA1UEBRMVIC8gVkFEQTgwMDkyN0hTUlNSTDA1MRMwEQYDVQQLEwpTdWN1cnNhbCAyMIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAiCXqWezB86LQauOQR0kCeowaCnqRb9rbzpcnSU5o9fPngPaa+U5Y0ZMkRbH/KOdXtIxZd4ZZmjv8h1wUuY6gEy0cknr+TTS4q+s4OXrlOUNFmzee/lSRJxLW7zEdfXNWO36slzYplMmmMWYQuFs6KI51GeB6U2fRFBdbhnr4CJY2T7/1foU8LkwTNIaXe3Ftr34t9SrDSpQeYSTSKhuQaf3RR5/n7ePCuHgoqGP8U3UyUxSaKsdNVu9W1bPnNYNqQra4hP8EahguTsSkZzbaK94qS2L4oz4CcqFVxCsjm0ZwWwfvHQ5TPKAmGBd8L4mW7Yb6qPQZmhU4aRSGrMMGeQIDAQABox0wGzAMBgNVHRMBAf8EAjAAMAsGA1UdDwQEAwIGwDANBgkqhkiG9w0BAQsFAAOCAgEAXVn15XA/xnwkvbWOkPHEXJTlmYxKnv8eOctybWlOT7NNzqm9Kt1Bu2PtYPaKrvcBYKezT2DARsp2HJNmU/oGPFL2hDFqVac+FLuYfiWY1GBMiNtnOu9GqiPoualKq1UM4dUspuir6ccRf33glbi4m8al89Rz+vZSRSRL44iYWQ4p3w6kxEk23MT3R8eQG4pGEcHYfkUchibT1BUUex3alW1xH9BuST5zHh8zii+aWkCC/UP2mTomEmMf4vJc0xVtnoNb8lDvYNwCO4ZODc0pssO8QaC4kEmiSqOeGxghORGkvqd0joDZ2cnmQ8ilgWJ+RBVQzxny/hem3AQFzP5CGFgZQ+I6frMV97jZzPbOqnLKyjre6Fc+SjPN+ExfXM0FrGMxeeXMzBubrD8UkmG/OT5nZI7urqXVOywRYbQkMVf+19BLiuTYHctBTxjjQ7HNWz5SS2qeKzPvKL4Av7828hBsweRpi8+KVqnxToDzH0FPNiYDexLg/QQKF9Dh4ryEYoMc54YpfC3H/Q+GMmUeMLf43mv+4bcSFwnKEDswY38V4ooOP4v9IIK3z7YP9+Gk4Vu4+0kTQrI1KuIiRYCGEQPOLWi6WcYGUHHO9FDz5hV0cYsC81QbWelSSqxazrrhb9SnbtYRqlYzdpY+S9gecBx+JKvDArJTypgomMUer44=\" Sello=\"V3o6iP5ZkO8Q31d5nN5Uhemf5GJWoOWMbQKVtIg4OTciZnCEmOy1BeI/efG9EcvR+WZ44L6dBF2sHeQrWLUGNK3LH9Q0VKk9fucLzMdmSL0RMYYt+wW/fra/xE/ajgjpUttdmZm2fjwy1D9EEKO/8Hxahj7Qab5WwxsRg0N4T5gLvtvmOTf0HkYPrSQFXWlFsNbYalEezerdYxy3rYptS2N8LPg37LvlsjxfKbyT4U7jWQkdPzeSFcpJLcyX5Rk8r2uWmqh8i2qzja7q/Ceq1PhyjpKzj8riOsU11mHoEHkCNMWP727VyjuHTpTDqYoiAZB/pvHH7Reyt7QDbll3yA==\">\r\n  <cfdi:Emisor Rfc=\"ATP561002J67\" Nombre=\"Kratos Consultores\" RegimenFiscal=\"624\" />\r\n  <cfdi:Receptor Rfc=\"UIC980724RXA\" Nombre=\"EDUARDO REYES MORENO\" UsoCFDI=\"G03\" DomicilioFiscalReceptor=\"50740\" RegimenFiscalReceptor=\"601\" />\r\n  <cfdi:Conceptos>\r\n    <cfdi:Concepto ClaveProdServ=\"78111802\" NoIdentificacion=\"SUV12345\" Cantidad=\"1\" ClaveUnidad=\"E48\" Descripcion=\"SERVICIO PUBLICO DE TRANSPORTE DE PERSONAS\" ValorUnitario=\"232.76\" Importe=\"232.76\" ObjetoImp=\"02\">\r\n      <cfdi:Impuestos>\r\n        <cfdi:Traslados>\r\n          <cfdi:Traslado Base=\"232.76\" Impuesto=\"002\" TipoFactor=\"Tasa\" TasaOCuota=\"0.160000\" Importe=\"37.24\" />\r\n        </cfdi:Traslados>\r\n      </cfdi:Impuestos>\r\n    </cfdi:Concepto>\r\n  </cfdi:Conceptos>\r\n  <cfdi:Impuestos TotalImpuestosTrasladados=\"37.24\">\r\n    <cfdi:Traslados>\r\n      <cfdi:Traslado Impuesto=\"002\" TipoFactor=\"Tasa\" TasaOCuota=\"0.160000\" Importe=\"37.24\" Base=\"232.76\" />\r\n    </cfdi:Traslados>\r\n  </cfdi:Impuestos>\r\n  <cfdi:Complemento>\r\n    <tfd:TimbreFiscalDigital xmlns:tfd=\"http://www.sat.gob.mx/TimbreFiscalDigital\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:schemaLocation=\"http://www.sat.gob.mx/TimbreFiscalDigital http://www.sat.gob.mx/sitio_internet/cfd/TimbreFiscalDigital/TimbreFiscalDigitalv11.xsd\" Version=\"1.1\" UUID=\"1D59A583-3443-458C-B67C-B11B01745A05\" FechaTimbrado=\"2024-02-19T16:35:41\" RfcProvCertif=\"PAC010101000\" SelloCFD=\"V3o6iP5ZkO8Q31d5nN5Uhemf5GJWoOWMbQKVtIg4OTciZnCEmOy1BeI/efG9EcvR+WZ44L6dBF2sHeQrWLUGNK3LH9Q0VKk9fucLzMdmSL0RMYYt+wW/fra/xE/ajgjpUttdmZm2fjwy1D9EEKO/8Hxahj7Qab5WwxsRg0N4T5gLvtvmOTf0HkYPrSQFXWlFsNbYalEezerdYxy3rYptS2N8LPg37LvlsjxfKbyT4U7jWQkdPzeSFcpJLcyX5Rk8r2uWmqh8i2qzja7q/Ceq1PhyjpKzj8riOsU11mHoEHkCNMWP727VyjuHTpTDqYoiAZB/pvHH7Reyt7QDbll3yA==\" NoCertificadoSAT=\"00000000000000000000\" SelloSAT=\"AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA==\" />\r\n  </cfdi:Complemento>\r\n</cfdi:Comprobante>'";
        HtmlContent = Fac.GeneraHtmlPDF(CFD_comprobanteStr, CFD_codigoDeBarras, CFD_cadenaOriginal, TFD_foliofiscalUUID, TFD_FechaTimbrado, TFD_noCertificadoSAT, TFD_selloSAT);
        byte[]? bytesPDF = Utilities.PDF.getBytesPDF(HtmlContent);
        //string base64_PDF = Convert.ToBase64String(bytesPDF);
        Attachments.Add(Transaccionidpeticionwebservice + ".pdf", bytesPDF);
        #endregion

        #region Enviamos correo
        await _notificacionCorreoService.EnvioFactura(emailUser, string.Empty, string.Empty, string.Empty, string.Empty, Attachments);
        #endregion
        response.Mensaje = "Solicitud exitosa";
        response.CodigoMensaje = "200";
        result.Facturado = true;
      }

      response.Data = result;
      return response;
    }

    private async Task<DatosFacturacionReceptor> getDatosFacturacionReceptor(int userId, int perfil_facturacion_id)
    {
      return await (from dfr in _context.Datosfacturacionreceptors
                    join u in _context.Usuarios on dfr.UsuarioIdusuario equals u.Idusuario
                    join us in _context.Usoscfdireceptors on dfr.UsoscfdireceptorIdusoscfdireceptor equals us.Idusoscfdireceptor
                    join rf in _context.Regimenfiscalreceptors on dfr.RegimenfiscalreceptorIdregimenfiscalreceptor equals rf.Idregimenfiscalreceptor
                    where u.Idusuario == userId
                    && dfr.Iddatosfacturacionreceptor == perfil_facturacion_id
                    select new DatosFacturacionReceptor
                    {
                      NombreReceptor = dfr.Nombreorazonsocial,
                      RfcReceptor = dfr.Rfc,
                      UsoCfdiReceptor = us.Clave,
                      DomicilioFiscalReceptor = dfr.Codigopostal,
                      RegimenFiscalReceptor = rf.Clave
                    }).FirstOrDefaultAsync();
    }

    private async Task<DatosFacturacionEmisor> getDatosFacturacionEmisor(int EmpresaId)
    {
      DatosFacturacionEmisor datos = await (from e in _context.Empresas
                                            join dfe in _context.Datosfacturacionemisors on e.Idempresa equals dfe.EmpresaIdempresa
                                            where e.Idempresa == EmpresaId
                                            select new DatosFacturacionEmisor
                                            {
                                              NombreEmpresa = e.Nombre,
                                              RfcEmpresa = e.Rfc,
                                              Serie = dfe.Serie,
                                              Folio = (int)dfe.Folio,
                                              RegimenfiscalEmisor = dfe.Regimenfiscal,
                                              LugarexpedicionCp = dfe.LugarexpedicionCp
                                            }).FirstOrDefaultAsync();
            var rfe = await _context.Regimenfiscalreceptors.FirstOrDefaultAsync(x => x.Idregimenfiscalreceptor == int.Parse(datos.RegimenfiscalEmisor));
            datos.RegimenfiscalEmisor = rfe.Clave;

      return datos;
    }

    private async Task<Datosfacturacionproducto?> getDatosfacturacionproductos()
    {
      return await _context.Datosfacturacionproductos.FirstOrDefaultAsync();
    }

    private async Task<ModelDatosTransaccion> getDatosTransaccionViaje(int viaje_id)
    {
      ModelDatosTransaccion DatosTransaccion = null;

      DatosTransaccion = await (from t in _context.Transaccions
                                join v in _context.Viajes on t.Idtransaccion equals v.TransaccionIdtransaccion
                                where v.Idviaje == viaje_id
                                && (t.EstatustransaccionIdestatustransaccion == Convert.ToInt32(EnumEstatusPago.COMPLETED)
                                || t.EstatustransaccionIdestatustransaccion == Convert.ToInt32(EnumEstatusPago.AUTHORIZED))
                                && (v.EstatusviajeIdestatusviaje == Convert.ToInt32(EnumEstatusViaje.EN_CURSO)
                                || v.EstatusviajeIdestatusviaje == Convert.ToInt32(EnumEstatusViaje.FINALIZADO)
                                || v.EstatusviajeIdestatusviaje == Convert.ToInt32(EnumEstatusViaje.PERDIDO))
                                select new ModelDatosTransaccion
                                {
                                  Idtransaccion = t.Idtransaccion,
                                  MetodopagoIdmetodopago = t.MetodopagoIdmetodopago,
                                  Cantidad = (decimal)t.Cantidad,
                                  TipotransaccionIdtipotransaccion = t.TipotransaccionIdtipotransaccion,
                                  UsuarioIdusuario = t.UsuarioIdusuario,
                                  Numeroordenpay = t.Numeroordenpay,
                                  Numeropeticionpay = t.Numeropeticionpay,
                                  Idviaje = v.Idviaje,
                                  EmpresaIdempresa = (int)v.EmpresaIdempresa,
                                  Facturado = (bool)v.Facturado == null ? false : (bool)v.Facturado
                                }).FirstOrDefaultAsync();

      return DatosTransaccion;
    }

    public async Task<SuVanResponse<GuardaDatosFacturacionReceptorResponse>> GuardaDatosFacturacionReceptor(int userId, GuardaDatosFacturacionReceptorRequest data)
    {

      GuardaDatosFacturacionReceptorResponse? result = new GuardaDatosFacturacionReceptorResponse();
      SuVanResponse<GuardaDatosFacturacionReceptorResponse> response = new();

      #region Validamos datos de entrada
      if (data.RFC.Length != 12 && data.RFC.Length != 13)
      {
        response.CodigoMensaje = "400";
        response.Mensaje = "El valor RFC es incorrecto";
        return response;
      }

      if (data.CodigoPostal.Length != 5)
      {
        response.CodigoMensaje = "400";
        response.Mensaje = "El valor CodigoPostal es incorrecto";
        return response;
      }

      if (data.UsoCFDIId <= 0)
      {
        response.CodigoMensaje = "400";
        response.Mensaje = "El valor UsoCFDIId es incorrecto";
        return response;
      }

      if (data.RegimenFiscalId <= 0)
      {
        response.CodigoMensaje = "400";
        response.Mensaje = "El valor RegimenFiscalId es incorrecto";
        return response;
      }
      #endregion


      //Datosfacturacionreceptor? DatosfacturacionreceptorEntity = _context.Datosfacturacionreceptors.FirstOrDefault(x => x.UsuarioIdusuario == userId);

      //if (DatosfacturacionreceptorEntity == null)
      //{
      var DatosfacturacionreceptorEntityAdd = new Database.Entities.Datosfacturacionreceptor()
      {
        UsuarioIdusuario = userId,
        Rfc = data.RFC.ToUpper(),
        Nombreorazonsocial = data.NombreRazonSocial,
        UsoscfdireceptorIdusoscfdireceptor = data.UsoCFDIId,
        RegimenfiscalreceptorIdregimenfiscalreceptor = data.RegimenFiscalId,
        Codigopostal = data.CodigoPostal,
        Fecharegistro = DateTime.Now
      };
      _context.Datosfacturacionreceptors.Add(DatosfacturacionreceptorEntityAdd);
      //}
      //else
      //{
      //  DatosfacturacionreceptorEntity.Nombreorazonsocial = data.NombreRazonSocial;
      //  DatosfacturacionreceptorEntity.UsoscfdireceptorIdusoscfdireceptor = data.UsoCFDIId;
      //  DatosfacturacionreceptorEntity.RegimenfiscalreceptorIdregimenfiscalreceptor = data.RegimenFiscalId;
      //  DatosfacturacionreceptorEntity.Codigopostal = data.CodigoPostal;
      //  DatosfacturacionreceptorEntity.Fecharegistro = DateTime.Now;
      //  _context.Datosfacturacionreceptors.Entry(DatosfacturacionreceptorEntity);
      //}
      await _context.SaveChangesAsync();

      result.Estatus = true;
      response.Mensaje = "Solicitud exitosa";
      response.CodigoMensaje = "200";
      response.Data = result;

      return response;
    }

    public async Task<SuVanResponse<List<DatosRegimenFiscalUsoCFDIResponse.RegimenFiscalReceptorResponse>>> ObtenRegimenFiscalReceptor(int userId, string rfc)
    {
      SuVanResponse<List<DatosRegimenFiscalUsoCFDIResponse.RegimenFiscalReceptorResponse>> response = new();
      #region Validamos datos de entrada
      if (string.IsNullOrEmpty(rfc))
      {
        response.CodigoMensaje = "400";
        response.Mensaje = "El valor RFC es obligatorio";
        return response;
      }
      if (rfc.Length != 12 && rfc.Length != 13)
      {
        response.CodigoMensaje = "400";
        response.Mensaje = "El valor RFC es incorrecto";
        return response;
      }
      #endregion

      using (var context = _context)
      {
        if (rfc.Length == 12)//Moral
        {
          var result = await (from o in context.Regimenfiscalreceptors
                              where o.Moral == 1
                              && o.Activo == 1
                              select new DatosRegimenFiscalUsoCFDIResponse.RegimenFiscalReceptorResponse()
                              {
                                id = o.Idregimenfiscalreceptor,
                                clave = o.Clave,
                                descripcion = o.Descripcion
                              }).ToListAsync();
          response.Data = result;

        }
        else if (rfc.Length == 13)//Fisica
        {
          var result = await (from o in context.Regimenfiscalreceptors
                              where o.Fisica == 1
                              && o.Activo == 1
                              select new DatosRegimenFiscalUsoCFDIResponse.RegimenFiscalReceptorResponse()
                              {
                                id = o.Idregimenfiscalreceptor,
                                clave = o.Clave,
                                descripcion = o.Descripcion
                              }).ToListAsync();
          response.Data = result;
        }
      }

      response.CodigoMensaje = "200";
      response.Mensaje = "Búsqueda exitosa";
      return response;

    }

    public async Task<SuVanResponse<List<DatosRegimenFiscalUsoCFDIResponse.UsoCFDIReceptorResponse>>> ObtenUsoCFDIReceptor(int userId, string rfc, int idregimenfiscalreceptor)
    {
      SuVanResponse<List<DatosRegimenFiscalUsoCFDIResponse.UsoCFDIReceptorResponse>> response = new();
      #region Validamos datos de entrada
      if (string.IsNullOrEmpty(rfc))
      {
        response.CodigoMensaje = "400";
        response.Mensaje = "El valor RFC es obligatorio";
        return response;
      }
      if (rfc.Length != 12 && rfc.Length != 13)
      {
        response.CodigoMensaje = "400";
        response.Mensaje = "El valor RFC es incorrecto";
        return response;
      }

      if (idregimenfiscalreceptor <= 0)
      {
        response.CodigoMensaje = "400";
        response.Mensaje = "El valor idregimenfiscalreceptor es incorrecto";
        return response;
      }
      #endregion

      using (var context = _context)
      {
        if (rfc.Length == 12)//Moral
        {
          var result = await (from rel in context.UsoscfdireceptorRegimenfiscalreceptors
                              join u in context.Usoscfdireceptors on rel.Idusoscfdireceptor equals u.Idusoscfdireceptor
                              join rf in context.Regimenfiscalreceptors on rel.Idregimenfiscalreceptor equals rf.Idregimenfiscalreceptor
                              where rf.Idregimenfiscalreceptor == idregimenfiscalreceptor
                              && u.Activo == 1
                              && u.Moral == 1
                              && rf.Activo == 1
                              && rf.Moral == 1
                              && rel.Activo == 1
                              && rel.Moral == 1
                              select new DatosRegimenFiscalUsoCFDIResponse.UsoCFDIReceptorResponse()
                              {
                                id = u.Idusoscfdireceptor,
                                clave = u.Clave,
                                descripcion = u.Descripcion
                              }).ToListAsync();
          response.Data = result;

        }
        else if (rfc.Length == 13)//Fisica
        {
          var result = await (from rel in context.UsoscfdireceptorRegimenfiscalreceptors
                              join u in context.Usoscfdireceptors on rel.Idusoscfdireceptor equals u.Idusoscfdireceptor
                              join rf in context.Regimenfiscalreceptors on rel.Idregimenfiscalreceptor equals rf.Idregimenfiscalreceptor
                              where rf.Idregimenfiscalreceptor == idregimenfiscalreceptor
                              && u.Activo == 1
                              && u.Fisica == 1
                              && rf.Activo == 1
                              && rf.Fisica == 1
                              && rel.Activo == 1
                              && rel.Fisica == 1
                              select new DatosRegimenFiscalUsoCFDIResponse.UsoCFDIReceptorResponse()
                              {
                                id = u.Idusoscfdireceptor,
                                clave = u.Clave,
                                descripcion = u.Descripcion
                              }).ToListAsync();
          response.Data = result;
        }
      }

      response.CodigoMensaje = "200";
      response.Mensaje = "Búsqueda exitosa";
      return response;
    }


    public async Task<SuVanResponse<List<ObtenDatosFacturacionReceptorResponse>>> ObtenDatosFacturacionReceptor(int userId)
    {
      SuVanResponse<List<ObtenDatosFacturacionReceptorResponse>> response = new();
      var result = await (from dfr in _context.Datosfacturacionreceptors
                          join u in _context.Usoscfdireceptors on dfr.UsoscfdireceptorIdusoscfdireceptor equals u.Idusoscfdireceptor
                          join rf in _context.Regimenfiscalreceptors on dfr.RegimenfiscalreceptorIdregimenfiscalreceptor equals rf.Idregimenfiscalreceptor
                          where dfr.UsuarioIdusuario == userId

                          select new ObtenDatosFacturacionReceptorResponse()
                          {
                            Iddatosfacturacionreceptor = dfr.Iddatosfacturacionreceptor,
                            NombreRazonSocial = dfr.Nombreorazonsocial,
                            RFC = dfr.Rfc,
                            CodigoPostal = dfr.Codigopostal,
                            RegimenFiscalId = rf.Idregimenfiscalreceptor,
                            RegimenFiscalClave = rf.Clave,
                            RegimenFiscalDescripcion = rf.Descripcion,
                            UsoCFDIId = u.Idusoscfdireceptor,
                            UsoCFDIClave = u.Clave,
                            UsoCFDIDescripcion = u.Descripcion
                          }).ToListAsync();


      response.CodigoMensaje = "200";
      response.Mensaje = "Búsqueda exitosa";

      response.Data = result;
      return response;
    }


    public async Task<SuVanResponse<string>> RemueveDatosFacturacionReceptor(int datosfacturacionreceptorId, int userId)
    {
      SuVanResponse<string> response = new();

      var DatosfacturacionreceptorsEntity = await _context.Datosfacturacionreceptors.FirstOrDefaultAsync(
        x => x.Iddatosfacturacionreceptor == datosfacturacionreceptorId
        && x.UsuarioIdusuario == userId);

      if (DatosfacturacionreceptorsEntity == null)
      {
        response.CodigoMensaje = "401";
        response.Mensaje = "No se encontro información";
        response.Data = null;
        return response;
      }
      _context.Datosfacturacionreceptors.Remove(DatosfacturacionreceptorsEntity);
      await _context.SaveChangesAsync();
      response.CodigoMensaje = "200";
      response.Mensaje = "Registro eliminado";
      response.Data = null;
      return response;
    }


    public string ObtenLiga(int EmpresaId)
    {

      var variables = (from a in _context.Variableempresas
                       where a.VariableIdvariable == 14 && a.EmpresaIdempresa == EmpresaId
                       select a).FirstOrDefault();

      return variables.Valor;
    }

  }
}

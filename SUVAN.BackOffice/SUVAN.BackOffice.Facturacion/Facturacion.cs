using Microsoft.VisualBasic;
using SUVAN.BackOffice.Models.Facturacion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace SUVAN.Facturacion
{
	public class Facturacion
	{

		public string TimbraFactura(string strxml, string url)
		{
			string RespuestaWS = string.Empty;

			strxml = Strings.Replace(Strings.Replace(strxml, @"\'", "\""), "  ", " ");
			XElement xmlElement = XElement.Parse(strxml);


			string Transaccion_EstatusTimbrado = string.Empty;//Transaccion
			string Transaccion_Tipo = string.Empty;//Tipo

			string CFD_codigoDeBarras = string.Empty;//CFD
			string CFD_selloEmisor = string.Empty;//CFD
			string CFD_Folio = string.Empty;//CFD
			string CFD_cadenaOriginal = string.Empty;//CFD
			string CFD_noCertificado = string.Empty;///CFD

			string TFD_foliofiscalUUID = string.Empty;//TFD
			string TFD_FechaTimbrado = string.Empty;//TFD
			string TFD_noCertificadoSAT = string.Empty;//TFD
			string TFD_selloSAT = string.Empty;//CFD

			using (StringReader stringReader = new StringReader(strxml))
			{
				XmlSerializer serializer = new XmlSerializer(typeof(XmlDocument));
				XmlDocument xmlDoc = (XmlDocument)serializer.Deserialize(stringReader);

				ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
				FacturacionPegaso.emitirCFDRequest req = new FacturacionPegaso.emitirCFDRequest(xmlDoc.DocumentElement);
				FacturacionPegaso.EmisionBaseExternalServiceClient client = new FacturacionPegaso.EmisionBaseExternalServiceClient(FacturacionPegaso.EmisionBaseExternalServiceClient.EndpointConfiguration.BasicHttpBinding_IEmisionBaseExternalService);

				client.Endpoint.Address = new System.ServiceModel.EndpointAddress(url);

				try
				{
					FacturacionPegaso.emitirCFDResponse Timbrado = client.emitirCFD(req);

					if (Timbrado != null)
					{
						RespuestaWS = Timbrado.emitirCFDResult.OuterXml.ToString();
					}
				}
				catch (Exception ex)
				{
					RespuestaWS = "ERROR: " + ex.Message;
				}
			}
			return RespuestaWS;
		}

		public string XMLrequest(string Serie, int Folio, string TipoComprobante, string LugarexpedicionCp, string MetodoPago, string Exportacion, string RfcEmpresa, string RegimenfiscalEmisor, string NombreEmpresa, string RfcReceptor, string NombreReceptor, string UsoCfdiReceptor, string DomicilioFiscalReceptor, string RegimenFiscalReceptor, string Cantidad, string Claveunidad, string Claveprodserv, string Descripcion, string NoIdentificacion, string Objetoimp, string TipoComprobanteClave, string Sucursal, decimal MontoTotal, decimal PorcentajeIVA, decimal MonTotalSinIVA, decimal MontoIVA, Guid Transaccionidpeticionwebservice)
		{
			// creamos el XML
			string strxml = "";

			// ENVIAMOS LA INFORMACION DE LA FACTURA 
			strxml = @"<?xml version=\'1.0\' encoding=\'utf-8\'?> ";
			strxml += @"<RequestCFD version=\'4.0\' xmlns=\'\'> ";

			//Comprobante
			strxml += @"<Comprobante ";
			strxml += @"xmlns:cfdi=\'http://www.sat.gob.mx/cfd/3\' ";
			strxml += @"xmlns:xsi=\'http://www.w3.org/2001/XMLSchema-instance\' ";
			strxml += @"Version=\'4.0\' ";
			strxml += @"Serie=\'" + Serie + "\\' ";
			strxml += @"Folio=\'" + Folio + "\\' ";
			strxml += @"FormaPago=\'01\' ";

			strxml += @"Total=\'" + MontoTotal.ToString() + "\\' ";
			strxml += @"MetodoPago=\'" + MetodoPago + "\\' ";//PUE = Pago en una sola exhibición
			strxml += @"TipoDeComprobante=\'" + TipoComprobante + "\\' ";// I = Ingreso
			strxml += @"LugarExpedicion=\'" + LugarexpedicionCp + "\\' ";//@"LugarExpedicion=\'51130\' ";/////P E N D I E N T E 
			strxml += @"Exportacion=\'" + Exportacion + "\\' ";//01 = (No aplica)
			strxml += @"SubTotal=\'" + MonTotalSinIVA.ToString() + "\\' ";//@"SubTotal=\'100.00\' ";//Monto sin el IVA
			strxml += @"Descuento=\'0.00\'> ";

			//Informacion del EMISOR (Empresa que emite la factura)
			strxml += @"<Emisor ";
			strxml += @"Rfc=\'" + RfcEmpresa + "\\' ";//ATP561002J67
			strxml += @"RegimenFiscal=\'" + RegimenfiscalEmisor + "\\' ";
			strxml += @"Nombre=\'" + NombreEmpresa + "\\' /> ";//@"Nombre=\'AUTOTRANSPORTES DE PASAJEROS MEXICO TOLUCA SAN LUIS MEXTEPEC QUERETARO FLECHA ROJA\' />";

			//Informacion del RECEPTOR  (Datos del Usuario)
			strxml += @"<Receptor ";
			strxml += @"Rfc=\'" + RfcReceptor + "\\' ";//UIC980724RXA--Pruebas
			strxml += @"Nombre=\'" + NombreReceptor + "\\' ";
			strxml += @"UsoCFDI=\'" + UsoCfdiReceptor + "\\' ";//G03=	Gastos en general.
			strxml += @"DomicilioFiscalReceptor=\'" + DomicilioFiscalReceptor + "\\' ";//@"DomicilioFiscalReceptor=\'50740\' ";
			strxml += @"RegimenFiscalReceptor=\'" + RegimenFiscalReceptor + "\\'/> ";//Este valor se obtiene de acuerdo al valor "UsoCFDI"

			//NOTA: De acuerdo a la definición solo se va a facturar un pago en este caso el del Viaje, si fueran más pagos
			//Conceptos
			strxml += @"<Conceptos>";

			strxml += @"<Concepto ";
			strxml += @"Cantidad=\'" + Cantidad + "\\' ";
			strxml += @"ClaveUnidad=\'" + Claveunidad + "\\' ";
			strxml += @"ClaveProdServ=\'" + Claveprodserv + "\\' ";
			strxml += @"Descripcion=\'" + Descripcion + "\\' ";
			strxml += @"NoIdentificacion=\'" + NoIdentificacion + "\\' ";
			strxml += @"ValorUnitario=\'" + MonTotalSinIVA.ToString() + "\\' ";
			strxml += @"Descuento=\'0.00\' ";
			strxml += @"Importe=\'" + MonTotalSinIVA.ToString() + "\\' ";
			strxml += @"ObjetoImp=\'" + Objetoimp + "\\'> ";

			//Impuestos
			strxml += @"<Impuestos> ";
			//Traslados
			strxml += @"<Traslados> ";
			//Traslado
			strxml += @"<Traslado ";
			strxml += @"Base=\'" + MonTotalSinIVA.ToString() + "\\' ";
			strxml += @"Impuesto=\'002\' ";
			strxml += @"TasaOCuota=\'" + PorcentajeIVA.ToString() + "\\' ";
			strxml += @"TipoFactor=\'Tasa\' ";
			strxml += @"Importe=\'" + MontoIVA.ToString() + "\\' />";
			strxml += @"</Traslados> ";
			strxml += @"</Impuestos> ";

			strxml += @"</Concepto> ";
			strxml += @"</Conceptos> ";

			//Impuestos
			strxml += @"<Impuestos TotalImpuestosTrasladados=\'" + MontoIVA.ToString() + "\\'>";
			//Traslados
			strxml += @"<Traslados> ";
			strxml += @"<Traslado ";
			strxml += @"Impuesto=\'002\' ";
			strxml += @"TipoFactor=\'Tasa\' ";
			strxml += @"TasaOCuota=\'" + PorcentajeIVA.ToString() + "\\' ";
			strxml += @"Importe=\'" + MontoIVA.ToString() + "\\' ";
			strxml += @"Base=\'" + MonTotalSinIVA.ToString() + "\\' />";
			strxml += @"</Traslados> ";
			strxml += @"</Impuestos> ";

			strxml += @"</Comprobante>";


			strxml += @"<Transaccion id=\'" + Transaccionidpeticionwebservice.ToString() + "\\'/>";
			strxml += @"<TipoComprobante clave=\'" + TipoComprobanteClave + "\\'/>";
			strxml += @"<Sucursal nombre=\'" + Sucursal + "\\'/>";
			strxml += @"</RequestCFD>";
			return strxml;
		}

		public string GeneraHtmlPDF(
	string xmlsat,
	string CFD_codigoDeBarras,
	string cadenaOriginalSAT,
	string TFD_foliofiscalUUID,
	string TFD_FechaTimbradoCertificacion,
	string TFD_noCertificadoSAT,
	string TFD_selloSAT
	)
		{
			//Variables para Nodo Comprobante
			string Version = string.Empty;
			string Serie = string.Empty;
			string Folio = string.Empty;
			string FechaEmision = string.Empty;
			string Moneda = string.Empty;
			string TipoCambio = string.Empty;
			string MonTotalSinIVA = string.Empty;//Subtotal
			string MontoTotal = string.Empty;//Total
			string MontoIVA = string.Empty;
			string FormaPago = string.Empty;
			string TipoComprobante = string.Empty;
			string MetodoPago = string.Empty;
			string LugarexpedicionCp = string.Empty;
			string Exportacion = string.Empty;
			string NoCertificadoCSD = string.Empty;
			string Certificado = string.Empty;
			string SelloDigitalDelEmisorCFDI = string.Empty;

			//Variables para Nodo Emisor
			string RfcEmpresaEmisor = string.Empty;
			string NombreEmpresaEmisor = string.Empty;
			string RegimenFiscalEmisor = string.Empty;

			//Variables para Nodo Receptor
			string RfcReceptor = string.Empty;
			string NombreReceptor = string.Empty;
			string UsoCfdiReceptor = string.Empty;
			string DomicilioFiscalReceptor = string.Empty;
			string RegimenFiscalReceptor = string.Empty;

			//Datos de Conceptos
			string ClaveProdServ = string.Empty;
			string NoIdentificacion = string.Empty;
			string Cantidad = string.Empty;
			string ClaveUnidad = string.Empty;
			string Descripcion = string.Empty;
			//string ValorUnitario = string.Empty;//Este valor es elmonto sin IVA
			//string Importe = string.Empty;//Este valor es elmonto sin IVA
			string ObjetoImp = string.Empty;

			//Datos de Impuestos
			//string Base = string.Empty;//Este valor es elmonto sin IVA
			string Impuesto = string.Empty;
			string TipoFactor = string.Empty;
			string TasaOCuota = string.Empty;
			string ImporteIVA = string.Empty;

			XmlDocument xmlDocResponse = new XmlDocument();

			xmlDocResponse.LoadXml(xmlsat);
			//agregamos un Namespace, que usaremos para buscar que el nodo no exista:
			XmlNamespaceManager nsm = new XmlNamespaceManager(xmlDocResponse.NameTable);
			nsm.AddNamespace("cfdi", "http://www.sat.gob.mx/cfd/4");
			nsm.AddNamespace("Comprobante", "http://www.sat.gob.mx/cfd/4");
			nsm.AddNamespace("Impuestos", "http://www.sat.gob.mx/cfd/4");

			XmlNode NodoComprobante = xmlDocResponse.SelectSingleNode("//Comprobante:Comprobante", nsm);
			//Obtenemos valores
			Version = NodoComprobante.Attributes["Version"]?.Value;
			Serie = NodoComprobante.Attributes["Serie"]?.Value;
			Folio = NodoComprobante.Attributes["Folio"]?.Value;
			FechaEmision = NodoComprobante.Attributes["Fecha"]?.Value;
			Moneda = NodoComprobante.Attributes["Moneda"]?.Value;
			TipoCambio = NodoComprobante.Attributes["TipoCambio"]?.Value;
			MonTotalSinIVA = NodoComprobante.Attributes["SubTotal"]?.Value;
			MontoTotal = NodoComprobante.Attributes["Total"]?.Value;
			FormaPago = NodoComprobante.Attributes["FormaPago"]?.Value;
			TipoComprobante = NodoComprobante.Attributes["TipoDeComprobante"]?.Value;
			MetodoPago = NodoComprobante.Attributes["MetodoPago"]?.Value;
			LugarexpedicionCp = NodoComprobante.Attributes["LugarExpedicion"]?.Value;
			Exportacion = NodoComprobante.Attributes["Exportacion"]?.Value;
			NoCertificadoCSD = NodoComprobante.Attributes["NoCertificado"]?.Value;
			Certificado = NodoComprobante.Attributes["Certificado"]?.Value;
			SelloDigitalDelEmisorCFDI = NodoComprobante.Attributes["Sello"]?.Value;

			//***************Accedemos a nodo "Emisor"***************
			if (xmlDocResponse.SelectSingleNode("//Comprobante:Emisor", nsm) != null)
			{
				XmlNode NodoEmisor = xmlDocResponse.SelectSingleNode("//Comprobante:Emisor", nsm);
				//Obtenemos valores
				RfcEmpresaEmisor = NodoEmisor.Attributes["Rfc"]?.Value;
				NombreEmpresaEmisor = NodoEmisor.Attributes["Nombre"]?.Value;
				RegimenFiscalEmisor = NodoEmisor.Attributes["RegimenFiscal"]?.Value;
			}

			//***************Accedemos a nodo "Receptor"***************
			if (xmlDocResponse.SelectSingleNode("//Comprobante:Receptor", nsm) != null)
			{
				XmlNode NodoReceptor = xmlDocResponse.SelectSingleNode("//Comprobante:Receptor", nsm);
				//Obtenemos valores
				RfcReceptor = NodoReceptor.Attributes["Rfc"]?.Value;
				NombreReceptor = NodoReceptor.Attributes["Nombre"]?.Value;
				UsoCfdiReceptor = NodoReceptor.Attributes["UsoCFDI"]?.Value;
				DomicilioFiscalReceptor = NodoReceptor.Attributes["DomicilioFiscalReceptor"]?.Value;
				RegimenFiscalReceptor = NodoReceptor.Attributes["RegimenFiscalReceptor"]?.Value;
			}


			//***************Accedemos a nodo "Conceptos" * **************
			XmlNode nodeConceptos = NodoComprobante.SelectSingleNode("cfdi:Conceptos", nsm);
			foreach (XmlNode NodoConceptos in nodeConceptos.SelectNodes("//cfdi:Comprobante//cfdi:Conceptos//cfdi:Concepto", nsm))
			{
				ClaveProdServ = NodoConceptos.Attributes["ClaveProdServ"]?.Value;
				NoIdentificacion = NodoConceptos.Attributes["NoIdentificacion"]?.Value;
				Cantidad = NodoConceptos.Attributes["Cantidad"]?.Value;
				ClaveUnidad = NodoConceptos.Attributes["ClaveUnidad"]?.Value;
				Descripcion = NodoConceptos.Attributes["Descripcion"]?.Value;
				ObjetoImp = NodoConceptos.Attributes["ObjetoImp"]?.Value;
			}

			//***************Accedemos a nodo "Impuestos"***************
			XmlNode nodeImpuestos = NodoComprobante.SelectSingleNode("cfdi:Impuestos", nsm);
			//string TotalImpuestosTrasladados = nodeImpuestos.Attributes["TotalImpuestosTrasladados"]?.Value;
			foreach (XmlNode NodoImpuestos in nodeImpuestos.SelectNodes("cfdi:Traslados", nsm))
			{
				XmlNode NodoTraslados = NodoImpuestos.SelectSingleNode("cfdi:Traslado", nsm);
				//Obtenemos valores
				Impuesto = NodoTraslados.Attributes["Impuesto"]?.Value;
				TipoFactor = NodoTraslados.Attributes["TipoFactor"]?.Value;
				TasaOCuota = NodoTraslados.Attributes["TasaOCuota"]?.Value;
				ImporteIVA = NodoTraslados.Attributes["Importe"]?.Value;
			}

			string directorioBase = AppDomain.CurrentDomain.BaseDirectory;
			string HtmlContent = File.ReadAllText($@"{directorioBase}Plantilla\PlantillaFactura.html");
			HtmlContent = HtmlContent.Replace("{{nombre_empresa_emisor}}", NombreEmpresaEmisor);
			HtmlContent = HtmlContent.Replace("{{emitido_en}}", "CDMX");
			HtmlContent = HtmlContent.Replace("{{lugar_expedicion_cp}}", LugarexpedicionCp);
			HtmlContent = HtmlContent.Replace("{{tipo_comprobante}}", TipoComprobante);
			HtmlContent = HtmlContent.Replace("{{regimen_fiscal_receptor}}", RegimenFiscalReceptor);
			HtmlContent = HtmlContent.Replace("{{uso_cfdi_receptor}}", UsoCfdiReceptor);


			HtmlContent = HtmlContent.Replace("{{rfc_empresa_emisor}}", RfcEmpresaEmisor);
			HtmlContent = HtmlContent.Replace("{{serie}}", Serie);
			HtmlContent = HtmlContent.Replace("{{folio}}", Folio.ToString());
			HtmlContent = HtmlContent.Replace("{{folio_uuid}}", TFD_foliofiscalUUID);
			HtmlContent = HtmlContent.Replace("{{no_de_certificado_sat}}", TFD_noCertificadoSAT);
			HtmlContent = HtmlContent.Replace("{{fecha_timbrado_certificacion}}", TFD_FechaTimbradoCertificacion);
			HtmlContent = HtmlContent.Replace("{{no_de_certificado_csd}}", NoCertificadoCSD);
			HtmlContent = HtmlContent.Replace("{{fecha_emision}}", FechaEmision);

			HtmlContent = HtmlContent.Replace("{{rfc_receptor}}", RfcReceptor);
			HtmlContent = HtmlContent.Replace("{{nombre_receptor}}", NombreReceptor);
			HtmlContent = HtmlContent.Replace("{{domicilio_fiscal_receptor}}", DomicilioFiscalReceptor);

			HtmlContent = HtmlContent.Replace("{{claveprod}}", ClaveProdServ);
			HtmlContent = HtmlContent.Replace("{{descripcion}}", Descripcion);
			HtmlContent = HtmlContent.Replace("{{cantidad}}", Cantidad);
			HtmlContent = HtmlContent.Replace("{{precio_unitario}}", MontoTotal.ToString());
			HtmlContent = HtmlContent.Replace("{{importe}}", MontoTotal.ToString());
			HtmlContent = HtmlContent.Replace("{{total_letra}}", "total letra");
			HtmlContent = HtmlContent.Replace("{{subtotal}}", MonTotalSinIVA.ToString());
			HtmlContent = HtmlContent.Replace("{{monto_iva}}", ImporteIVA.ToString());
			HtmlContent = HtmlContent.Replace("{{total}}", MontoTotal.ToString());
			HtmlContent = HtmlContent.Replace("{{metodo_de_pago}}", MetodoPago);
			HtmlContent = HtmlContent.Replace("{{forma_de_pago}}", FormaPago);

			HtmlContent = HtmlContent.Replace("{{sello_del_emisor_cfdi}}", SelloDigitalDelEmisorCFDI);
			HtmlContent = HtmlContent.Replace("{{sello_sat}}", TFD_selloSAT);
			HtmlContent = HtmlContent.Replace("{{cadena_original_sat}}", cadenaOriginalSAT);
			HtmlContent = HtmlContent.Replace("{{codigo_qr}}", CFD_codigoDeBarras);

			return HtmlContent;
		}
	}
}

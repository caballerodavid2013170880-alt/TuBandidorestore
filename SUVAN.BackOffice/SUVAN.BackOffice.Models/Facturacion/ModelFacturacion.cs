using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Models.Facturacion
{
    public class ModelFacturacion
    {
    }

    public class ModelDatosTransaccion 
    {
        public int Idtransaccion { get; set; }
        public int MetodopagoIdmetodopago { get; set; }

        public decimal Cantidad { get; set; }

        public int TipotransaccionIdtipotransaccion { get; set; }

        public int UsuarioIdusuario { get; set; }

        public string? Numeroordenpay { get; set; }

        public string? Numeropeticionpay { get; set; }

        public int Idviaje { get; set; }

        public int EmpresaIdempresa { get; set; }

        public bool? Facturado { get; set; }
    }

    public class DatosFacturacionEmisor
    {
        public string NombreEmpresa { get; set; }

        public string RfcEmpresa { get; set; }

        public string Serie { get; set; }

        public int Folio { get; set; }

        public string RegimenfiscalEmisor { get; set; }

        public string LugarexpedicionCp { get; set; }
    }

    public class DatosFacturacionReceptor
    {
        public string RfcReceptor { get; set; }

        public string NombreReceptor { get; set; }

        public string? UsoCfdiReceptor { get; set; }

        public string DomicilioFiscalReceptor { get; set; }

        public string RegimenFiscalReceptor { get; set; }
    }
}

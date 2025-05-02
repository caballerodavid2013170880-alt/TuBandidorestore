using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Models.UnlimintPay.Pago
{
    public class RespuestaValidacionPago
    {
        private string codigoError { get; set; }
        private string error { get; set; }
        private RespuestaValidacionPagoWS datosPago { get; set; }
        public string CodigoError { get { return codigoError; } }
        public string Error { get { return error; } }
        public RespuestaValidacionPagoWS DatosPago { get { return datosPago; } }

        public void generar(string pmtCodigoError, string pmtError, RespuestaValidacionPagoWS pmtRespuesta = null)
        {
            this.codigoError = pmtCodigoError;
            this.error = pmtError;
            if (pmtRespuesta != null)
            {
                //this.datosPago =new DatosPago(pmtRespuesta);
                this.datosPago = pmtRespuesta;
            }
        }
    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Models.UnlimintPay.Pago
{
    public class RespuestaGeneracionPago
    {
        private string codigoError { get; set; }
        private string error { get; set; }
        private string redirectUrl { get; set; }
        public string CodigoError { get { return codigoError; } }
        public string Error { get { return error; } }
        public string RedirectUrl { get { return redirectUrl; } }

        public void generar(string pmtCodigoError, string pmtError, string pmtRedirectUrl)
        {
            this.codigoError = pmtCodigoError;
            this.error = pmtError;
            this.redirectUrl = pmtRedirectUrl;
        }
    }
}

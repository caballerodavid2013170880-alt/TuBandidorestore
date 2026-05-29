using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Models.UnlimintPay.Pago
{
    public class PeticionValidacionPago
    {
        public string endPoint { get; set; }
        public string token { get; set; }
        public string request_id { get; set; }
        public string merchant_order_id { get; set; }
    }
}

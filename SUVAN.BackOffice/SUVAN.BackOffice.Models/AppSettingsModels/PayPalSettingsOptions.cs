using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Models.AppSettingsModels
{
    public class PayPalSettingsOptions
    {

        public string endPointAmbiente { get; set; }

        public string endPointCrearOrden { get; set; }

        public string endPointOrdersCapture { get; set; }

        public string client_id { get; set; }

        public string client_secret { get; set; }

        public string endPointGeneraTokenPayPal { get; set; }

        public string success_url { get; set; }
        public string cancel_url { get; set; }

        public string description_pay { get; set; }

    }
}

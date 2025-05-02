using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Models.AppSettingsModels
{
    public class UnlimitPaySettingsOptions
    {
        public string endPointGeneraToken { get; set; }

        public string endPointAmbiente { get; set; }

        public string terminal_code { get; set; }

        public string password { get; set; }

        public string endPointCreatePaymentPage { get; set; }

        public string endPointValidacionPago { get; set; }

        public string success_url { get; set; }
        public string cancel_url { get; set;}
        public string inprocess_url { get; set; }
        public string decline_url { get; set; }

        public string payment_method { get; set; }
        public string description_pay { get; set; }
    }
}
